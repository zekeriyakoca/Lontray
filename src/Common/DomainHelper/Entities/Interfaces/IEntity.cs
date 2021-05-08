using System;

namespace DomainHelper.Entities
{
    public interface IEntity
    {
        public int Id { get; set; }
    }

    public interface IHasCreationTime
    {
        DateTime? LastCreationTime { get; set; }
    }

    public interface ICreationAudited : IHasCreationTime
    {
        int? LastCreaterUserId { get; set; }
    }
    public interface IHasModificationTime
    {
        DateTime? LastModificationTime { get; set; }
    }

    public interface IModificationAudited : IHasModificationTime
    {
        int? LastModifierUserId { get; set; }
    }
    public interface IAudited : ICreationAudited, IModificationAudited
    {

    }
}
