using Assignment_1.Data;
using Assignment_1.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
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

        [HttpGet]
        [Route("GetAllEmployee"), Authorize(Roles = "Admin")]
        public ActionResult<List<Emplyoee>> GetAllEmployee()
        {
            try
            {
                List<Emplyoee> EmpList = _dbContext.Employee.ToList<Emplyoee>();
                return Ok(EmpList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost]
        [Route("AddEmployee"), Authorize(Roles = "Admin")]
        public ActionResult AddEmployee(Emplyoee emp)
        {
            try
            {
                _dbContext.Employee.Add(emp);
                _dbContext.SaveChanges();
                return Ok("Employee Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
        [HttpGet]
        [Route("GetEmployee/{Id}")]
        public ActionResult<Emplyoee> GetEmployeeById(int Id)
        {
            try
            {
                Emplyoee result = _dbContext.Employee.FirstOrDefault(x => x.Id == Id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPut]
        [Route("UpdateEmployee"), Authorize(Roles = "Admin")]
        public ActionResult UpdateEmployee(Emplyoee emp)
        {
            try
            {
                var Result = _dbContext.Employee.Where(x => x.Id == emp.Id).FirstOrDefault();
                Result.Name = emp.Name;
                Result.Email = emp.Email;
                Result.Password = emp.Password;
                Result.Role = emp.Role;
                _dbContext.SaveChanges();
                return Ok("Employee Added Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpDelete]
        [Route("DeleteEmplyee/{Id}"), Authorize(Roles = "Admin")]
        public ActionResult DeleteEmployee(int Id)
        {
            try
            {
                var result = _dbContext.Employee.Where(x => x.Id == Id).FirstOrDefault();
                _dbContext.Employee.Remove(result);
                _dbContext.SaveChanges();
                return Ok("Employee Deleted Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

    }
}
