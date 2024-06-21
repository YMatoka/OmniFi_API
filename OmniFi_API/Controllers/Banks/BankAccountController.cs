using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Dtos.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers.Banks
{
    [Route($"api/{ControllerRouteNames.BankExchangeController}")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private readonly IRepository<Bank> _banksRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private ApiResponse _apiResponse;

        public BankAccountController(
            IUserRepository userRepository,
            IBankAccountRepository bankAccountRepository,
            IRepository<Bank> banksRepository)
        {
            _userRepository = userRepository;
            _apiResponse = new();
            _bankAccountRepository = bankAccountRepository;
            _banksRepository = banksRepository;
        }


        [HttpPost(nameof(Create))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> Create([FromBody] BankAccountCreateDTO bankAccountCreateDTO)
        {
            var user = await _userRepository.GetUserAsync(bankAccountCreateDTO.UsernameOrEmail);

            if (user is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the username or email '{bankAccountCreateDTO.UsernameOrEmail}' is invalid");
                return BadRequest(_apiResponse);
            }

            var bank = await _banksRepository.GetAsync(
                filter: (x) => x.BankName.ToUpper() == bankAccountCreateDTO.BankName.ToUpper(),
                tracked: false);

            if (bank is null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the bank '{bankAccountCreateDTO.BankName}' does'nt exists in the database");
                return BadRequest(_apiResponse);
            }

            if (await _bankAccountRepository.GetAsync(
                (x) => x.UserID == user.Id
                && x.BankID == bank.BankID) is not null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages.Add($"the user '{user.UserName}' already have a '{bank.BankName}' account");
                return BadRequest(_apiResponse);
            }

            var bankAccount = new BankAccount()
            {
                BankID = bank.BankID,
                UserID = user.Id
            };

            await _bankAccountRepository.CreateAsync(bankAccount, bankAccountCreateDTO);

            _apiResponse.IsSuccess = true;
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return CreatedAtAction(nameof(Create), _apiResponse);
        }

    }
}
