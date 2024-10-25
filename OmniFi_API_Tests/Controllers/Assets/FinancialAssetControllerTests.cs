using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using NUnit.Framework;
using OmniFi_API.Controllers.Assets;
using OmniFi_API.Data;
using OmniFi_API.Models.Assets;
using OmniFi_API.Repository.Interfaces;
using OmniFi_DTOs.Dtos.Api;
using OmniFi_DTOs.Dtos.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace OmniFi_API_Tests.Controllers.Assets
{
    [TestFixture]
    public class FinancialAssetControllerTests
    {
        private Mock<ApplicationDbContext>? _db;
        private Mock<IFinancialAssetRepository>? _financialAssetRepository;
        private Mock<IUserRepository>? _userRepository;
        private Mock<IRepository<AssetPlatform>>? _assetPlatform;
        private Mock<IMapper>? _mapper;
        //private Mock<ApiResponse> _apiResponse;
        private FinancialAssetController? _ct;

        [SetUp]
        public void Setup()
        {
            _db = new Mock<ApplicationDbContext>();
            _mapper = new Mock<IMapper>();
            _financialAssetRepository = new Mock<IFinancialAssetRepository>();
            _userRepository = new Mock<IUserRepository>();
            _assetPlatform = new Mock<IRepository<AssetPlatform>>();
            //_apiResponse = new Mock<ApiResponse>();

            _ct = new FinancialAssetController(
                _financialAssetRepository.Object,
                _mapper.Object,
                _userRepository.Object,
                _assetPlatform.Object);

            //_apiResponse = new Mock<ApiResponse>();
        }

        [Test]
        public async Task GetFinanicalAssets_ShallReturnOkApiResponse_IfUserNameExists()
        {

            int statusCodeOk = 200;

            // Arrange
            var userName = "ghost";
            _userRepository
                .Setup(x => x.GetUserAsync(It.IsAny<string>(), It.IsAny<bool>()).Result)
                .Returns(new OmniFi_API.Models.Identity.ApplicationUser()
                {
                    FiatCurrency = new OmniFi_API.Models.Currencies.FiatCurrency() 
                    { CurrencyCode="EUR", CurrencyName="EURO", CurrencySymbol = ""},
                    FirstName = "Yann",
                    LastName = "MATOKA",
                    UserName = userName
                });

            _financialAssetRepository
                .Setup(x => x.GetAllWithEntitiesAsync(
                    It.IsAny<Expression<Func<FinancialAsset, bool>>>(),
                    It.IsAny<bool>()).Result)
                .Returns(GetFinancialAssets());

            _mapper
                .Setup(x => x.Map<FinancialAssetDTO>(It.IsAny<FinancialAsset>()))
                .Returns(new FinancialAssetDTO()
                {
                    AssetPlatformName = "Test",
                    Amount = 100,
                    AssetSourceName = "Test",
                    FiatCurrencyCode = "EUR",
                    UserName = "test"
                });

            // Act
            var result = await _ct!.GetFinancialAssets(userName);

            var castedResult = (OkObjectResult?)result.Result;

            // Assert
            Assert.That(castedResult, Is.Not.Null);
            Assert.That(castedResult?.StatusCode, Is.EqualTo(statusCodeOk));

        }

        public static FinancialAsset[] GetFinancialAssets()
        {
            return new FinancialAsset[]
            {
                new FinancialAsset()
                {
                    FiatCurrencyID = 1,
                    AssetPlatformID = 1,
                    FinancialEntityId = 1,
                    UserID = "User1",
                    AssetSourceID = 1,
                    Amount = 150,
                    FirstRetrievedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                },
                                new FinancialAsset()
                {
                    FiatCurrencyID = 2,
                    AssetPlatformID = 2,
                    FinancialEntityId = 2,
                    UserID = "User2",
                    AssetSourceID = 1,
                    Amount = 666,
                    FirstRetrievedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow,
                }
            };
        }


    }
}
