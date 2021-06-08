using System;

namespace EventBus.Dtos
{
    public class BaseQueueItemDto
    {
        public BaseQueueItemDto()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
        public BaseQueueItemDto(Guid id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
