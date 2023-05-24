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

var builder = WebApplication.CreateBuilder(args);

//CAMBIAR ESTO POR CONFIGURACION CON EL SINONIMO DE KEYVAULT EN AMAZON
//SE GUARDARA LA CONFIGURACION DE LA CADENA DE CONEXION DE AMAZON RDS, DENTRO DEL SINONIMO DE KEYVAULT EN AMAZON

/*builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync("SqlAzureF2G");

string connectionString = keyVaultSecret.Value;
builder.Services.AddDbContext<F2GDataBaseContext>(options => options.UseSqlServer(connectionString));*/

/*string connectionString = builder.Configuration.GetConnectionString("MySqlF2G");*/
builder.Services.AddDbContext<F2GDataBaseContext>(options => options.UseMySql(connectionString , ServerVersion.AutoDetect(connectionString)));

//FIN DE CAMBIO DE SINONIMO DE KEYVAULT EN AMAZON

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

app.UseCors("corsapp");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
