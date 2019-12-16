namespace Spyder.Client.Images
{
    public class MockThumbnailImage : MockThumbnailImage<int>
    {
    }

    public class MockThumbnailImage<K> : ThumbnailImageBase<K, string>
    {
        public const string DefaultImageString = "Default Image";

        protected override string GetDefaultImage(ImageSize size)
        {
            return DefaultImageString;
        }
    }
}
