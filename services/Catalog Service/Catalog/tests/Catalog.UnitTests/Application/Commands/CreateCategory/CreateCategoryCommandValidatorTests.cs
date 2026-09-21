using Catalog.Application.Category.Coammands.CreateCategory;
using Catalog.Domain.Enums;

namespace Catalog.UnitTests.Application.Commands.CreateCategory
{
    public class CreateCategoryCommandValidatorTests
    {
        private readonly CreateCategoryCommandValidator _validator = new();

        [Fact]
        public void Validate_WithValidCommand_ReturnsValid()
        {
            var command = new CreateCategoryCommand(
                "Electronics",
                null
                );

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WithEmptyName_ReturnsValidationErrorForName()
        {
            var command = new CreateCategoryCommand(
                string.Empty,
                null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
        }

        [Fact]
        public void Validate_WithEmptyAttributes_ReturnsValidationErrorForAttributes()
        {
            var command = new CreateCategoryCommand(
                "Books",
                null
               );

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Attributes" && e.ErrorMessage == "Attributes are required.");
        }
        [Fact]
        public void Validate_WithWhitespaceName_ReturnsValidationErrorForName()
        {
            var command = new CreateCategoryCommand(
                "   ",
                null
               );

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Errors,
                e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_WithNullAttributes_ReturnsValidationErrorForAttributes()
        {
            var command = new CreateCategoryCommand(
                "Electronics",
                null
               );

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Errors,
                e => e.PropertyName == "Attributes");
        }

        [Fact]
        public void Validate_WithEmptyAttributeName_ReturnsValidationErrorForAttributeName()
        {
            var command = new CreateCategoryCommand(
                "Electronics",
                null
             );

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Errors,
                e => e.PropertyName.Contains("Name"));
        }

        [Fact]
        public void Validate_WithNullAttributeOptions_ReturnsValidationErrorForOptions()
        {
            var command = new CreateCategoryCommand(
                "Electronics",
                null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);

            Assert.Contains(
                result.Errors,
                e => e.PropertyName.Contains("Options"));
        }
    }
}
