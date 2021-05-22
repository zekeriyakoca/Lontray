using System.Threading.Tasks;

namespace Ordering.Domain.Common
{
    public interface IHandler<T> where T : IDomainEvent
    {
        Task Handle(T eventParams);
    }
}
