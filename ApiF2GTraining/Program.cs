using ApiF2GTraining.Data;
using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Reflection.Metadata;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.Azure;
using Azure.Security.KeyVault.Secrets;
using ApiF2GTrainingAmazon.Helpers;

var builder = WebApplication.CreateBuilder(args);

string connectionString = await HelperSecretManager.GetSecretAsync("MySqlF2G");
builder.Services.AddDbContext<F2GDataBaseContext>(options => options.UseMySql(connectionString , ServerVersion.AutoDetect(connectionString)));
builder.Services.AddTransient<IRepositoryF2GTraining, RepositoryF2GTraining>();

builder.Services.AddSingleton<HelperOAuthToken>();
HelperOAuthToken helper = new HelperOAuthToken(builder.Configuration);
builder.Services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(document => {

    document.Title = "Api OAuth F2G Training";
    document.Description = "Api de F2G Training";
    
    document.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
    });

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

builder.Services.AddCors(p => p.AddPolicy("corsenabled", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.InjectStylesheet("/css/bootstrap.css");
    options.InjectStylesheet("/css/material3x.css");
    options.SwaggerEndpoint(
        url: "/swagger/v1/swagger.json", name: "Api v1");
    options.RoutePrefix = "";
    options.DocExpansion(DocExpansion.None);
});

app.UseCors("corsenabled");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
