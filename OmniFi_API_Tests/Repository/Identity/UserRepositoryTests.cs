using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using NUnit.Framework;
using OmniFi_API.Data;
using OmniFi_API.Data.Interfaces;
using OmniFi_API.Dtos.Identity;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;
using OmniFi_API.Options.Identity;
using OmniFi_API.Repository.Identity;
using OmniFi_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Assert = NUnit.Framework.Assert;

namespace OmniFi_API_Tests.Repository.Identity
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<ApplicationDbContext>? _db;
        private Mock<UserManager<OmniFi_API.Models.Identity.ApplicationUser>>? _userManager;
        private Mock<RoleManager<ApplicationRole>>? _roleManager;
        private Mock<IMapper>? _mapper;
        private Mock<IOptions<UserRepositoryOptions>>? _options;
        private UserRepository? _ct;
        private Mock<IdentityResult>? _identityResult;

        [SetUp]
        public void Setup()
        {
            _db = new Mock<ApplicationDbContext>();

            _userManager = new Mock<UserManager<OmniFi_API.Models.Identity.ApplicationUser>>(
                new Mock<IUserStore<OmniFi_API.Models.Identity.ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<OmniFi_API.Models.Identity.ApplicationUser>>().Object,
                new IUserValidator<OmniFi_API.Models.Identity.ApplicationUser>[0],
                new IPasswordValidator<OmniFi_API.Models.Identity.ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<OmniFi_API.Models.Identity.ApplicationUser>>>().Object
                );

            _roleManager = new Mock<RoleManager<ApplicationRole>>(
                new Mock<IRoleStore<ApplicationRole>>().Object,
                new IRoleValidator<ApplicationRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<ApplicationRole>>>().Object
                );

            _mapper = new Mock<IMapper>();

            _options = new Mock<IOptions<UserRepositoryOptions>>();

            _options!
                .Setup(x => x.Value)
                .Returns(new UserRepositoryOptions() { ExpirationTime = 7, SecretKey = "This a secret API key for testing purpose" });

            _ct = new UserRepository(
                _db.Object,
                _userManager.Object,
                _roleManager.Object,
                _mapper.Object,
                _options.Object);

            _identityResult = new Mock<IdentityResult>();

        }


        private static List<OmniFi_API.Models.Identity.ApplicationUser> GetApplicationUsers()
        {
            var usedFiatCurrency = new FiatCurrency()
            {
                CurrencyCode = "EUR",
                CurrencyName = "Euros",
                CurrencySymbol = "$"
            };

            return new List<OmniFi_API.Models.Identity.ApplicationUser>()
            {
                new OmniFi_API.Models.Identity.ApplicationUser()
                {
                    Id = "testID",
                    UserName = "TestUserName",
                    FirstName = "TestFirstName",
                    FiatCurrency = usedFiatCurrency,
                    Email = "test@mail.com",
                    NormalizedEmail = "test@mail.com".ToUpper(),
                    LastName = "TestLastName"
                },
                new OmniFi_API.Models.Identity.ApplicationUser()
                {
                    Id = "testID1",
                    UserName = "TestUserName1",
                    FirstName = "TestFirstName1",
                    FiatCurrency = usedFiatCurrency,
                    Email = "test1@mail.com",
                    NormalizedEmail = "test1@mail.com".ToUpper(),
                    LastName = "TestLastName1"
                }
            };
        }

        private static List<FiatCurrency> GetFiatCurrencies()
        {
            return new List<FiatCurrency>()
            {
                new FiatCurrency() 
                {
                    CurrencyCode = "EUR",
                    CurrencyName = "Euros",
                    CurrencySymbol = "$"
                }

            };

         }

            [Test]
        public void IsUserExistsByUserName_ShallReturnTrue_ForExistingUserName()
        {
            // Arrange
            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            var userName = "TestUserName";

            // Act
            var result = _ct!.IsUserExistsByUserName(userName);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserExistsByEmail_ShallReturnTrue_ForExistingEmail()
        {
            // Arrange
            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            var email = "test1@mail.com";

            // Act
            var result = _ct!.IsUserExistsByEmail(email);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserExistsByUserName_ShallReturnFalse_ForNonExistingUserName()
        {
            // Arrange
            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            var userName = "UserName";

            // Act
            var result = _ct!.IsUserExistsByUserName(userName);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsUserExistsByEmail_ShallReturnFalse_ForNonExistingEmail()
        {
            // Arrange
            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            var email = "tes@mail.com";

            // Act
            var result = _ct!.IsUserExistsByEmail(email);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCaseSource(nameof(RegisterationRequestDTOsCases))]
        public void Register_ShallReturnAnUserDTO_ForACorrectRegistrationRequestDTO(RegisterationRequestDTO registerationRequestDTO)
        {
            // Arrange
            _userManager!
                .Setup(x => x.CreateAsync(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Success);

            _userManager!
                .Setup(x => x.AddToRoleAsync(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>(), It.IsAny<string>()).Result)
                .Returns(IdentityResult.Success);

            _roleManager!
                .Setup(x => x.RoleExistsAsync(It.IsAny<string>()).Result)
                .Returns(true);

            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            _db!
                .Setup(x => x.FiatCurrencies)
                .ReturnsDbSet(GetFiatCurrencies());

            _mapper!
                .Setup(x => x.Map<UserDTO>(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>()))
                .Returns(new UserDTO()
                {
                    FirstName = registerationRequestDTO.FirstName,
                    LastName = registerationRequestDTO.LastName,
                    UserName = registerationRequestDTO.UserName,
                    ID = "test"
                });

            //_mapper!
            //    .Setup(x => x.Map<UserDTO>(It.IsAny<ApplicationUser>()))
            //    .Returns(new UserDTO() { });

            var email = "tes@mail.com";

            // Act
            var result = _ct!.Register(registerationRequestDTO).Result;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.User!.UserName, Is.EqualTo(registerationRequestDTO.UserName));
        }

        public static RegisterationRequestDTO[] RegisterationRequestDTOsCases()
        {
            return
            [
                new RegisterationRequestDTO(){
                    FirstName = "Test",
                    Email = "test@mail",
                    LastName = "testLastName",
                    Password = "1234",
                    UserName="TestUserName"
                },

                new RegisterationRequestDTO(){
                    FirstName = "Test1",
                    Email = "test1@mail",
                    LastName = "test1LastName",
                    Password = "1234",
                    UserName="TestUserName1"
                }
            ];
        }

        [TestCaseSource(nameof(LoginRequestDTOsCases))]
        public void Login_ShallReturnALoginResponseDTO_ForACorrectLoginRequestDTO(LoginRequestDTO loginRequestDTO)
        {
            // Arrange
            _userManager!
                .Setup(x => x.GetRolesAsync(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>()).Result)
                .Returns(new List<string>() { Roles.User});

            _userManager!
                .Setup(x => x.CheckPasswordAsync(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>(), It.IsAny<string>()).Result)
                .Returns(true);

            _db!
                .Setup(x => x.Users)
                .ReturnsDbSet(GetApplicationUsers());

            _db!
                .Setup(x => x.FiatCurrencies)
                .ReturnsDbSet(GetFiatCurrencies());

            _mapper!
                .Setup(x => x.Map<UserDTO>(It.IsAny<OmniFi_API.Models.Identity.ApplicationUser>()))
                .Returns(new UserDTO()
                {
                    FirstName = "test",
                    LastName = "test",
                    UserName = loginRequestDTO!.UserNameOrEmail,
                    ID = "test"
                });

            //_mapper!
            //    .Setup(x => x.Map<UserDTO>(It.IsAny<ApplicationUser>()))
            //    .Returns(new UserDTO() { });

            var email = "tes@mail.com";

            // Act
            var result = _ct!.Login(loginRequestDTO).Result;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.User.UserName, Is.EqualTo(loginRequestDTO.UserNameOrEmail));
            Assert.That(result!.Token, Is.Not.Null);
        }

        public static LoginRequestDTO[] LoginRequestDTOsCases()
        {
            return
            [
                new LoginRequestDTO(){
                    UserNameOrEmail = "test@mail",
                    Password = "1234",
                },

                new LoginRequestDTO(){
                    UserNameOrEmail = "test1@mail",
                    Password = "1234",
                }
            ];
        }
    }
}
