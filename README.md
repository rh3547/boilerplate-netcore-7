# .NET 7 Nukleus Backend Boilerplate
Boilerplate for API Projects using .NET CORE 7.
- Using FluentValidation, Serilog, EF Core.

## Setup and Run:
Follow detailed instructions here: https://thaledev.atlassian.net/wiki/spaces/Administra/pages/9371649/Backend+Boilerplate+Getting+Started

## Architecture Overview:
https://thaledev.atlassian.net/wiki/spaces/Administra/pages/4554753/Backend+Architecture+Overview+-+.NET+CORE+7+DDD+CA

## In Progress:
- TODO Fix Global Error Handling (Example: Passing malformed GUID causes explosions)
- TODO implement layer-specific known-error handling (Try/Catch? Exception Throwing (not performant), Result<>, ErrorOR<>
- TODO Abstract Away payload-return of FluentValidation Validate() results from Controller.
- TODO use discriminated-union as opposed to exception throwing (ErrorOR<>, Result<>, etc)
- TODO add IConfiguration for ErrorHandling for differnet environments (same way I did ValidationActionFilter)
- TODO implement examples for more advanced service transactions
