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
        /// <summary>
        /// Create New Account
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("new")]
        public async Task<IActionResult> CreateAccount([FromBody]CreateAccountDTO model)
        {
            var accountId = await _accountApplicationService.CreateAccountAsync(model.HolderName);
            return Ok(accountId);
        }
        /// <summary>
        /// Set Daily Wire Transfer Limit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("setDailyWireTransferLimit")]
        public async Task<IActionResult> SetDailyWireTransferLimit([FromBody]SetDailyWireTransferLimitDTO model)
        {
            try
            {
                await _accountApplicationService.SetDailyWireTransferLimitAsync(AccountId.With(model.AccountId), model.DailyWireTransfer);
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }
            return Ok(new { isSuccess = true });
        }
        /// <summary>
        /// Set Over Draft Limit
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("setOverDraftLimit")]
        public async Task<IActionResult> SetOverDraftLimit([FromBody]SetOverDraftLimitDTO model)
        {
            try
            {
                await _accountApplicationService.SetOverDraftLimitAsync(AccountId.With(model.AccountId), model.OverDraftLimit);
            }
            catch (Exception e)
            {
                return BadRequest(new { Error = e.Message });
            }
            return Ok(new { isSuccess = true });
        }
        /// <summary>
        /// Get Account By Provided Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            var account = await _accountQueryService.GetAccountByIdAsync(AccountId.With(accountId));
            return Ok(account);
        }
        /// <summary>
        /// Get List Of All Accounts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountQueryService.GetAllAccountsAsync();
            return Ok(accounts);
        }
        /// <summary>
        /// Withdraw Cash From Account Balance
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("withdraw")]
        public async Task<IActionResult> WithdrawCash([FromBody]WitddrawCashDTO model)
        {
            try
            {
                await _accountApplicationService.WithdrawCashAsync(AccountId.With(model.AccountId), model.Amount);
            }
            catch (Exception e)
            {
                return BadRequest(new {Error = e.Message});
            }
            return Ok(new {isSuccess = true});
        }
        /// <summary>
        /// Deposit Cash Into Account 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{type}/deposit")]
        public async Task<IActionResult> Deposit(DepositType type, [FromBody]DepositDTO model)
        {
            if (type == DepositType.Cash)
            {
                try
                {
                    await _accountApplicationService.DepositCashAsync(AccountId.With(model.AccountId), model.Amount);
                }
                catch (Exception e)
                {
                    return BadRequest(new { Error = e.Message });
                }
                return Ok(new {isSuccess = true});
            }
            if (type == DepositType.Check) {
                try
                {
                    await _accountApplicationService.DepositCheckAsync(AccountId.With(model.AccountId), model.Amount);
                }
                catch (Exception e)
                {
                    return BadRequest(new { Error = e.Message });
                }
                return Ok(new { isSuccess = true });
            }
            return BadRequest();
        }
    }
}