namespace Nukleus.API.Common.Validation
{
    // This class represents the IConfiguration settings JSON structure in the appsetttings.
    public class ValidationSettings
    {
        public const string SectionName = "ValidationSettings";

        // Should we respond with unsanitized validation messages to the client.
        public bool ShowUnsanitizedValidationMessages {get; init;}

        // Should we log unsanitized validation messages.
        public bool LogUnsanitizedValidationMessages { get; init; }
    }
}