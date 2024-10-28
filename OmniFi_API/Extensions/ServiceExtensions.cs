using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OmniFi_API.Comparers;
using OmniFi_API.Data;
using OmniFi_API.Factory.Interfaces;
using OmniFi_API.Factory;
using OmniFi_API.Mapping;
using OmniFi_API.Models.Api.Banks;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Encryption;
using OmniFi_API.Models.Identity;
using OmniFi_API.Options.Banks;
using OmniFi_API.Options.Cryptos;
using OmniFi_API.Options.Currencies;
using OmniFi_API.Options.Identity;
using OmniFi_API.Repository.Api.Banks;
using OmniFi_API.Repository.Assets;
using OmniFi_API.Repository.Banks;
using OmniFi_API.Repository.Cryptos;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Repository;
using System.Text;
using Microsoft.EntityFrameworkCore;
using OmniFi_API.Services.Interfaces;
using OmniFi_API.Services.Encryption;
using OmniFi_API.Services.Portfolio;
using OmniFi_API.Services.Api.Cryptos;
using OmniFi_API.Services.Api.Banks;
using OmniFi_API.Services.Api.Currencies;
using Microsoft.AspNetCore.Identity;

namespace OmniFi_API.Extensions
{
    public static class ServiceExtensions
    {
        const string DefaultSQlConnection = "DefaultSQLConnection";
        const string SecondSQlConnection = "SecondSQLConnection";
        const string SecretKeySection = "UserRepositoryOptions:SecretKey";

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            services.AddDbContext<ApplicationDbContext>(
                options => options
                .UseSqlServer(configuration.GetConnectionString(SecondSQlConnection))
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
            );

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRepository<CryptoExchange>, BaseRepository<CryptoExchange>>();
            services.AddScoped<IRepository<Bank>, BaseRepository<Bank>>();
            services.AddScoped<IRepository<AesKey>, BaseRepository<AesKey>>();
            services.AddScoped<IRepository<AesIV>, BaseRepository<AesIV>>();
            services.AddScoped<IRepository<AssetPlatform>, BaseRepository<AssetPlatform>>();
            services.AddScoped<IRepository<FiatCurrency>, BaseRepository<FiatCurrency>>();
            services.AddScoped<IRepository<AssetPlatform>, BaseRepository<AssetPlatform>>();
            services.AddScoped<IRepository<Bank>, BaseRepository<Bank>>();
            services.AddScoped<IRepository<CryptoExchange>, BaseRepository<CryptoExchange>>();
            services.AddScoped<IRepository<AssetSource>, BaseRepository<AssetSource>>();
            services.AddScoped<IRepository<CryptoHoldingHistory>, BaseRepository<CryptoHoldingHistory>>();
            services.AddScoped<IRepository<CryptoCurrency>, BaseRepository<CryptoCurrency>>();
            services.AddScoped<IRepository<BankSubAccount>, BaseRepository<BankSubAccount>>();
            services.AddScoped<IRepository<BankAgreement>, BaseRepository<BankAgreement>>();

            services.AddScoped<IBankDataApiRepository, BankDataApiCredentialRepository>();
            services.AddScoped<ICryptoExchangeAccountRepository, CryptoExchangeAccountRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<ICryptoApiCredentialRepository, CryptoApiCredentialRepository>();
            services.AddScoped<IBankCredentialRepository, BankCredentialRepository>();
            services.AddScoped<IFinancialAssetRepository, FinancialAssetRepository>();
            services.AddScoped<IFinancialAssetHistoryRepository, FinancialAssetHistoryRepository>();
            services.AddScoped<ICryptoHoldingRepository, CryptoHoldingRepository>();

            services.AddScoped<IEqualityComparer<BankSubAccount>, BankSubAccountComparer>();

            services.AddScoped<IStringEncryptionService, StringEncryptionService>();
            services.AddScoped<IFetchPortfolioService, PortfolioService>();

            services.AddHttpClient<BinanceService>();
            services.AddScoped<BinanceService>();

            services.AddHttpClient<CryptoDotComService>();
            services.AddScoped<CryptoDotComService>();

            services.AddHttpClient<KrakenService>();
            services.AddScoped<KrakenService>();

            services.AddHttpClient<BankInfoService>();
            services.AddScoped<BankInfoService>();

            services.AddHttpClient<IBankInfoService, BankInfoService>();
            services.AddScoped<IBankInfoService, BankInfoService>();

            services.AddHttpClient<BankInfoService>();
            services.AddScoped<BankInfoService>();

            services.AddHttpClient<IFiatCurrencyService, FiatCurrencyService>();
            services.AddScoped<IFiatCurrencyService, FiatCurrencyService>();

            services.AddHttpClient<ICryptoInfoService, CryptoInfoService>();
            services.AddScoped<ICryptoInfoService, CryptoInfoService>();

            services.AddHttpClient<BinanceService>();
            services.AddScoped<BinanceService>();

            services.AddScoped<IFinancialAssetServiceFactory, FinancialAssetServiceFactory>();

            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(MappingConfig));

            services.Configure<UserRepositoryOptions>(
                configuration.GetSection(UserRepositoryOptions.SectionName));

            services.Configure<FiatCurrencyServiceOptions>(
                configuration.GetSection(FiatCurrencyServiceOptions.SectionName));

            services.Configure<CryptoInfoServiceOptions>(
                configuration.GetSection(CryptoInfoServiceOptions.SectionName));

            services.Configure<BankInfoServiceOptions>(configuration.GetSection(
                BankInfoServiceOptions.SectionName));

            services.Configure<GocardlessBankInfoOptions>(configuration.GetSection(
                GocardlessBankInfoOptions.SectionName));

            var secretKey = configuration.GetValue<string>(SecretKeySection);


            services.AddAuthentication(x =>
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

            services.AddSwaggerGen(options =>
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

            return services;
        }
    }
}
