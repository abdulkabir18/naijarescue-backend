using Application.Extensions;
using Host.Extensions;
using Infrastructure.Extensions;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApiVersioningWithExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddDbConnection(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddSecurity();
builder.Services.AddEmailService(builder.Configuration);

builder.Services.Configure<PasswordHasherSettings>(builder.Configuration.GetSection("PasswordHasher"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Brevo"));

var app = builder.Build();

await app.Services.SeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NaijaRescue API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseAuthorization();

app.MapControllers();

app.Run();
