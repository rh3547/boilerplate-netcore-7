namespace Nukleus.Application.Common.Validation
{
    public enum ErrorType
    {
        Validation,
        Conflict,
        Unauthorized,
        NotFound,
        Unknown, // Used for 'catch' blocks and such
        Generic,
        CaughtException,
        ExternalResourceRequestFailed
    }
    public readonly record struct Error
    {
        // Fields and Getters
        public readonly ErrorType ErrorType {get;}

        // Have the option to provide the entire exception or the error message
        public readonly Exception? Exception {get;}

        public readonly string Description {get;}

        /*
        Encapsulation of an exception to add metadata like ErrorType, 
        useful for translating to HTTP Result Codes in the presentation layer.
        For example: Validation Errors --> BadRequest, etc.
        */
        private Error(Exception e, ErrorType errorType){
            Exception = e;
            Description = e.Message;
            ErrorType = errorType;
        }

        private Error(string errorDescription, ErrorType errorType)
        {
            Description = errorDescription;
            ErrorType = errorType;
        }
        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.Unknown"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error UnknownError(
            string description = "An unknown error has occurred.") =>
            new(description, ErrorType.Unknown);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.Validation"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error ValidationError(
            string description = "A validation error has occurred.") =>
            new(description, ErrorType.Validation);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.Conflict"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error ConflictError(
            string description = "A conflict error has occurred.") =>
            new(description, ErrorType.Conflict);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.NotFound"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error NotFoundError(
            string description = "The expected resource could not be found.") =>
            new(description, ErrorType.NotFound);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.CaughtException"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error CaughtExceptionError(
            string description = "An exception was caught") =>
            new(description, ErrorType.CaughtException);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.Generic"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error GenericError(
            string description = "A generic error occured") =>
            new(description, ErrorType.Generic);

        /// <summary>
        /// Creates an <see cref="Error"/> of type <see cref="ErrorType.Generic"/> from a code and description.
        /// </summary>
        /// <param name="description">The error description.</param>
        public static Error Unauthorized(
            string description = "Unauthorized Operation.") =>
            new(description, ErrorType.Unauthorized);
    }
}