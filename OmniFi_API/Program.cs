using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniFi_API.Data;
using OmniFi_API.Data.Interfaces;
using OmniFi_API.Mapping;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;
using OmniFi_API.Options;
using OmniFi_API.Options.Cryptos;
using OmniFi_API.Options.Encryption;
using OmniFi_API.Options.Identity;
using OmniFi_API.Repository;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Services.Encryption;
using OmniFi_API.Services.Interfaces;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"))
    );

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRepository<CryptoExchange>, Repository<CryptoExchange>>();
builder.Services.AddScoped<ICryptoExchangeAccountRepository, CryptoExchangeAccountRepository>();
builder.Services.AddScoped<ICryptoApiCredentialRepository, CryptoApiCredentialRepository>();
builder.Services.AddScoped<IRepository<AesKey>, Repository<AesKey>>();

builder.Services.AddScoped<IStringEncryptionService, StringEncryptionService>();

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.Configure<UserRepositoryOptions>(
    builder.Configuration.GetSection(UserRepositoryOptions.SectionName));

builder.Services.Configure<StringEncryptionServiceOptions>(
    builder.Configuration.GetSection(StringEncryptionServiceOptions.SectionName));

builder.Services.Configure<CryptoApiCredentialOptions>(
    builder.Configuration.GetSection(CryptoApiCredentialOptions.SectionName));


var app = builder.Build();

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
