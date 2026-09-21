using Catalog.Application.Abstractions.Persistence;
using Catalog.Application.Abstractions.Persistence.Repositories;
using Catalog.Application.Exceptions;
using Catalog.Domain.Entities;
using Catalog.Domain.ValueObjects;
using MediatR;

namespace Catalog.Application.Category.Coammands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(
        ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with id {request.CategoryId} not found.");
        }
       
        category.Rename(CategoryName.Create(request.Name));

        if (category.ParentCategoryId != request.ParentCategoryId)
        {
            if (request.ParentCategoryId.HasValue)
            {
                var parentId = request.ParentCategoryId.Value;

                var parentCategory = await _categoryRepository.GetByIdAsync(
                    parentId, cancellationToken);

                if (parentCategory is null)
                {
                    throw new NotFoundException("Parent category was not found.");
                }

                var wouldCreateCycle =
           await _categoryRepository.WouldCreateCycleAsync(
               category.Id,
               parentId,
               cancellationToken);


                if (wouldCreateCycle)
                {
                    throw new InvalidOperationException(
                        "A category cannot be moved under one of its descendants.");
                }
            }
        }
        category.ChangeParentCategory(request.ParentCategoryId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
