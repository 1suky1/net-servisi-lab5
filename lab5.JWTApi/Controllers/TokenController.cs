using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using lab5.DAL.Entities;
using Microsoft.Extensions.Configuration;
using lab5.JWTApi.Models;
using System.IdentityModel.Tokens.Jwt;
using lab5.JWTApi.Helpers;

namespace lab5.JWTApi.Controllers
{
	[Produces("application/json")]
	[ApiVersion("1.0")]
	[Route("api/Token")]
	[AllowAnonymous]
	public class TokenController : Controller
    {
		private readonly AdventureWorks2014Context _context;

		public IConfiguration Configuration { get; }

		public TokenController(AdventureWorks2014Context context, IConfiguration configuration)
		{
			_context = context;
			Configuration = configuration;
		}

		[HttpPost("RequestToken")]
		public IActionResult RequestToken([FromBody]  TokenRequestModel tokenRequest)
		{
			if(_context.Customer.Any(c=>c.Person.FirstName == tokenRequest.FirstName /*&& c.AccountNumber == tokenRequest.AccountNumber*/))
			{
				JwtSecurityToken token = JwsTokenCreator.CreateToken(tokenRequest.FirstName, Configuration["Auth:JwtSecurityKey"], Configuration["Auth:ValidIssuer"], Configuration["Auth:ValidAudience"]);
				string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

				return Ok(tokenStr);
			}

			return Unauthorized();
		}

		[HttpGet("RequestTokenVersion")]
		[HttpPost("RequestTokenVersion")]
		[MapToApiVersion("1.0"), MapToApiVersion("1.1")]
		public string GetApiVersion() => HttpContext.GetRequestedApiVersion().ToString();

	}


	[Produces("application/json")]
	[ApiVersion("1.1")]
	//[Route("api/v{version:apiVersion}/Token")]
	[Route("api/Token")]
	[AllowAnonymous]
	public class TokenV1_1Controller : Controller
	{
		private readonly AdventureWorks2014Context _context;

		public IConfiguration Configuration { get; }

		public TokenV1_1Controller(AdventureWorks2014Context context, IConfiguration configuration)
		{
			_context = context;
			Configuration = configuration;
		}

		[HttpPost("RequestToken")]
		public IActionResult RequestToken([FromBody] TokenRequestModel tokenRequest)
		{
			var user =  _context.Customer.FirstOrDefault(c => c.Person.FirstName == tokenRequest.FirstName /* && c.AccountNumber == tokenRequest.AccountNumber*/);
			if (user != null)
			{
				JwtSecurityToken token = JwsTokenCreator.CreateToken(tokenRequest.FirstName,
																	Configuration["Auth:JwtSecurityKey"],
																	Configuration["Auth:ValidIssuer"],
																	Configuration["Auth:ValidAudience"]);
				string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

				return Ok(tokenStr);
			}
			return Unauthorized();
		}
	}
}