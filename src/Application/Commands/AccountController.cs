using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reventuous;
using static Reventuous.Sample.Application.Commands;

namespace Reventuous.Sample.Application
{
    [ApiController]
    [Route("/api/account")]
    public class AccountController : ControllerBase
    {

        AccountService _service;
        readonly ILogger<AccountController> _log;
        public AccountController(
            AccountService service,
            ILoggerFactory loggerFactory
        )
        {
            _service = service;
            _log = loggerFactory.CreateLogger<AccountController>();
        } 
        
        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccount command)
        {
            try 
            {
                await _service.Handle(command, default);
                return Ok();
            }
            catch(DomainException e) 
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _log.LogError($"{e.Message} - {e.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IActionResult> CreditAccount(CreditAccount command)
        {
            try 
            {
                await _service.Handle(command, default);
                return Ok();
            }
            catch(DomainException e) 
            {
                return BadRequest(e.Message);
            }
            catch(ArgumentNullException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                _log.LogError($"{e.Message} - {e.StackTrace}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


    }
}
