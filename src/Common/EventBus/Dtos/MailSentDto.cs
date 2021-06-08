namespace EventBus.Dtos
{
    public class MailSentDto : BaseQueueItemDto
    {
        public string message { get; set; }
    }
}
