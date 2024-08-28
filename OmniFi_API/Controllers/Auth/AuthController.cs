using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Utilities;
using OmniFi_DTOs.Dtos.Api;
using System.Net;

namespace OmniFi_API.Controllers.Auth
{
    [Route($"api/{ControllerRouteNames.AuthController}")]
    [ApiController]


    public class AuthController : ControllerBase
    {
        private ApiResponse _apiResponse;
        public AuthController()
        {
            _apiResponse = new ApiResponse();
        }

        [HttpGet(nameof(Callback))]
        public async Task<ActionResult<ApiResponse>> Callback([FromQuery] string code, [FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.AddErrorMessage("Authorization code not provide");

                return BadRequest(_apiResponse);
            }
        }
    }
}
