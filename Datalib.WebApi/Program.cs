using System.Diagnostics;
using System.Reflection;
using Datalib.WebApi;
using Datalib.WebApi.Data;
using Datalib.WebApi.Data.Implementation;
using Datalib.WebApi.Data.Seed;
using Datalib.WebApi.Domain.Models;
using Datalib.WebApi.Services;
using Datalib.WebApi.Services.Implementation;
using Datalib.WebApi.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DatalibDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUnitOfWork, DatalibDbContext>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, FakeEmailService>();
builder.Services.AddHostedService<ReminderWorker>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Allow Swagger to pull description from xml documentation. See also *.csproj.
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<ISeed<Book>, BookSeed>();
    builder.Services.AddScoped<ISeed<User>, UserSeed>();
    builder.Services.AddScoped<ISeed<Checkout>, CheckoutSeed>();
}
else
{
    builder.Services.AddScoped<ISeed<Book>, NoSeed<Book>>();
    builder.Services.AddScoped<ISeed<User>, NoSeed<User>>();
    builder.Services.AddScoped<ISeed<Checkout>, NoSeed<Checkout>>();
}
var app = builder.Build();

InitDb(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static void InitDb(IHost app)
{
    using var scope = app.Services.CreateScope();
    
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var dbContext = serviceProvider.GetRequiredService<DatalibDbContext>();
        DbInitializer.Initialize(dbContext, serviceProvider);
    }
    catch (Exception e)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "Error occurred initializing the DB.");
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
    }
}