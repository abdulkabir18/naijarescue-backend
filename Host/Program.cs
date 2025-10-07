using Application.Extensions;
using Host.Extensions;
using Host.Hubs;
using Infrastructure.Extensions;
using Infrastructure.Middlewares;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCorsPolicy();
builder.Services.AddApiVersioningWithExplorer();
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddDbConnection(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddSecurity();
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddStorageService(builder.Environment.WebRootPath);

builder.Services.Configure<PasswordHasherSettings>(builder.Configuration.GetSection("PasswordHasher"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Brevo"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;
    await scopedProvider.SeedDatabaseAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NaijaRescue API v1");
    });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<AuditMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/notifications");

app.UseCors("AllowFrontend");

app.Run();