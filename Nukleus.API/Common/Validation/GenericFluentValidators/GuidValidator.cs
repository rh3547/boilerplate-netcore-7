using Nukleus.Application.Common.Validation;
using FluentValidation;

namespace Nukleus.API.Common.GenericFluentValidators
{
    public class GuidValidator : AbstractValidator<Guid>
    {
        public GuidValidator()
        {
            RuleFor(x => x).NotEmpty();
        }
    }
}