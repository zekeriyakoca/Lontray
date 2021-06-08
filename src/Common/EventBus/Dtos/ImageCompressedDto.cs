namespace EventBus.Dtos
{
    public class ImageCompressedDto : BaseQueueItemDto
    {
        public int ImageId { get; set; }
        public string CacheKey { get; set; }
        public string CompressedLargeImageUrl { get; set; }
        public string CompressedMediumImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
    }
}
