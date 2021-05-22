namespace EventBus.Dtos
{
    public class ImageToProcessDto : BaseQueueItemDto
    {

        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string CacheKey { get; set; }
        public string ImageFileName { get; set; }
        public string MailAddressToSend { get; set; }
        public string CleanKey
        {
            get
            {
                return Id.ToString().Replace("-", "");
            }
        }
    }
}
