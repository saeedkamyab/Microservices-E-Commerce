using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using MediatR;

namespace Catalog.Application.Category.Coammands.ActivateCategory;

public sealed class ActivateCategoryCommandHandler : IRequestHandler<ActivateCategoryCommand>
{
    private readonly ICategoryRepository _categorytRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateCategoryCommandHandler(
        ICategoryRepository categorytRepository,
        IUnitOfWork unitOfWork)
    {
        _categorytRepository = categorytRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ActivateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categorytRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with ID {request.CategoryId} not found.");
        }
        category.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
