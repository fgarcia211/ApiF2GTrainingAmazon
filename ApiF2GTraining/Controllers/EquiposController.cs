using ApiF2GTraining.Helpers;
using ApiF2GTraining.Repositories;
using ModelsF2GTraining;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using ApiF2GTraining.Models;

namespace ApiF2GTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquiposController : ControllerBase
    {
        private IRepositoryF2GTraining repo;

        public EquiposController(IRepositoryF2GTraining repo)
        {
            this.repo = repo;
        }

        // POST: api/Equipos
        /// <summary>
        /// Inserta un equipo que introduzca el cliente segun su ID User, su nombre, y un archivo de imagen
        /// </summary>
        /// <remarks>
        /// Inserta equipos por su iduser, su nombre y su imagen
        /// </remarks>
        /// <response code="200">OK. Inserta el equipo</response>        
        /// <response code="401">Acceso no autorizado</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> InsertEquipo(EquipoModel model)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            //Enlace del blob en la imagen
            await this.repo.InsertEquipo(user.IdUsuario, model.nombre, model.imagen);
            return Ok();
        }

        // GET: api/Equipos/GetEquiposUser/{iduser}
        /// <summary>
        /// Busca los equipos, que coincidan con el ID de usuario
        /// </summary>
        /// <remarks>
        /// Busca equipos por ID de usuario
        /// </remarks>
        /// <response code="200">OK. Devuelve los equipos del usuario</response>        
        /// <response code="401">Acceso no autorizado</response>
        /// <response code="404">No se han encontrado equipos con ese ID de usuario</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Equipo>>> GetEquiposUser()
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            return await this.repo.GetEquiposUser(user.IdUsuario);
        }

        // GET: api/Equipos/GetEquipo/{idequipo}
        /// <summary>
        /// Devuelve el equipo con el ID solicitado
        /// </summary>
        /// <remarks>
        /// Busca equipo por ID de equipo
        /// </remarks>
        /// <param name="idequipo">Id del equipo.</param>
        /// <response code="200">OK. Devuelve el equipo con el ID solicitado</response>        
        /// <response code="401">Acceso no autorizado</response>
        /// <response code="404">No se ha encontrado ningun equipo con ese ID</response> 
        [Authorize]
        [HttpGet]
        [Route("[action]/{idequipo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Equipo>> GetEquipo(int idequipo)
        {
            Usuario user = HelperContextUser.GetUsuarioByClaim(HttpContext.User.Claims.SingleOrDefault(x => x.Type == "UserData"));

            Equipo equipo = await this.repo.GetEquipo(idequipo);

            if (equipo.IdUsuario == user.IdUsuario)
            {
                return equipo;
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}
