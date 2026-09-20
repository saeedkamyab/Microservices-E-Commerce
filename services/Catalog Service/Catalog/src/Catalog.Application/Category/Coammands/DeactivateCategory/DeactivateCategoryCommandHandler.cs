using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Category.Coammands.DeactivateCategory;

public sealed class DeactivateCategoryCommandHandler : IRequestHandler<DeactivateCategoryCommand>
{
    private readonly ICategoryRepository _categorytRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateCategoryCommandHandler(
        ICategoryRepository categorytRepository,
        IUnitOfWork unitOfWork)
    {
        _categorytRepository = categorytRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeactivateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categorytRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with ID {request.CategoryId} not found.");
        }
        category.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
