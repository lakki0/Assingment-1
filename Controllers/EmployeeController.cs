using Assignment_1.Data;
using Assignment_1.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AssignDBContext _dbContext;
       private readonly IConfiguration _configuration;

        public EmployeeController(AssignDBContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login([FromBody] Emplyoee emplyoee)
        {
            if(emplyoee == null)
            {
                return BadRequest();
            }

            var user = await _dbContext.Employee.FirstOrDefaultAsync(e=>e.Email== emplyoee.Email && e.Password==emplyoee.Password);
           
            if (user == null)
            {

                return NotFound();
            }

            var token = CreateToken(user);

            return Ok(new { user = user, token=token});


        }
        private string CreateToken(Emplyoee emplyoee)
        {
            List<Claim> claims = new List<Claim>
             {
               new Claim(ClaimTypes.Name, emplyoee.Email),
               new Claim(ClaimTypes.Role, emplyoee.Role)
              };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

              _configuration["JWT:Issuer"],

              _configuration["JWT:Audience"],

              claims,

              expires: DateTime.Now.AddDays(10),

              signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
