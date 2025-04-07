using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiPerson.Data;
using WebApiPerson.MODELO;

namespace WebApiPerson.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
       
        private readonly IConfiguration _config;
        private readonly PersonDbContext _context;

        
        public AuthController(IConfiguration config, PersonDbContext context)
        {
            _config = config;
            _context = context;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            // Busca en la base de datos el cliente cuyo correo coincida con el proporcionado.
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == loginDto.Email);

            // Valida que el cliente exista y que la contraseña sea correcta.
            if (cliente == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, cliente.Password))
            {
                return Unauthorized("Credenciales inválidas");
            }


            // Genera un token JWT para el cliente autenticado.
            var token = GenerateJwtToken(cliente);

          // Retorna el token en la respuesta.
            return Ok(new { token });
        }

        // Método para generar un token JWT.
        private string GenerateJwtToken(Cliente cliente)
        {
            var secretKey = _config["JwtSettings:SecretKey"];
            // Depurar: escribir en consola el valor y la longitud de la clave
            Console.WriteLine($"Clave utilizada: '{secretKey}' (longitud: {secretKey.Length} caracteres)");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, cliente.Email),
        new Claim("ClienteId", cliente.Id.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO para recibir las credenciales del login utilizando el correo
    public class UserLoginDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

}



