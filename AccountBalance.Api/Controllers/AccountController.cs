using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountBalance.Api.Models;
using AccountBalance.Application.Interfaces;
using AccountBalance.Domain.Aggregates.AccountAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountApplicationService _accountApplicationService;
        private readonly IAccountQueryService _accountQueryService;

        public AccountController(IAccountApplicationService accountApplicationService, IAccountQueryService accountQueryService) 
        {
            _accountApplicationService = accountApplicationService;
            _accountQueryService = accountQueryService;
        }

        [HttpPost("new")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountDTO model)
        {
            var accountId = await _accountApplicationService.CreateAccountAsync(model.HolderName);
            return Ok(accountId);
        }

        [HttpPut("setDailyWireTransferLimit")]
        public async Task<IActionResult> SetDailyWireTransferLimit([FromBody]SetDailyWireTransferLimitDTO model)
        {
            await _accountApplicationService.SetDailyWireTransferLimitAsync(AccountId.With(model.AccountId), model.DailyWireTransfer);
            return Ok(new { isSuccess = true });
        }
        [HttpPut("setOverDraftLimit")]
        public async Task<IActionResult> SetOverDraftLimit([FromBody]SetOverDraftLimitDTO model)
        {
            await _accountApplicationService.SetOverDraftLimitAsync(AccountId.With(model.AccountId), model.OverDraftLimit);
            return Ok(new { isSuccess = true });
        }
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            var account = await _accountQueryService.GetAccountByIdAsync(AccountId.With(accountId));
            return Ok(account);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountQueryService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [HttpPut("withdraw")]
        public async Task<IActionResult> WithdrawCash([FromBody]WitddrawCashDTO model)
        {
            await _accountApplicationService.WithdrawCashAsync(AccountId.With(model.AccountId), model.Amount);
            return Ok(new {isSuccess = true});
        }

        [HttpPut("{type}/deposit")]
        public async Task<IActionResult> Deposit(DepositType type, [FromBody]DepositDTO model)
        {
            if (type == DepositType.Cash)
            {
                await _accountApplicationService.DepositCashAsync(AccountId.With(model.AccountId), model.Amount);
                return Ok(new {isSuccess = true});
            }
            if (type == DepositType.Check) {
                await _accountApplicationService.DepositCheckAsync(AccountId.With(model.AccountId), model.Amount);
                return Ok(new { isSuccess = true });
            }
            return BadRequest();
        }
    }
}