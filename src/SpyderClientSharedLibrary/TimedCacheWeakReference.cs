using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Spyder.Client.Threading;

namespace Spyder.Client
{
    public class TimedCacheWeakReference<T> where T: class
    {
        #region Static Cache Expiration Monitor

        private const int cacheTimerInterval = 1000;

        private static readonly object cacheExpirationLock = new object();
        private static Timer cacheExpirationTimer;
        private static readonly List<CacheExpirationRecord> registeredCacheExpirationItems = new List<CacheExpirationRecord>();

        private static void RegisterExpiration(TimedCacheWeakReference<T> classToRegister, T target)
        {
            lock (cacheExpirationLock)
            {
                //Force set the target while we are within this class, in case it was cleared out in our expiration handler while we were waiting for the lock
                classToRegister.strongReference = target;

                var existing = registeredCacheExpirationItems.FirstOrDefault(item => item.TimedCacheWeakReference == classToRegister);
                if (target == null)
                {
                    //Strong reference has already been claimed, so remove this item from the collection
                    registeredCacheExpirationItems.Remove(existing);
                }
                else
                {
                    if (existing == null)
                    {
                        //Register a new item for expiration
                        registeredCacheExpirationItems.Add(new CacheExpirationRecord()
                        {
                            ExpirationTime = DateTime.Now.Add(classToRegister.cacheDuration),
                            TimedCacheWeakReference = classToRegister
                        });
                    }
                    else
                    {
                        //Extend expiration lease
                        existing.ExpirationTime = DateTime.Now.Add(classToRegister.cacheDuration);
                    }

                    //Initialize our timer if it isn't already running
                    if (cacheExpirationTimer == null)
                    {
                        //TODO:  May need to make this evaluate on a shorter time scale
                        cacheExpirationTimer = new Timer(OnCacheExpirationTimer_Elapsed, null, cacheTimerInterval, cacheTimerInterval);
                    }
                }
            }
        }

        private static void OnCacheExpirationTimer_Elapsed(object state)
        {
            //Stop timer immediately while processing
            cacheExpirationTimer.Change(Timeout.Infinite, Timeout.Infinite);

            lock (cacheExpirationLock)
            {
                int index = 0;
                while (index < registeredCacheExpirationItems.Count)
                {
                    var item = registeredCacheExpirationItems[index];
                    if (item.ExpirationTime < DateTime.Now)
                    {
                        //Cache timeout has expired
                        item.TimedCacheWeakReference.strongReference = null;
                        registeredCacheExpirationItems.RemoveAt(index);
                    }
                    else
                    {
                        //Item has not yet expired
                        index++;
                    }
                }

                //Restart our timer under the lock, but only if our list still contains items
                if (registeredCacheExpirationItems.Count > 0)
                {
                    cacheExpirationTimer.Change(cacheTimerInterval, cacheTimerInterval);
                }
            }
        }

        private class CacheExpirationRecord
        {
            public TimedCacheWeakReference<T> TimedCacheWeakReference { get; set; }
            public DateTime ExpirationTime { get; set; }
        }

        #endregion

        private WeakReference<T> weakReference;
        private T strongReference;

        private TimeSpan cacheDuration = TimeSpan.FromMinutes(1);
        public TimeSpan CacheDuration
        {
            get { return cacheDuration; }
        }

        public bool StrongReferenceAvailable
        {
            get { return strongReference != null; }
        }
        
        /// <summary>
        /// Initializes a new instance of the TimedCacheWeakReference<T> class that references the specified object
        /// </summary>
        /// <param name="target">The object to reference, or null</param>
        public TimedCacheWeakReference(T target, TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;

            if(target != null)
                SetTarget(target);
        }

        public void SetTarget(T target)
        {
            SetTarget(target, cacheDuration);
        }

        public void SetTarget(T target, TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;
            this.strongReference = target;

            //Set our weak reference
            if (weakReference == null)
            {
                this.weakReference = new WeakReference<T>(target);
            }
            else
            {
                this.weakReference.SetTarget(target);
            }

            //Start the cache expiration timer
            if (target != null && cacheDuration != TimeSpan.Zero)
            {
                RegisterExpiration(this, target);
            }
        }

        /// <summary>
        /// Tries to retrieve the target object that is referenced by the current System.WeakReference<T> object.
        /// </summary>
        /// <param name="target">When this method returns, contains the target object, if it is available.  This parameter is treated as uninitialized.</param>
        /// <returns>true if the target was retrieved; otherwise, false.</returns>
        public bool TryGetTarget(out T target)
        {
            bool success = false;

            //Do we have a strong reference available?
            target = strongReference;
            if (target != null)
            {
                success = true;
            }
            else if (weakReference.TryGetTarget(out target))
            {
                success = true;
            }

            if (success)
            {
                //Reset our cache timer, and then ensure our strong reference is up to date
                RegisterExpiration(this, target);
            }

            return success;
        }
    }
}
