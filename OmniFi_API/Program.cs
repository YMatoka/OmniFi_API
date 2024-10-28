using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniFi_API.Data;
using OmniFi_API.Data.Interfaces;
using OmniFi_API.Mapping;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;
using OmniFi_API.Options;
using OmniFi_API.Options.Identity;
using OmniFi_API.Repository;
using OmniFi_API.Repository.Banks;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Encryption;
using OmniFi_API.Services.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OmniFi_API.Repository.Assets;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Currencies;
using Microsoft.Extensions.Logging;
using OmniFi_API.Services.Portfolio;
using OmniFi_API.Options.Currencies;

using OmniFi_API.Services.Api.Cryptos;
using OmniFi_API.Services.Api.Currencies;
using OmniFi_API.Options.Cryptos;
using OmniFi_API.Utilities;
using OmniFi_API.Factory.Interfaces;
using OmniFi_API.Factory;
using OmniFi_API.Options.Banks;
using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Repository.Api.Banks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OmniFi_API.Services.Api.Banks;
using OmniFi_API.Comparers;
using System.Globalization;
using OmniFi_API.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddServices(builder.Configuration);

// adding service log at host level
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

// Set invariant culture for the whole project, for each thread
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
