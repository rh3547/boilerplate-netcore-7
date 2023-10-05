using System.Diagnostics.Contracts;

namespace Nukleus.Application.Common.Validation
{
    // https://www.moesif.com/blog/technical/api-design/Which-HTTP-Status-Code-To-Use-For-Every-CRUD-App/
    // Types are used for status codes mapping, Success message is used for client-facing.
    public enum OperationType
    {
        CreateResource, // Create Domain Event - Translated to 201
        RetrieveResource, // Read Domain Event - Translated to 200
        PagedRetrieveResource, // Read Partial Domain Event - Translated to 206
        UpdateResource, // Update Domain Event - Translated to 200, 202, or 204
        UpdateResourceNoContent, // Update No Content (ex. Save Changes) - Translated to 204
        DeleteResource,
        AcceptedEventualRequest,

        GenericCompleted
    }
    public readonly record struct Success<TValue>
    {
        // Fields and Getters
        public readonly TValue Value {get;}
        public readonly OperationType OperationType { get; }

        public readonly string Description { get; }

        [Pure]
        public static implicit operator Success<TValue>(TValue value) => new Success<TValue>(value);

        private Success(TValue value, string description = "Operation completed succesfully.", OperationType type = OperationType.GenericCompleted)
        {
            Value = value;   
            Description = description;
            OperationType = type;
        }

        public static Success<TValue> CreateResourceSuccess(
            TValue value,
            string description = "Resource created succesfully.",
            OperationType type = OperationType.CreateResource) =>
            new(value, description, type);
    }
}