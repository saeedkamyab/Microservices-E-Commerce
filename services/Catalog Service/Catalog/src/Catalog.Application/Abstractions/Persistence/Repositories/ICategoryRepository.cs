

namespace Catalog.Application.Abstractions.Persistence.Repositories;

public interface ICategoryRepository
{
    Task<Domain.Entities.Category?> GetByIdAsync(
       Guid id,
       CancellationToken cancellationToken);

    Task AddAsync(
      Domain.Entities.Category category,
      CancellationToken cancellationToken);
}
