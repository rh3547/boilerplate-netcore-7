using System.Reflection;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Nukleus.API.Common.BaseController.Attributes
{
    public class RolePolicyAttribute : Attribute
    {
        public string PolicyName { get; set; }

        public RolePolicyAttribute(string policyName)
        {
            PolicyName = policyName;
        }
    }

    public static class RolePolicyHelper
    {
        // public static async Task SeedPolicies(IServiceProvider serviceProvider)
        // {
        //     var policyService = serviceProvider.GetRequiredService<IPolicyService>(); // Replace IPolicyService with the interface you use to interact with your database for storing policies

        //     var controllerActions = GetNukleusControllerActions();

        //     foreach (var (controllerType, actionMethod) in controllerActions)
        //     {
        //         var policyName = GeneratePolicyName(controllerType, actionMethod);
        //         // Check if the policy already exists in the database and add it if not
        //         if (!await policyService.PolicyExistsAsync(policyName))
        //         {
        //             await policyService.AddPolicyAsync(policyName);
        //         }
        //     }
        // }

        public static string GeneratePolicyName(Type controllerType, MethodInfo actionMethod)
        {
            var controllerName = controllerType.Name.Replace("Controller", "");
            var actionName = actionMethod.Name;

            return $"{controllerName}.{actionName}";
        }

        public static List<(Type ControllerType, MethodInfo ActionMethod)> GetNukleusControllerActions()
        {
            var nukleusControllerType = typeof(NukleusController);
            var assembly = Assembly.GetExecutingAssembly();

            var controllerTypes = assembly.GetTypes()
                .Where(type => type.IsSubclassOf(nukleusControllerType));

            var controllerActions = new List<(Type ControllerType, MethodInfo ActionMethod)>();

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => m.DeclaringType == controllerType && m.IsDefined(typeof(HttpMethodAttribute), false));

                foreach (var method in methods)
                {
                    controllerActions.Add((controllerType, method));
                }
            }

            return controllerActions;
        }

    }
}