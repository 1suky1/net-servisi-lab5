using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using lab5.DAL.Entities;
using lab5.JWTApi.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace lab5.Tests.Tests
{
    public class Address_Should
    {
		DbContextOptions<AdventureWorks2014Context> _dbContextOptions;

		public Address_Should()
		{
			_dbContextOptions = new DbContextOptionsBuilder<AdventureWorks2014Context>().UseInMemoryDatabase(databaseName: "name").Options;
		}

		[Fact]
		public async void PostAddress()
		{
			using (var context = new AdventureWorks2014Context(_dbContextOptions))
			{
				var addressApi = new AddressesController(context);
				for (int i = 0; i < 10; ++i)
				{
					Address address = new Address();
					address.AddressLine1 = "Address line 1";
					address.AddressLine2 = "Address line 2";
					address.City = "Zagreb";
					address.PostalCode = $"1234{ i }";
					var result = await addressApi.PostAddress(address);
					var badRequest = result as BadRequestObjectResult;

					Assert.Null(badRequest);
				}
			}
		}

		[Fact]
		public async void GetAddress()
		{
			using (var context = new AdventureWorks2014Context(_dbContextOptions))
			{
				var addressApi = new AddressesController(context);
				for (int i = 0; i < 10; ++i)
				{
					Address address = new Address();
					address.AddressLine1 = "Address line 1";
					address.AddressLine2 = "Address line 2";
					address.City = "Zagreb";
					address.PostalCode = $"1234{ i }";
					addressApi.PostAddress(address).Wait();
				}
			}

			using (var context = new AdventureWorks2014Context(_dbContextOptions))
			{
				var addressApi = new AddressesController(context);
				var result = await addressApi.GetAddress(5);
				var okResult = result as OkObjectResult;

				Assert.NotNull(okResult);
				Assert.Equal(200, okResult.StatusCode);

				Address address = okResult.Value as Address;
				Assert.NotNull(address);
				Assert.Equal("12344", address.PostalCode);
			}

		}

    }
}
