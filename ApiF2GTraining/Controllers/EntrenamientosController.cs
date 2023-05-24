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
    public class EntrenamientosController : ControllerBase
    {
        private IRepositoryF2GTraining repo;

        public EntrenamientosController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        // POST: api/Entrenamientos
        /// <summary>
        /// Inserta un entrenamiento en la BB.DD segun el nombre del entrenamiento y el ID de su equipo
        /// </summary>
        /// <remarks>
        /// Inserta entrenamiento con nombre, y el id del equipo
        /// </remarks>
        /// <param name="idequipo">Id del equipo.</param>
        /// <param name="nombre">Nombre del entrenamiento</param>
        /// <response code="200">OK. Inserta el entrenamiento en BB.DD</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>  
        [Authorize]
        [HttpPost("{idequipo}/{nombre}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> InsertEntrenamiento(int idequipo, string nombre)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (user.IdUsuario == equipo.IdUsuario)
            {
                await this.repo.InsertEntrenamiento(idequipo, nombre);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
            
        }


        // POST: api/Jugadores/AniadirJugadoresSesion/{identrenamiento}?idsjugador
        /// <summary>
        /// Empieza un entrenamiento y añade los ID de jugadores introducidos a la sesión
        /// </summary>
        /// <remarks>
        /// Empieza entrenamiento y añade jugadores a la sesion
        /// 
        /// - El ID de usuario, del equipo perteneciente al identrenamiento, debe ser igual al del usuario
        /// - No deben introducirse ID de jugadores repetidos
        /// - El equipo de los ID de jugadores introducidos, deben pertenecer al mismo ID de equipo perteneciente al entrenamiento
        /// 
        /// </remarks>
        /// <param name="idsjugador">ID de jugadores a incluir en el entrenamiento</param>
        /// <param name="identrenamiento">ID de entrenamiento a introducir</param>
        /// <response code="200">OK. Comienza el entrenamiento con los jugadores introducidos</response>
        /// <response code="400">ERROR: Solicitud mal introducida</response>
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpPost]
        [Route("[action]/{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AniadirJugadoresSesion([FromQuery] List<int> idsjugador, int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (entrena == null)
            {
                return BadRequest();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                //HAY QUE COMPROBAR QUE TODOS LOS IDSJUGADOR SEAN DISTINTOS
                if (HelperF2GTraining.HayRepetidos(idsjugador))
                {
                    return BadRequest(new
                    {
                        response = "Error: Hay IDS de jugador repetidos"
                    });
                }
                else
                {
                    List<Jugador> jugadoresseleccionados = new List<Jugador>();
                    foreach (int idjug in idsjugador)
                    {
                        jugadoresseleccionados.Add(await this.repo.GetJugadorID(idjug));
                    }

                    if (HelperF2GTraining.JugadoresEquipoCorrecto(jugadoresseleccionados, equipo.IdEquipo))
                    {
                        await this.repo.AniadirJugadoresSesion(idsjugador, identrenamiento);
                        await this.repo.EmpezarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            response = "Error: Un jugador introducido no pertenece al equipo del entrenamiento"
                        });
                    }

                }

            }
            else
            {
                return Unauthorized();
            }
        }

        // POST: api/Jugadores/AniadirPuntuacionesEntrenamiento/{identrenamiento}?idsjugador&valoraciones
        /// <summary>
        /// Finaliza un entrenamiento empezado y introduce las valoraciones a cada jugador
        /// </summary>
        /// <remarks>
        /// Finaliza entrenamiento y añade notas a cada jugador
        /// 
        /// - IMPORTANTE: Se ordenan los IDs de jugador, de menor a mayor (1,2,4..), las valoraciones no se ordenan
        /// - El ID de usuario, del equipo perteneciente al identrenamiento, debe ser igual al del usuario
        /// - No deben introducirse ID de jugadores repetidos
        /// - El entrenamiento debe haber sido empezado
        /// - Deben incluirse todos los ID de jugadores que estaban al empezar la sesion
        /// - Por cada ID jugador, deben introducirse 6 valoraciones entre 0 y 10. Su orden es:
        /// - 1º-RITMO O SALTO, 2º-TIRO O PARADA, 3º-PASE O SAQUE, 4º-REGATE O REFLEJOS, 5º-DEFENSA O VELOCIDAD DE REACCION, 6º-FISICO O POSICION
        /// 
        /// </remarks>
        /// <param name="idsjugador">ID de jugadores a incluir en el entrenamiento</param>
        /// <param name="valoraciones">Puntuaciones de cada jugador (6 por cada ID de jugador)</param>
        /// <param name="identrenamiento">ID de entrenamiento a introducir</param>
        /// <response code="200">OK. Devuelve los jugadores y las notas pertenecientes a ese entrenamiento</response>
        /// <response code="400">ERROR: Solicitud mal introducida</response>
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpPut]
        [Route("[action]/{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> AniadirPuntuacionesEntrenamiento([FromQuery] List<int> idsjugador, [FromQuery] List<int> valoraciones, int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            //Comprobamos que el entrenamiento exista, que este activo, y que el usuario haya pasado al menos 1 idjugador y 1 valoracion
            //Tambien comprobamos que no haya repetidos
            if (entrena == null)
            {
                return NotFound();
            }
            else if (idsjugador.Count() == 0 || valoraciones.Count() == 0)
            {
                return BadRequest(new
                {
                    response = "Error: Debes introducir ids de jugadores y valoraciones"
                });
            }
            else if (entrena.Activo != true)
            {
                return BadRequest(new
                {
                    response = "Error: El entrenamiento no esta activo"
                });
            }
            else if (HelperF2GTraining.HayRepetidos(idsjugador))
            {
                return BadRequest(new
                {
                    response = "Error: Hay IDS de jugador repetidos"
                });
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                //Recogemos los jugadores que estaban apuntados a la sesion
                List<Jugador> jugadoresentrena = await this.repo.JugadoresXSesion(identrenamiento);

                //Comprobamos que los ID sean iguales a los que estan registrados en el entrenamiento
                if (HelperF2GTraining.ComprobarIDJugadoresEntrena(idsjugador, jugadoresentrena))
                {
                    double comprobante = double.Parse(valoraciones.Count().ToString()) / double.Parse(idsjugador.Count().ToString());
                    if (comprobante != 6)
                    {
                        return BadRequest(new
                        {
                            response = "Error: Debes introducir 6 valoraciones entre 0 y 10 por cada ID jugador"
                        });
                    }
                    else
                    {
                        foreach (int val in valoraciones)
                        {
                            if (val > 10 || val < 0)
                            {
                                return BadRequest(new
                                {
                                    response = "Error: Las valoraciones deben encontrarse entre 0 y 10"
                                });
                            }
                        }

                        await this.repo.AniadirPuntuacionesEntrenamiento(idsjugador, valoraciones, identrenamiento);
                        await this.repo.FinalizarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                }
                else
                {
                    return BadRequest(new
                    {
                        response = "Error: No se han introducido los IDs de jugador pertenecientes a ese entrenamiento"
                    });
                }

            }
            else
            {
                return Unauthorized();
            }

        }

        // GET: api/Entrenamientos/GetEntrenamientosEquipo/{idequipo}
        /// <summary>
        /// Busca los entrenamientos, segun el ID de su equipo
        /// </summary>
        /// <remarks>
        /// Busca entrenamientos por ID de equipo
        /// </remarks>
        /// <param name="idequipo">Id del equipo.</param>
        /// <response code="200">OK. Devuelve los entrenamientos del equipo solicitado</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun equipo con ese ID</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Entrenamiento>>> GetEntrenamientosEquipo(int idequipo)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (equipo == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario && equipo != null)
            {
                return await this.repo.GetEntrenamientosEquipo(idequipo);
            }
            else
            {
                return Unauthorized();
            }
            
        }

        // GET: api/Entrenamientos/GetEntrenamiento/{idequipo}
        /// <summary>
        /// Busca el entrenamiento correspondiente a su ID de entrenamiento
        /// </summary>
        /// <remarks>
        /// Busca entrenamiento por su ID
        /// </remarks>
        /// <param name="identrena">Id del entrenamiento.</param>
        /// <response code="200">OK. Devuelve el entrenamiento solicitado</response>        
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun entrenamiento con ese ID</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]/{identrena}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Entrenamiento>> GetEntrenamiento(int identrena)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrena);

            if (entrena != null)
            {
                Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

                if (equipo.IdUsuario == user.IdUsuario)
                {
                    return entrena;
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }


        // GET: api/Jugadores/JugadoresXSesion/{identrenamiento}
        /// <summary>
        /// Devuelve los jugadores pertenecientes a un ID de entrenamiento de la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve los jugadores de usuario pertenecientes a un entrenamiento
        /// 
        /// - El ID de usuario, del equipo perteneciente al identrenamiento, debe ser igual al del usuario
        /// </remarks>
        /// <param name="identrenamiento">ID de entrenamiento a introducir</param>
        /// <response code="200">OK. Devuelve los jugadores pertenecientes a ese entrenamiento</response>
        /// <response code="404">ERROR: No se ha encontrado el entrenamiento con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<Jugador>>> JugadoresXSesion(int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            if (entrena == null)
            {
                return NotFound();
            }

            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (equipo == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario)
            {
                return await this.repo.JugadoresXSesion(identrenamiento);
            }
            else
            {
                return Unauthorized();
            }

        }

        // GET: api/Jugadores/GetNotasSesion/{identrenamiento}
        /// <summary>
        /// Devuelve los jugadores pertenecientes a un ID de entrenamiento, con sus notas,  de la BB.DD
        /// </summary>
        /// <remarks>
        /// Devuelve los jugadores de usuario, con sus notas, pertenecientes a un entrenamiento
        /// 
        /// - El ID de usuario, del equipo perteneciente al identrenamiento, debe ser igual al del usuario
        /// - El ID de entrenamiento introducido debe tener fecha de inicio y de fin
        /// 
        /// </remarks>
        /// <param name="identrenamiento">ID de entrenamiento a introducir</param>
        /// <response code="200">OK. Devuelve los jugadores y las notas pertenecientes a ese entrenamiento</response>
        /// <response code="400">ERROR: Solicitud mal introducida</response>
        /// <response code="404">ERROR: No se ha encontrado el entrenamiento con ese ID</response>  
        /// <response code="401">Credenciales incorrectas</response>
        [Authorize]
        [HttpGet]
        [Route("[action]/{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<List<JugadorEntrenamiento>>> GetNotasSesion(int identrenamiento)
        {
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            if (entrena == null)
            {
                return NotFound();
            }

            Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

            if (equipo == null)
            {
                return NotFound();
            }
            else if (user.IdUsuario == equipo.IdUsuario && entrena.FechaInicio != null && entrena.FechaFin != null)
            {
                return await this.repo.GetNotasSesion(identrenamiento);
            }
            else if (entrena.FechaInicio == null || entrena.FechaFin == null)
            {
                if (user.IdUsuario == equipo.IdUsuario)
                {
                    return BadRequest(new
                    {
                        response = "Error: El entrenamiento no esta finalizado"
                    });
                }
                else
                {
                    return Unauthorized();
                }

            }
            else
            {
                return Unauthorized();
            }
        }

        // DELETE: api/Entrenamientos/BorrarEntrenamiento/{idequipo}
        /// <summary>
        /// Borra el entrenamiento correspondiente a su ID de entrenamiento
        /// </summary>
        /// <remarks>
        /// Borra entrenamiento por su ID
        /// 
        /// - Para borrar un entrenamiento, no debe haber empezado
        /// </remarks>
        /// <param name="identrenamiento">Id del entrenamiento.</param>
        /// <response code="200">OK. Borra el entrenamiento solicitado</response>
        /// <response code="400">ERROR: Solicitud mal introducida</response>
        /// <response code="401">Debe entregar un token para realizar la solicitud</response>
        /// <response code="404">No se ha encontrado ningun entrenamiento con ese ID</response> 
        [Authorize]
        [HttpDelete("{identrenamiento}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> BorrarEntrenamiento(int identrenamiento)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));
            Entrenamiento entrena = await this.repo.GetEntrenamiento(identrenamiento);

            if (entrena != null)
            {
                Equipo equipo = await this.repo.GetEquipo(entrena.IdEquipo);

                if (equipo.IdUsuario == user.IdUsuario)
                {
                    if (entrena.FechaFin == null && entrena.FechaInicio == null)
                    {
                        await this.repo.BorrarEntrenamiento(identrenamiento);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            response = "El entrenamiento ya ha empezado o ha sido finalizado"
                        });
                    }
                    
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
            
        }

    }
}
