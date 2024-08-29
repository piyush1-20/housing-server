using API.Dtos;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Web.Helpers;
using API.Errors;


namespace API.Controllers
{
   
    public class AccountController:BaseController
    {
        private readonly IUnitofWork uow;
        private readonly IConfiguration configuration;

        public AccountController(IUnitofWork uow,IConfiguration configuration)
        {
            this.uow = uow;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult>Login(LoginReqDto loginreq)
        {
            var user = await uow.UserRepository.Authenticate(loginreq.UserEmail, loginreq.Password);

            APIError apierror = new APIError();
            if (user == null)
            {
                apierror.ErrorCode = Unauthorized().StatusCode;
                apierror.ErrorMessage = "Invalid user Id or password";
                apierror.ErrorDetails = "This error occurs when userID or password is incorrect";
                return Unauthorized(apierror);
            }

            var loginRes = new LoginResDto();
            loginRes.UserEmail = user.UserEmail;
            loginRes.Token = CreateJWT(user);
            return Ok(loginRes);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(LoginReqDto loginreq)
        {

            if(await uow.UserRepository.UserAlreadyExists(loginreq.UserEmail))
            {
                
                return BadRequest("User Already Exists");
            }
            uow.UserRepository.Register(loginreq.UserEmail, loginreq.Password);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string CreateJWT(User user)
        {

            var secretKey = configuration.GetSection("AppSettings:Key").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Email,user.UserEmail),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
            };

            var signCredential = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signCredential
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
