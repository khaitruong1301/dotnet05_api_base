using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet05_api_base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using dotnet05_api_base.Models;

namespace dotnet05_api_base.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CybersoftMarketplaceContext _context;
        public UserController(CybersoftMarketplaceContext context)
        {
            _context = context;
        }


        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _context.Users.OrderBy(item => item.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(item => new
            {
                id = item.Id,
                fullName = item.FullName,
                email = item.Email,
                phoneNumber = item.Phone,
                address = item.Address,
                avatar = item.Avatar
            }).ToListAsync();
            return Ok(users);

        }

      
    }
}