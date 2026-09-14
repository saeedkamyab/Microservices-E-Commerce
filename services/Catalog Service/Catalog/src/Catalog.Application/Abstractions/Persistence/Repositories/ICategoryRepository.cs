using Catalog.Domain.Entities;

namespace Catalog.Application.Abstractions.Persistence.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken);

    Task AddAsync(
      Category category,
      CancellationToken cancellationToken);
}
