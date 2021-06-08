using System.Threading.Tasks;

namespace Ordering.Domain.Common
{
    public interface IEventHandler<TParam>
    {
        Task Handle(TParam param);
    }
}
