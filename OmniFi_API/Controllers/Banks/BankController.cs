﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OmniFi_API.Dtos.Banks;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Controllers;
using OmniFi_API.Repository.Interfaces;
using OmniFi_API.Utilities;
using System.Net;

namespace OmniFi_API.Controllers.Banks
{
    [Route($"api/{ControllerRouteNames.BankController}")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IRepository<Bank> _bankRepository;
        private ApiResponse _apiResponse;
        private readonly IMapper _mapper;

        public BankController(IRepository<Bank> bankRepository, IMapper mapper)
        {
            _bankRepository = bankRepository;
            _apiResponse = new ApiResponse();
            _mapper = mapper;
        }


        [HttpGet("{id:int}", Name = nameof(GetBank))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetBank(int id)
        {
            try
            {
                if(id == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var bank = await _bankRepository.GetAsync(x => x.BankID == id);

                if(bank is null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode =HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode=HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<BankDTO>(bank);

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess=false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet(nameof(GetBanks))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetBanks()
        {
            try
            {
                var banks = await _bankRepository.GetAllAsync();

                _apiResponse.IsSuccess = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                _apiResponse.Result = _mapper.Map<IEnumerable<BankDTO>>(banks);

                return Ok(_apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.AddErrorMessage(ex.Message);
                return _apiResponse;
            }
        }

    }
}