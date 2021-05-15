using Catalog.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.AppServices
{
    public interface ICatalogAppService
    {
        Task CreateCatalogItem(CreateCatalogItemDto dto);
        Task UpdateCatalogItem(UpdateCatalogItemDto dto);
    }
}
