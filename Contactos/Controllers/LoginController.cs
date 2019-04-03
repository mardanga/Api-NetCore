using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contactos.Models;
using Contactos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Contactos.Controllers
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private IUserService _us;

        public LoginController(IConfiguration config, IUserService us)
        {
            _config = config;
            _us = us;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody]UserDto login)
        {
            IActionResult response = Unauthorized();
            //Método responsable de Validar las credenciales del usuario y devolver el modelo Usuario
            //Para demostración (en este punto) he usado datos de prueba sin persistencia de Datos
            //Si no retorna un objeto nulo, se procede a generar el JWT.
            //Usando el método GenerateJSONWebToken
            //var user = AuthenticateUser(login);
            var user = _us.Authenticate(login.Username, login.Password);

        if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            User user = new User();

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.FechaCreado = userDto.FechaCreado;

            try
            {
                _us.Create(user, userDto.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
                
            }
        }


        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
            new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
            new Claim("FechaCreado", userInfo.FechaCreado.ToString("yyyy-MM-dd")),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            //Se crea el token utilizando la clase JwtSecurityToken
            //Se le pasa algunos datos como el editor (issuer), audiencia
            // tiempo de expiración y la firma.

            var token = new JwtSecurityToken(_config["jwt:Issuer"],
                _config["jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            //Finalmente el método JwtSecurityTokenHandler genera el JWT. 
            //Este método espera un objeto de la clase JwtSecurityToken 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Usuario AuthenticateUser(Usuario login)
        {
            Usuario user = null;

            //Validate the User Credentials  
            //Demo Purpose, I have Passed HardCoded User Information  
            if (login.Username == "Daniel")
            {
                user = new Usuario { Username = "Daniel", Password = "123456",Email = "mail@mial.com", FechaCreado = DateTime.Now                   };  
                //user = new Usuario { Username = login.Username, Password = login.Password, Email = login.Email, FechaCreado = login.FechaCreado };
            }
            return user;
        }
    }
}