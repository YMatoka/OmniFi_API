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

builder.Services.AddScoped<ICryptoExchangeAccountRepository, CryptoExchangeAccountRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ICryptoApiCredentialRepository, CryptoApiCredentialRepository>();
builder.Services.AddScoped<IBankCredentialRepository, BankCredentialRepository>();
builder.Services.AddScoped<IFinancialAssetRepository, FinancialAssetRepository>();
builder.Services.AddScoped<IFinancialAssetHistoryRepository, FinancialAssetHistoryRepository>();

builder.Services.AddScoped<IStringEncryptionService, StringEncryptionService>();

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services.Configure<UserRepositoryOptions>(
    builder.Configuration.GetSection(UserRepositoryOptions.SectionName));

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
