using System;
using System.ComponentModel.DataAnnotations;

namespace DomainHelper.Entities
{
    public class Entity : IEntity
    {
        [Key]
        public int Id { get; set; }
    }

    public class EntityAudited : Entity, IAudited
    {
        public int? LastModifierUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public int? LastCreaterUserId { get; set; }
        public DateTime? LastCreationTime { get; set; }
    }
}
