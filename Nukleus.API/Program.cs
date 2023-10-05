using Mapster;
using Nukleus.API;
using Nukleus.API.Middleware;
using Nukleus.Application;
using Nukleus.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container. Also for DI
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration); // This is why we have a layer reference to infrastructure.
    builder.Services.AddPresentationServices(builder.Configuration);

    builder.Services.AddControllers(options =>
    {
        options.ModelValidatorProviders.Clear(); // Disables the handler, but does not disable validation.
    });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    // Add all of our custom middleware to the request pipeline.
    app.RegisterCustomMiddleware();

    app.Run();
}
// https://stackoverflow.com/questions/38238043/how-and-where-to-call-database-ensurecreated-and-database-migrate
// public static void EnsureDatabaseCreated()
// {
//     var optionsBuilder = new DbContextOptionsBuilder();
//     if (HostingEnvironment.IsDevelopment()) optionsBuilder.UseSqlServer(Configuration["Data:dev:DataContext"]);
//     else if (HostingEnvironment.IsStaging()) optionsBuilder.UseSqlServer(Configuration["Data:staging:DataContext"]);
//     else if (HostingEnvironment.IsProduction()) optionsBuilder.UseSqlServer(Configuration["Data:live:DataContext"]);
//     var context = new ApplicationContext(optionsBuilder.Options);
//     context.Database.EnsureCreated();

//     optionsBuilder = new DbContextOptionsBuilder();
//     if (HostingEnvironment.IsDevelopment()) optionsBuilder.UseSqlServer(Configuration["Data:dev:TransientContext"]);
//     else if (HostingEnvironment.IsStaging()) optionsBuilder.UseSqlServer(Configuration["Data:staging:TransientContext"]);
//     else if (HostingEnvironment.IsProduction()) optionsBuilder.UseSqlServer(Configuration["Data:live:TransientContext"]);
//     new TransientContext(optionsBuilder.Options).Database.EnsureCreated();
// }