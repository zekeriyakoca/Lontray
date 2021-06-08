using AutoMapper;
using Catalog.API.Dtos;
using Catalog.API.Entities;
using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.IntegrationEvents.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.AppServices
{
    public class CatalogAppService : ICatalogAppService
    {
        private readonly IMapper mapper;
        private readonly CatalogContext context;
        private readonly ICatalogIntegrationService integrationService;

        public CatalogAppService(IMapper mapper, CatalogContext context, ICatalogIntegrationService integrationService)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.integrationService = integrationService;
        }
        public async Task CreateCatalogItem(CreateCatalogItemDto dto)
        {
            var itemToCreate = mapper.Map<CatalogItem>(dto);
            if (itemToCreate == null)
                throw new Exception($"{typeof(CreateCatalogItemDto).Name} is null or cannot be converted to CatalogItem");

            context.CatalogItems.Add(itemToCreate);
            await context.SaveChangesAsync();

            // Not likely to need the following event.
            //var eventToPublish = mapper.Map<CatalogItemCreatedEvent>(itemToCreate);
            //integrationService.PublishEvent(eventToPublish);
        }
        public async Task UpdateCatalogItem(UpdateCatalogItemDto dto)
        {
            var itemToUpdate = context.CatalogItems.SingleOrDefault(c => c.Id == dto.Id);
            if (itemToUpdate == null)
                throw new Exception($"CatalogItem cannot be found to update. CatalogItemId :{dto.Id}");

            itemToUpdate = mapper.Map<UpdateCatalogItemDto, CatalogItem>(dto, itemToUpdate);
            await context.SaveChangesAsync();

            var eventToPublish = mapper.Map<CatalogItemUpdatedEvent>(itemToUpdate);
            integrationService.PublishEvent(eventToPublish);
        }

    }
}
