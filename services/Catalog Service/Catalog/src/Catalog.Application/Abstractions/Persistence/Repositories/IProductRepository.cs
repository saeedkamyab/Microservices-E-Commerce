using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions.Persistence.Repositories;

public interface IProductRepository
{
    Task<Catalog.Domain.Entities.Product?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken);

    Task<Catalog.Domain.Entities.Product?> GetByIdWithSpecificationsAsync(
       Guid id,
       CancellationToken cancellationToken);


    Task<bool> AnyProductUsesAttributeDefinitionAsync(
    Guid attributeDefinitionId,
    CancellationToken cancellationToken);

    Task AddAsync(
        Catalog.Domain.Entities.Product product,
        CancellationToken cancellationToken);

    Task SyncSpecificationsAsync(
    Domain.Entities.Product product,
    CancellationToken cancellationToken);
}
