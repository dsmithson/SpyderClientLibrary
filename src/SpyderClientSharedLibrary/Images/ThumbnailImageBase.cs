using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Diagnostics;
using Knightware.Primitives;
using Knightware.Threading;
using Knightware;

namespace Spyder.Client.Images
{
    public delegate void CreateImageRequestHandler(object sender, CreateImageRequestEventArgs e);

    /// <summary>
    /// Represents a generic container for an image which can be requested at small, medium, and large sizes
    /// </summary>
    /// <typeparam name="T">ImageType represented by this class.  BitmapSource is an example of this value.</typeparam>
    /// <typeparam name="K">Key type used to uniquely identify this image class.  An example could be a string filename, or URI</typeparam>
    public abstract class ThumbnailImageBase<K, T> : DispatcherPropertyChangedBase
        where T : class
    {
        private TaskCompletionSource<bool> nativeResolutionTcs = new TaskCompletionSource<bool>();
        private readonly Dispatcher dispatcher = Dispatcher.Current;
        private bool extraSmallImageLoadFailed;
        private bool smallImageLoadFailed;
        private bool mediumImageLoadFailed;
        private bool largeImageLoadFailed;
                
        /// <summary>
        /// Event raised indicating that an image file needs to be requested
        /// </summary>
        public event CreateImageRequestHandler CreateImageRequested;
        protected void OnCreateImageRequested(CreateImageRequestEventArgs e)
        {
            if (CreateImageRequested != null)
                CreateImageRequested(this, e);
        }

        /// <summary>
        /// Dispatcher associated with this item
        /// </summary>
        public Dispatcher Dispatcher { get { return dispatcher; } }

        /// <summary>
        /// Key field used to uniquely identify this resource.  Could be a string 
        /// </summary>
        public virtual K Key
        {
            get { return key; }
            set { key = value; }
        }
        private K key;

        private Size nativeResolution;
        public virtual Size NativeResolution
        {
            get { return nativeResolution; }
            set
            {
                if (nativeResolution != value)
                {
                    nativeResolution = value;
                    OnPropertyChanged();

                    //Free any awaiters
                    if (nativeResolutionTcs != null)
                        nativeResolutionTcs.TrySetResult(true);
                }
            }
        }

        private bool isLoadingExtraSmallImage;
        public bool IsLoadingExtraSmallImage
        {
            get { return isLoadingExtraSmallImage; }
            set
            {
                if (isLoadingExtraSmallImage != value)
                {
                    isLoadingExtraSmallImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimedCacheWeakReference<T> extraSmallImage;
        public virtual T ExtraSmallImage
        {
            get { return TryGetImage(extraSmallImage, extraSmallImageLoadFailed, ImageSize.ExtraSmall, () => IsLoadingExtraSmallImage, (isLoading) => IsLoadingExtraSmallImage = isLoading); }
        }

        private bool isLoadingSmallImage;
        public bool IsLoadingSmallImage
        {
            get { return isLoadingSmallImage; }
            set
            {
                if (isLoadingSmallImage != value)
                {
                    isLoadingSmallImage = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private TimedCacheWeakReference<T> smallImage;
        public virtual T SmallImage
        {
            get { return TryGetImage(smallImage, smallImageLoadFailed, ImageSize.Small, () => IsLoadingSmallImage, (isLoading) => IsLoadingSmallImage = isLoading); }
        }

        private bool isLoadingMediumImage;
        public bool IsLoadingMediumImage
        {
            get { return isLoadingMediumImage; }
            set
            {
                if (isLoadingMediumImage != value)
                {
                    isLoadingMediumImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimedCacheWeakReference<T> mediumImage;
        public virtual T MediumImage
        {
            get 
            {
                return TryGetImage(mediumImage, mediumImageLoadFailed, ImageSize.Medium, () => IsLoadingMediumImage, (isLoading) => IsLoadingMediumImage = isLoading);
            }
        }

        private bool isLoadingLargeImage;
        public bool IsLoadingLargeImage
        {
            get { return isLoadingLargeImage; }
            set
            {
                if (isLoadingLargeImage != value)
                {
                    isLoadingLargeImage = value;
                    OnPropertyChanged();
                }
            }
        }

        private TimedCacheWeakReference<T> largeImage;
        public virtual T LargeImage
        {
            get { return TryGetImage(largeImage, largeImageLoadFailed, ImageSize.Large, () => IsLoadingLargeImage, (isLoading) => IsLoadingLargeImage = isLoading); }
        }
        
        private T TryGetImage(TimedCacheWeakReference<T> imageReference, bool previousImageLoadFailed, ImageSize size, Func<bool> getIsLoadingImage, Action<bool> setIsLoadingImage)
        {
            T existing;
            if (imageReference == null || !imageReference.TryGetTarget(out existing) || existing == null)
            {
                if (!getIsLoadingImage() && !previousImageLoadFailed)
                {
                    //Attempt to queue the loading of the image now
                    var e = new CreateImageRequestEventArgs() { ImageSize = size };
                    OnCreateImageRequested(e);
                    if (e.Handled)
                        setIsLoadingImage(true);
                }

                //Return the default image
                return GetDefaultImage(size);
            }

            //Image exists, so let's return it now
            return existing;
        }

        /// <summary>
        /// Returns a default image, which is used when an image being requested is not available
        /// </summary>
        protected abstract T GetDefaultImage(ImageSize size);

        /// <summary>
        /// Sets the internal reference for an internally stored image, and raises the corresponding image change property change event
        /// </summary>
        /// <param name="size">Size of the target image</param>
        /// <param name="image">Resulting image to store</param>
        /// <param name="nativeResolution">Native resolution of the image being set</param>
        public void SetImage(ImageSize size, T image)
        {
            try
            {
                switch (size)
                {
                    case ImageSize.ExtraSmall:
                        IsLoadingExtraSmallImage = false;
                        extraSmallImageLoadFailed = (image == null);
                        extraSmallImage = (image == null ? null : new TimedCacheWeakReference<T>(image, TimeSpan.FromMinutes(1)));
                        OnPropertyChanged("ExtraSmallImage");
                        break;

                    case ImageSize.Small:
                        IsLoadingSmallImage = false;
                        smallImageLoadFailed = (image == null);
                        smallImage = (image == null ? null : new TimedCacheWeakReference<T>(image, TimeSpan.FromMinutes(1)));
                        OnPropertyChanged("SmallImage");
                        break;

                    case ImageSize.Medium:
                        IsLoadingMediumImage = false;
                        mediumImageLoadFailed = (image == null);
                        mediumImage = (image == null ? null : new TimedCacheWeakReference<T>(image, TimeSpan.FromMinutes(1)));
                        OnPropertyChanged("MediumImage");
                        break;

                    case ImageSize.Large:
                        IsLoadingLargeImage = false;
                        largeImageLoadFailed = (image == null);
                        largeImage = (image == null ? null : new TimedCacheWeakReference<T>(image, TimeSpan.FromMinutes(1)));
                        OnPropertyChanged("LargeImage");
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Image size '{0}' is not supported", size));
                }
            }
            finally
            {
                //If our native resolution has not yet been set, then complete the task so that callers waiting for native resolution
                //won't hang indefinately
                if (!nativeResolutionTcs.Task.IsCompleted)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "Native resolution for Thumbnail '{0}' was not set before image was set.", this.key);
                    nativeResolutionTcs.TrySetResult(false);
                }
            }
        }

        public async Task<Size> GetNativeResolutionAsync()
        {
            //We'll use our task status to determine the state of our variable
            var task = nativeResolutionTcs.Task;

            //If the image resolution has already been set, then we can return it immediately
            if (task.IsCompleted)
                return nativeResolution;

            //If no thumbnail image is being loaded yet, then let's force the smallest image to load to force
            //the image processing pipeline to be invoked, which will set our native resolution
            if (!isLoadingLargeImage && !isLoadingMediumImage && !isLoadingSmallImage && !isLoadingExtraSmallImage)
            {
                //Force a small image pull
                var extraSmallImage = this.ExtraSmallImage;
            }

            //Return our task handle to our caller, so they can wait for the image
            //TODO:  Put a timeout in here to prevent a possible hang
            await task;

            return nativeResolution;
        }
    }
}
