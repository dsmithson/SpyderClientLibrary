using PCLStorage;
using Knightware.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knightware.Diagnostics;
using Knightware.Primitives;
using System.Xml.Linq;
using System.Xml;
using Knightware.Net.Sockets;
using Spyder.Client.Net;

namespace Spyder.Client.Images
{
    public delegate void ProcessImageStreamHandler<K, T>(object sender, ProcessImageStreamEventArgs<K, T> e);

    public abstract class QFTThumbnailManagerBase<K, T, U> : ThumbnailManagerBase<K, T, U>
        where U : ThumbnailImageBase<K, T>, new()
        where T : class
        where K : QFTThumbnailIdentifier
    {
        private IFolder localImagesFolder;
        private string remoteImagePath = @"c:\spyder\images";
        private readonly Dictionary<string, QFTClient> qftClients;
        private Func<IStreamSocket> getStreamSocket;
        private Dictionary<QFTThumbnailIdentifier, Size> knownResolutions = new Dictionary<QFTThumbnailIdentifier, Size>();

        public string RemoteImagePath
        {
            get { return remoteImagePath; }
            set { remoteImagePath = value; }
        }

        /// <summary>
        /// Initiliazes a new instance of the QFTThumbnailManagerBase class
        /// </summary>
        /// <param name="localImagesFolder">root directory for storing downloaded images (cache).  Subfolders will be created for each sub-server IP.</param>
        protected QFTThumbnailManagerBase(Func<IStreamSocket> getStreamSocket, IFolder localImagesFolder)
        {
            this.localImagesFolder = localImagesFolder;
            this.getStreamSocket = getStreamSocket;

            this.qftClients = new Dictionary<string, QFTClient>();
        }

        public override async Task ShutdownAsync()
        {
            //Let the base handle it's cleanup
            await base.ShutdownAsync();

            //Clear out any existing QFT Clients
            if (qftClients.Count > 0)
            {
                List<Task> tasks = new List<Task>();
                foreach (var qftClient in qftClients.Values)
                {
                    tasks.Add(qftClient.ShutdownAsync());
                }
                await Task.WhenAll(tasks);
                this.qftClients.Clear();
            }
        }

        protected override U CreateNewThumbnail(K identifier)
        {
            U response = new U();

            //Check to see if we have a known resolution for this image, previously loaded
            if (identifier != null)
            {
                var resolutionIdentifier = new QFTThumbnailIdentifier(identifier.ServerIP, identifier.FileName);
                if (knownResolutions.ContainsKey(resolutionIdentifier))
                {
                    response.NativeResolution = knownResolutions[resolutionIdentifier];
                }
            }

            return response;
        }

        protected abstract Task<ProcessedImageResult> ScaleImageAsync(K identifier, ImageSize targetSize, Stream nativeImageStream);

        protected abstract Task<T> ProcessImageAsync(K identifier, Stream fileStream);

        public virtual async Task<Stream> GetImageStreamAsync(K identifier, ImageSize targetSize)
        {
            //If we have no local backing store, we can't get an image stream.
            var serverImagesFolder = await GetFolder(identifier.ServerIP);
            if (serverImagesFolder == null)
                return null;

            string scaledFolderName = "Scaled_" + (int)targetSize;
            try
            {
                Stream fileStream = null;
                bool fileStreamIsScaled = false;

                //First, lets look for an existing copy of this file in our local storage.  If it exists, use it.
                //First look for a cached image file pre-scaled
                try
                {
                    var preScaledFolder = await serverImagesFolder.GetFolderAsync(scaledFolderName);

                    if (preScaledFolder != null)
                    {
                        //Look for the existance of a pre-scaled image already in our folder
                        try
                        {
                            var file = await preScaledFolder.GetFileAsync(identifier.FileName);
                            if (file != null)
                            {
                                fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);
                                fileStreamIsScaled = true;
                            }
                        }
                        catch (FileNotFoundException)
                        {
                            //No such file exists
                            fileStream = null;
                        }
                        catch (Exception ex)
                        {
                            fileStream = null;
                            TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to open file '{1}': {2}",
                                ex.GetType().Name, identifier.FileName, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Folder must not exist, or is not accessible
                    fileStream = null;
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to open pre-scaled image folder '{1}': {2}",
                                ex.GetType().Name, scaledFolderName, ex.Message);
                }

                //Did we find the file in our pre-scaled image cache?
                if (fileStream == null)
                {
                    try
                    {
                        //Look in our unscaled image cache
                        var file = await serverImagesFolder.GetFileAsync(identifier.FileName);
                        if (file != null)
                        {
                            fileStream = await file.OpenAsync(PCLStorage.FileAccess.Read);
                            if (fileStream.Length == 0)
                            {
                                //Invalid file.  Remove it.
                                fileStream.Dispose();
                                fileStream = null;
                                await file.DeleteAsync();
                            }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        //File doesn't exist yet in our local cache
                    }
                    catch (Exception ex)
                    {
                        TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to get local file '{1}': {2}",
                                ex.GetType().Name, identifier.FileName, ex.Message);
                    }
                }

                //If we didn't get a stream from our local file cache above, let's fetch it from QFT and save it locally
                if (fileStream == null)
                {
                    var qftClient = await GetQFTClient(identifier.ServerIP);
                    if (qftClient != null)
                    {
                        IFile newFile = await serverImagesFolder.CreateFileAsync(identifier.FileName, CreationCollisionOption.ReplaceExisting);
                        fileStream = await newFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite);
                        string remoteFile = qftClient.ConvertAbsolutePathToRelative(Path.Combine(remoteImagePath, identifier.FileName));
                        if (await qftClient.ReceiveFile(remoteFile, fileStream, null))
                        {
                            //Download succeeded.  Seek to the beginning of the file stream so it can be read below
                            fileStream.Seek(0, SeekOrigin.Begin);
                        }
                        else
                        {
                            //Download failed.  Delete our local cache file
                            fileStream.Dispose();
                            fileStream = null;

                            await newFile.DeleteAsync();
                        }
                    }
                }

                //If we have a file stream, but it isn't scaled, then let's scale it now
                if (fileStream != null && !fileStreamIsScaled && targetSize != ImageSize.Native)
                {
                    //TODO: Store this native image size in our thumbnail image
                    ProcessedImageResult imageResult = await ScaleImageAsync(identifier, targetSize, fileStream);

                    //dispose our native file stream and replace it with this scaled one
                    fileStream.Dispose();
                    fileStream = null;

                    if (imageResult != null && imageResult.ScaledStream != null)
                    {
                        //Store the native image size to our identifier
                        //NOTE:  I'm allowing the overridden class to actually set the native resolution to the ThumbnailImage, so it can do so in a thread-safe mannor
                        await OnNativeResolutionAvailable(identifier.ServerIP, identifier.FileName, imageResult.NativeSize);
                        await SaveImageMetadata(identifier.ServerIP);

                        //Assign the scaled image result to our response strema, and seek it back to the beginning for the next use
                        fileStream = imageResult.ScaledStream;
                        fileStream.Seek(0, SeekOrigin.Begin);

                        var preScaledFolder = await serverImagesFolder.CreateFolderAsync(scaledFolderName, CreationCollisionOption.OpenIfExists);
                        var preScaledFile = await preScaledFolder.CreateFileAsync(identifier.FileName, CreationCollisionOption.ReplaceExisting);
                        using (var stream = await preScaledFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            await fileStream.CopyToAsync(stream);
                        }
                    }
                }

                return fileStream;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to get image file stream: {1}", ex.GetType().Name, ex.Message);
                return null;
            }
        }

        protected override async Task<T> GenerateImageAsync(K identifier, ImageSize targetSize)
        {
            Stream fileStream = null;
            try
            {
                //If this is the first image loaded on this server, then load the metadata for images previously collected (if any)
                bool isFirstImage = false;
                PerformImageListOperation(images => isFirstImage = images.Where(entry => entry.Key.ServerIP == identifier.ServerIP).Count() == 1);
                if (isFirstImage)
                {
                    await LoadImageMetadata(identifier.ServerIP);
                }

                fileStream = await GetImageStreamAsync(identifier, targetSize);

                //We should have a file stream now that we can use to generate our image.  Send the stream up for processing
                if (fileStream != null)
                {
                    return await ProcessImageAsync(identifier, fileStream);
                }
                else
                {
                    //Failed to load image.  Return null;
                    return default(T);
                }
            }
            finally
            {
                //Clean up our file stream (if present)
                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }
            }
        }

        protected virtual Task OnNativeResolutionAvailable(string serverIP, string fileName, Size nativeResolution)
        {
            //Update existing list items
            PerformImageListOperation((list) =>
                {
                    foreach (var entry in list)
                    {
                        if (string.Compare(entry.Key.ServerIP, serverIP, StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(fileName, entry.Key.FileName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            entry.Value.NativeResolution = nativeResolution;
                        }
                    }
                });

            //Update our internal list of known resolutions
            var identifier = new QFTThumbnailIdentifier(serverIP, fileName);
            lock (knownResolutions)
            {
                if (knownResolutions.ContainsKey(identifier))
                {
                    knownResolutions[identifier] = nativeResolution;
                }
                else
                {
                    knownResolutions.Add(identifier, nativeResolution);
                }
            }

            return Task.FromResult(true);
        }

        protected async Task<IFolder> GetFolder(string serverIP)
        {
            if (localImagesFolder == null)
                return null;

            var serverRootFolder = await localImagesFolder.CreateFolderAsync(serverIP, CreationCollisionOption.OpenIfExists);
            var imageRootFolder = await serverRootFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            return imageRootFolder;
        }

        private async Task<QFTClient> GetQFTClient(string serverIP)
        {
            QFTClient response = null;
            if (qftClients.ContainsKey(serverIP))
            {
                response = qftClients[serverIP];
            }
            else
            {
                response = new QFTClient(getStreamSocket, serverIP);
                qftClients.Add(serverIP, response);
            }

            if (!response.IsRunning)
            {
                if (!await response.StartupAsync())
                    return null;
            }

            return response;
        }

        #region Metadata Saving / Loading

        protected bool imageMetadataSaveRequred;
        private const string imageMetadataFileName = "ThumbnailImageMetadata.xml";

        protected async Task<bool> LoadImageMetadata(string serverIP)
        {
            var serverImagesFolder = await GetFolder(serverIP);
            if (serverImagesFolder == null)
                return false;

            var file = (await serverImagesFolder.GetFilesAsync()).FirstOrDefault(f => string.Compare(f.Name, imageMetadataFileName, StringComparison.CurrentCultureIgnoreCase) == 0);
            if (file != null)
            {
                try
                {
                    using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.Read))
                    {
                        if (stream.Length > 0)
                        {
                            if (LoadImageMetadata(serverIP, stream))
                                return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while opening metadata file for {1}: {2}",
                        ex.GetType().Name, serverIP, ex.Message);
                }

                //Delete this file - it appears to have failed to load above
                await file.DeleteAsync();
            }
            return false;
        }

        protected bool LoadImageMetadata(string serverIP, Stream metadataFileStream)
        {
            if (string.IsNullOrEmpty(serverIP) || metadataFileStream == null)
                return false;

            try
            {
                //Load our thumbnail metadata
                var metadata = XDocument.Load(metadataFileStream);
                var thumbnailMetadataItems = metadata.Descendants("ThumbnailImages")
                    .SelectMany((item) => item.Descendants("ThumbnailImage"))
                    .Select((item) => new
                        {
                            FileName = item.Element("FileName").Value,
                            Width = int.Parse(item.Element("Width").Value),
                            Height = int.Parse(item.Element("Height").Value)
                        });

                //Now write the metadata into thumbnail objects
                foreach (var thumbnailMetadataItem in thumbnailMetadataItems)
                {
                    //Set thumbnail metadata properties
                    OnNativeResolutionAvailable(serverIP, thumbnailMetadataItem.FileName, new Size(thumbnailMetadataItem.Width, thumbnailMetadataItem.Height));
                }
                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while loading thumbnail metadata: {1}", ex.GetType().Name, ex.Message);
                return false;
            }
        }

        protected async Task<bool> SaveImageMetadata(string serverIP)
        {
            var serverImagesFolder = await GetFolder(serverIP);
            if (serverImagesFolder == null)
                return false;

            try
            {
                var file = await serverImagesFolder.CreateFileAsync(imageMetadataFileName, CreationCollisionOption.ReplaceExisting);
                if (file == null)
                    return false;

                using (Stream stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    return SaveImageMetadata(serverIP, stream);
                }
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while saving metadata file for {1}: {2}",
                    ex.GetType().Name, serverIP, ex.Message);
            }
            return false;
        }

        protected bool SaveImageMetadata(string serverIP, Stream metadataFileStream)
        {
            if (string.IsNullOrEmpty(serverIP) || metadataFileStream == null)
                return false;

            try
            {
                //Create a local list of items to be serialized
                var imageItems = new List<Tuple<string, int, int>>();
                PerformImageListOperation(imageDictionary =>
                    {
                        imageItems = imageDictionary
                            .Where(entry => entry.Key.ServerIP == serverIP)
                            .Select(entry => new Tuple<string, int, int>(entry.Key.FileName, entry.Value.NativeResolution.Width, entry.Value.NativeResolution.Height))
                            .ToList();
                    });

                using (XmlWriter writer = XmlWriter.Create(metadataFileStream))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("ThumbnailManagerMetadata");
                    writer.WriteStartElement("ThumbnailImages");

                    foreach (var item in imageItems)
                    {
                        writer.WriteStartElement("ThumbnailImage");
                        writer.WriteElementString("FileName", item.Item1);
                        writer.WriteElementString("Width", item.Item2.ToString());
                        writer.WriteElementString("Height", item.Item3.ToString());
                    }

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while saving thumbnail metadata: {1}", ex.GetType().Name, ex.Message);
                return false;
            }
        }

        #endregion

        #region Send Image to server

        public virtual async Task<bool> SetImageAsync(K identifier, Stream stream, FileProgressDelegate progress = null)
        {
            if (identifier == null || stream == null)
                return false;

            try
            {
                var client = await GetQFTClient(identifier.ServerIP);
                if (client == null)
                    return false;

                string remoteFile = QFTClient.ConvertToRelativePath(Path.Combine(@"c:\spyder\images", identifier.FileName));
                stream.Seek(0, SeekOrigin.Begin);
                if (!await client.SendFile(stream, remoteFile, progress))
                    return false;

                //Before we return, let's also write the file to our local cache to avoid a subsequent download
                try
                {
                    var serverImagesFolder = await GetFolder(identifier.ServerIP);
                    if (serverImagesFolder != null)
                    {
                        IFile newFile = await serverImagesFolder.CreateFileAsync(identifier.FileName, CreationCollisionOption.ReplaceExisting);
                        using (var fileStream = await newFile.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //even though this failed, we'll still return true since we succeeded in pushing the file to the server
                    TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to write image file '{1}' to local cache: {2}",
                        ex.GetType().Name, identifier.FileName, ex.Message);
                }

                return true;
            }
            catch (Exception ex)
            {
                TraceQueue.Trace(this, TracingLevel.Warning, "{0} occurred while trying to send image file '{1}' to the server: {2}",
                    ex.GetType().Name, identifier.FileName, ex.Message);

                return false;
            }
        }

        #endregion
    }
}
