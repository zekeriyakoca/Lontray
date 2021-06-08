using Catalog.API.Dtos;
using System.Threading.Tasks;

namespace Catalog.API.AppServices
{
    public interface ICatalogAppService
    {
        Task CreateCatalogItem(CreateCatalogItemDto dto);
        Task UpdateCatalogItem(UpdateCatalogItemDto dto);
    }
}
