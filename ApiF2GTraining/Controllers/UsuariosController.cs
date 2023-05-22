using ApiF2GTraining.Helpers;
using ApiF2GTraining.Models;
using ApiF2GTraining.Repositories;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ApiF2GTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private IRepositoryF2GTraining repo;
        private HelperOAuthToken helper;

        public UsuariosController(IRepositoryF2GTraining repo, HelperOAuthToken helper)
        {
            this.repo = repo;
            this.helper = helper;
        }

        // GET: api/Usuarios
        /// <summary>
        /// Inserta un usuario en la BB.DD de USUARIOS
        /// </summary>
        /// <remarks>
        /// Inserta usuarios en la BB.DD
        /// 
        /// - El telefono, el correo y el nombre de usuario deben ser únicos
        /// </remarks>
        /// <param name="user">JSON del usuario</param>
        /// <response code="200">OK. Devuelve los entrenamientos del equipo solicitado</response>
        /// <response code="400">ERROR: Solicitud mal introducida</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InsertUsuario(Usuario user)
        {
            if (!(await this.repo.CheckUsuarioRegistro(user.Nombre)) && !(await this.repo.CheckTelefonoRegistro(user.Telefono)) && !(await this.repo.CheckCorreoRegistro(user.Correo)))
            {
                await this.repo.InsertUsuario(user);
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    response = "Error: El telefono, el correo o el nombre de usuario esta repetido dentro de la BBDD"
                });
            }
            
        }

        // POST: api/Usuarios/Login
        /// <summary>
        /// Devuelve el token para hacer login con el nombre y la contrasenia introducida si coincide con la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve token para peticiones
        /// </remarks>
        /// <param name="model">Nombre y contraseña del usuario</param>
        /// <response code="200">OK. Devuelve el token para realizar peticiones protegidas</response>        
        /// <response code="401">Credenciales incorrectas</response>
        [HttpPost]
        [Route(("[action]"))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Usuario>> Login(LoginModel model)
        {
            Usuario user = await this.repo.GetUsuarioNamePass(model.username, model.password);
            if (user != null)
            {
                SigningCredentials credentials =
                new SigningCredentials(this.helper.GetKeyToken()
                , SecurityAlgorithms.HmacSha256);

                string jsonUser = JsonConvert.SerializeObject(user);
                Claim[] info = new[]
                {
                    new Claim("UserData", jsonUser)
                };

                JwtSecurityToken token =
                    new JwtSecurityToken(
                        claims: info,
                        issuer: this.helper.Issuer,
                        audience: this.helper.Audience,
                        signingCredentials: credentials,
                        expires: DateTime.UtcNow.AddMinutes(180),
                        notBefore: DateTime.UtcNow
                        );
                return Ok(new
                {
                    response =
                    new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Usuarios/GetUsuarioLogueado
        /// <summary>
        /// Devuelve el usuario que ha hecho login en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve usuario logueado
        /// </remarks>
        /// <response code="200">OK. Devuelve el usuario logueado</response>  
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Usuario>> GetUsuarioLogueado()
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            return user;
        }

        // GET: api/Usuarios/TelefonoRegistrado/{telefono}
        /// <summary>
        /// Devuelve si un telefono introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un telefono ya existe
        /// </remarks>
        /// <param name="telefono">Telefono a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{telefono}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> TelefonoRegistrado(int telefono)
        {
            return await this.repo.CheckTelefonoRegistro(telefono);
        }

        // GET: api/Usuarios/NombreRegistrado/{nombre}
        /// <summary>
        /// Devuelve si un nombre introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un nombre ya existe
        /// </remarks>
        /// <param name="nombre">Nombre a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{nombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> NombreRegistrado(string nombre)
        {
            return await this.repo.CheckUsuarioRegistro(nombre);
        }

        // GET: api/Usuarios/CorreoRegistrado/{correo}
        /// <summary>
        /// Devuelve si un correo introducido por el usuario existe en la tabla USUARIOS
        /// </summary>
        /// <remarks>
        /// Comprueba si un correo ya existe
        /// </remarks>
        /// <param name="correo">Correo a comprobar</param>
        /// <response code="200">OK. Devuelve true o false, dependiendo si existe</response>   
        [HttpGet]
        [Route("[action]/{correo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CorreoRegistrado(string correo)
        {
            return await this.repo.CheckCorreoRegistro(correo);
        }

    }
}
