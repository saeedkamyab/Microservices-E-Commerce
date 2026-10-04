

namespace Catalog.Application.Abstractions.Persistence.Repositories;

public interface ICategoryRepository
{
    Task<Domain.Entities.Category?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken);

    Task<Domain.Entities.Category?> GetWithAttributeDefinitionsAsync(
   Guid id,
   CancellationToken cancellationToken);


    Task<bool> WouldCreateCycleAsync(
    Guid categoryId,
    Guid newParentId,
    CancellationToken cancellationToken);


    Task<bool> AttributeDefinitionNameExistsAsync(
    Guid categoryId,
    string name,
    CancellationToken cancellationToken);


    Task AddAsync(
      Domain.Entities.Category category,
      CancellationToken cancellationToken);
    Task AddCategoryAttributeDefinitionAsync(
      Domain.Entities.Category category,
      CancellationToken cancellationToken);

    Task SyncRemovedAttributeDefinitionsAsync(
Domain.Entities.Category category,
CancellationToken cancellationToken);

}
