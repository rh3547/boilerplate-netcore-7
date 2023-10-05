using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Nukleus.API.Common.Helpers
{
    public class StringArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var stringValue = valueProviderResult.FirstValue;
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stringArray = JsonSerializer.Deserialize<string[]>(stringValue, options);
                bindingContext.Result = ModelBindingResult.Success(stringArray);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
