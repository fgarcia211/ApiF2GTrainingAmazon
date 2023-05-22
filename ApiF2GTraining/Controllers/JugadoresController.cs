using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiF2GTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JugadoresController : ControllerBase
    {
        private IRepositoryF2GTraining repo;

        public JugadoresController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        // POST: api/Jugadores
        /// <summary>
        /// Inserta un jugador introducido por el usuario en la BBDD
        /// </summary>
        /// <remarks>
        /// Inserta jugador en la BBDD:
        /// 
        /// - El ID de equipo debe pertenecer al usuario
        /// - Debe introducirse un ID de posicion existente
        /// </remarks>
        /// <param name="j">Jugador a introducir</param>
        /// <response code="200">OK. Inserta el jugador en la BBDD</response>
        /// <response code="400">ERROR: Ha ocurrido algun error de introduccion</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> InsertJugador(Jugador j)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(j.IdEquipo);

            if (equipo == null)
            {
                return BadRequest();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                //Hay que comprobar el ID posicion, que existe
                if (HelperF2GTraining.PosicionCorrecta(await this.repo.GetPosiciones(), j.IdPosicion))
                {
                    await this.repo.InsertJugador(j.IdEquipo, j.IdPosicion, j.Nombre, j.Dorsal, j.Edad, j.Peso, j.Altura);
                    return Ok();
                }
                else
                {
                    return BadRequest(new
                    {
                        response = "Error: El ID de posicion no es correcta"
                    });
                }
                
            }
            else
            {
                return Unauthorized();
            }
            
        }


        // GET: api/Jugadores/GetPosiciones
        /// <summary>
        /// Devuelve todas las posiciones de los jugadores disponibles
        /// </summary>
        /// <remarks>
        /// Devuelve las posiciones
        /// </remarks>
        /// <response code="200">OK. Devuelve las posiciones introducidas</response>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Posicion>>> GetPosiciones()
        {
            return await this.repo.GetPosiciones();
        }

        // GET: api/Jugadores/GetJugadorID/{idjugador}
        /// <summary>
        /// Devuelve el jugador con el ID correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve jugador por ID en la BB.DD
        /// 
        /// - El ID de usuario, del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Devuelve el jugador de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el jugador con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Jugador>> GetJugadorID(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return jugador;
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/Jugadores/GetEstadisticasJugador/{idjugador}
        /// <summary>
        /// Devuelve las estadisticas del jugador con el ID correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve estadisticas por ID Jugador en la BB.DD
        /// 
        /// - El ID de usuario, del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Devuelve las estadisticas de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el jugador con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<EstadisticaJugador>> GetEstadisticasJugador(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.GetEstadisticasJugador(idjugador);
            }
            else
            {
                return Unauthorized();
            }
        }

        // GET: api/Jugadores/GetJugadoresEquipo/{idequipo}
        /// <summary>
        /// Devuelve los jugadores pertenecientes al ID de equipo introducido en la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve jugadores por ID Equipo en la BB:DD
        /// 
        /// - El ID de usuario del equipo, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idequipo">ID de equipo a introducir</param>
        /// <response code="200">OK. Devuelve los jugadores de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el equipo con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Jugador>>> GetJugadoresEquipo(int idequipo)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (equipo == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.GetJugadoresEquipo(idequipo);
            }
            else
            {
                return Unauthorized();
            }

        }

        // GET: api/Jugadores/JugadoresXUsuario
        /// <summary>
        /// Devuelve los jugadores pertenecientes al usuario de la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve los jugadores del usuario
        /// </remarks>
        /// <response code="200">OK. Borra el jugador de la BB.DD</response>
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Jugador>>> JugadoresXUsuario()
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            return await this.repo.JugadoresXUsuario(user.IdUsuario);
        }


        // DELETE: api/Jugadores/DeleteJugador/{idjugador}
        /// <summary>
        /// Borra el jugador con el ID Correspondiente en la BB.DD
        /// </summary>
        /// <remarks>
        /// Borra el jugador con ese ID
        /// 
        /// - El ID de usuario ,del equipo del jugador, debe pertenecer al usuario
        /// </remarks>
        /// <param name="idjugador">ID de jugador a introducir</param>
        /// <response code="200">OK. Borra el jugador de la BB.DD</response>
        /// <response code="404">ERROR: No se ha encontrado el equipo con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpDelete]
        [Route("[action]/{idjugador}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> DeleteJugador(int idjugador)
        {
            Jugador jugador = await this.repo.GetJugadorID(idjugador);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(jugador.IdEquipo);

            if (jugador == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                await this.repo.DeleteJugador(idjugador);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }

        }

    }
}
