using Knightware.Diagnostics;
using Knightware.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spyder.Client.Images
{
    /// <summary>
    /// Manages the generic management of loading images for a user interface.
    /// </summary>
    /// <typeparam name="T">Image type being loaded by this class.  Example would be BitmapSource.</typeparam>
    /// <typeparam name="K">Key type used to uniquely identify images. Example could be a string filename.</typeparam>
    /// <typeparam name="U">U is the ThumbnailImageType being managed by this manager</typeparam>
    public abstract class ThumbnailManagerBase<K, T, U>
        where U : ThumbnailImageBase<K, T>, new()
        where T : class
    {
        private readonly object imagesLock = new object();
        private Dictionary<K, U> images;
        private AsyncListProcessor<ThumbnailListItem> imageProcessor;

        public bool IsRunning { get; private set; }

        public virtual async Task<bool> StartupAsync()
        {
            await ShutdownAsync();
            IsRunning = true;

            lock (imagesLock)
            {
                images = new Dictionary<K, U>();
            }

            imageProcessor = new AsyncListProcessor<ThumbnailListItem>(ProcessThumbnailListItem, maxDegreeOfParallelism: Environment.ProcessorCount);
            if (!await imageProcessor.StartupAsync())
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "Failed to initialize image processor.  Shutting down...");
                await ShutdownAsync();
                return false;
            }

            return true;
        }

        public virtual async Task ShutdownAsync()
        {
            IsRunning = false;

            if (imageProcessor != null)
            {
                await imageProcessor.ShutdownAsync();
                imageProcessor = null;
            }

            lock (imagesLock)
            {
                if (images != null)
                {
                    foreach (var image in images.Values)
                    {
                        image.CreateImageRequested -= thumbnail_CreateImageRequested;
                    }
                    images = null;
                }
            }
        }

        public U GetThumbnail(K identifier)
        {
            if (!IsRunning)
                throw new InvalidOperationException("ThumbnailManager instance is not running");

            U response = null;
            PerformImageListOperation((list) =>
                {
                    if (images.ContainsKey(identifier))
                    {
                        response = images[identifier];
                    }
                    else
                    {
                        response = CreateNewThumbnail(identifier);
                        response.Key = identifier;
                        response.CreateImageRequested += thumbnail_CreateImageRequested;
                        list.Add(identifier, response);
                    }
                });

            return response;
        }

        protected virtual U CreateNewThumbnail(K identifier)
        {
            return new U();
        }

        protected void PerformImageListOperation(Action<Dictionary<K, U>> action)
        {
            lock (imagesLock)
            {
                action(images);
            }
        }

        private void thumbnail_CreateImageRequested(object sender, CreateImageRequestEventArgs e)
        {
            imageProcessor.Add(new ThumbnailListItem()
            {
                Size = e.ImageSize,
                ThumbnailImage = (U)sender,
            });

            e.Handled = true;
        }

        private async Task ProcessThumbnailListItem(AsyncListProcessorItemEventArgs<ThumbnailListItem> e)
        {
            T result = null;
            try
            {
                result = await GenerateImageAsync(e.Item.ThumbnailImage.Key, e.Item.Size);
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while processing thumbnail image: {1}", ex.GetType().Name, ex.Message);
                result = null;
            }

            //Set result on item, even if we errored out above
            Task t = e.Item.ThumbnailImage.Dispatcher.BeginInvoke(() => e.Item.ThumbnailImage.SetImage(e.Item.Size, result));
        }

        /// <summary>
        /// Generates and returns an image for a supplied thumbnailImage instance, at a specified target size
        /// </summary>
        protected abstract Task<T> GenerateImageAsync(K identifier, ImageSize targetSize);

        protected class ThumbnailListItem
        {
            /// <summary>
            /// Requesting thumbnail image object
            /// </summary>
            public U ThumbnailImage { get; set; }

            /// <summary>
            /// Targeted size of image being requested
            /// </summary>
            public ImageSize Size { get; set; }
        }
    }
}
