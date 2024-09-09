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

const string DefaultSQlConnection = "DefaultSQLConnection";
const string SecondSQlConnection = "SecondSQLConnection";
const string SecretKeySection = "UserRepositoryOptions:SecretKey";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString(DefaultSQlConnection))
    .EnableSensitiveDataLogging()
    .EnableDetailedErrors()
    );

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IRepository<CryptoExchange>, BaseRepository<CryptoExchange>>();
builder.Services.AddScoped<IRepository<Bank>, BaseRepository<Bank>>();
builder.Services.AddScoped<IRepository<AesKey>, BaseRepository<AesKey>>();
builder.Services.AddScoped<IRepository<AesIV>, BaseRepository<AesIV>>();
builder.Services.AddScoped<IRepository<AssetPlatform>, BaseRepository<AssetPlatform>>();
builder.Services.AddScoped<IRepository<FiatCurrency>, BaseRepository<FiatCurrency>>();
builder.Services.AddScoped<IRepository<AssetPlatform>, BaseRepository<AssetPlatform>>();
builder.Services.AddScoped<IRepository<Bank>, BaseRepository<Bank>>();
builder.Services.AddScoped<IRepository<CryptoExchange>, BaseRepository<CryptoExchange>>();
builder.Services.AddScoped<IRepository<AssetSource>, BaseRepository<AssetSource>>();
builder.Services.AddScoped<IRepository<CryptoHoldingHistory>, BaseRepository<CryptoHoldingHistory>>();
builder.Services.AddScoped<IRepository<CryptoCurrency>, BaseRepository<CryptoCurrency>>();
builder.Services.AddScoped<IRepository<BankSubAccount>, BaseRepository<BankSubAccount>>();
builder.Services.AddScoped<IRepository<BankAgreement>, BaseRepository<BankAgreement>>();

builder.Services.AddScoped<IBankDataApiRepository, BankDataApiCredentialRepository>();
builder.Services.AddScoped<ICryptoExchangeAccountRepository, CryptoExchangeAccountRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ICryptoApiCredentialRepository, CryptoApiCredentialRepository>();
builder.Services.AddScoped<IBankCredentialRepository, BankCredentialRepository>();
builder.Services.AddScoped<IFinancialAssetRepository, FinancialAssetRepository>();
builder.Services.AddScoped<IFinancialAssetHistoryRepository, FinancialAssetHistoryRepository>();
builder.Services.AddScoped<ICryptoHoldingRepository, CryptoHoldingRepository>();

builder.Services.AddScoped<IStringEncryptionService, StringEncryptionService>();
builder.Services.AddScoped<IFetchPortfolioService, PortfolioService>();


builder.Services.AddHttpClient<IFinancialAssetService, BinanceService>();
builder.Services.AddScoped<IFinancialAssetService, BinanceService>();

builder.Services.AddHttpClient<IFiatCurrencyService, FiatCurrencyService>();
builder.Services.AddScoped<IFiatCurrencyService, FiatCurrencyService>();

builder.Services.AddHttpClient<ICryptoInfoService, CryptoInfoService>();
builder.Services.AddScoped<ICryptoInfoService, CryptoInfoService>();

builder.Services.AddHttpClient<CryptoDotComService>();
builder.Services.AddScoped<CryptoDotComService>();

builder.Services.AddHttpClient<KrakenService>();
builder.Services.AddScoped<KrakenService>();

builder.Services.AddHttpClient<BinanceService>();
builder.Services.AddScoped<BinanceService>();

builder.Services.AddScoped<IFinancialAssetServiceFactory, FinancialAssetServiceFactory>();



builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.Configure<UserRepositoryOptions>(
    builder.Configuration.GetSection(UserRepositoryOptions.SectionName));

builder.Services.Configure<FiatCurrencyServiceOptions>(
    builder.Configuration.GetSection(FiatCurrencyServiceOptions.SectionName));

builder.Services.Configure<CryptoInfoServiceOptions>(
    builder.Configuration.GetSection(CryptoInfoServiceOptions.SectionName));

builder.Services.Configure<BankInfoServiceOptions>(builder.Configuration.GetSection(
    BankInfoServiceOptions.SectionName));

var secretKey = builder.Configuration.GetValue<string>(SecretKeySection);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey!)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
                      "Enter Bearer [space] and then your token in the text input below \r\n\r\n " +
                      "Example: \"Bearer 1234abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme()
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },

                Scheme = "oauth2",
                Name= "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    }
    );
});

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
