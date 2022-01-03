using Catalog.API.Dtos;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Catalog.API.AppServices
{
    public interface ICatalogAppService
    {
        Task CreateCatalogItem(CreateCatalogItemDto dto);
        Task<CatalogItemDto> GetCatalogItem([NotNull] int id);
        Task<bool> RemoveCatalogItem([NotNull] int id);
        Task UpdateCatalogItem(UpdateCatalogItemDto dto);
    }
}
