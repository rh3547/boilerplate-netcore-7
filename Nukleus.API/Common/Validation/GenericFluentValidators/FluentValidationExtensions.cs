using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Nukleus.API.Common
{
    public static class FluentValidationExtensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        // TODO Under DEV
        // public static IRuleBuilderOptions<T, Guid> GUIDNotDefault<T, TElement>(this IRuleBuilder<T, Guid> ruleBuilder)
        // {
        //     return ruleBuilder.Must(g => g != default).WithMessage("Guid must not be default.");
        // }

        //https://stackoverflow.com/questions/35871741/how-to-use-reflection-in-fluentvalidation
        
    }
}