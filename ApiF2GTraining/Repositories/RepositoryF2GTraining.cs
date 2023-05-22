using System.Data;
using ApiF2GTraining.Data;
using ModelsF2GTraining;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

#region PROCEDURES USUARIOS

/*CREATE OR ALTER PROCEDURE SP_INSERT_USUARIO (@NOMBRE NVARCHAR(50),@CORREO NVARCHAR(100), @CONTRASENIA NVARCHAR(50), @TELEFONO INT)
AS
	INSERT INTO USUARIOS VALUES ((SELECT ISNULL(MAX(ID),0) FROM USUARIOS)+1,@NOMBRE,@CORREO,@CONTRASENIA,@TELEFONO,NULL)
GO

CREATE OR ALTER PROCEDURE SP_FIND_USUARIO (@NOMBRE NVARCHAR(50), @CONTRASENIA NVARCHAR(50))
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE NOM_USUARIO = @NOMBRE AND CONTRASENIA = @CONTRASENIA
GO

CREATE OR ALTER PROCEDURE SP_FIND_TOKEN (@TOKEN NVARCHAR(100))
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE TOKEN = @TOKEN
GO

CREATE OR ALTER PROCEDURE SP_FIND_NOM_USUARIO (@NOMBRE NVARCHAR(50))
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE NOM_USUARIO = @NOMBRE
GO

CREATE OR ALTER PROCEDURE SP_FIND_CORREO (@CORREO NVARCHAR(50))
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE CORREO = @CORREO
GO

CREATE OR ALTER PROCEDURE SP_FIND_TELEFONO (@TELEFONO INT)
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE TELEFONO = @TELEFONO
GO

CREATE OR ALTER PROCEDURE SP_FIND_TOKEN (@TOKEN NVARCHAR(100))
AS
	SELECT ID,NOM_USUARIO,CORREO,CONTRASENIA,TELEFONO,ISNULL(TOKEN,'SIN TOKEN') AS TOKEN FROM USUARIOS
	WHERE TOKEN = @TOKEN
GO

CREATE OR ALTER PROCEDURE SP_UPDATE_TOKEN (@OLDTOKEN NVARCHAR(100), @NEWTOKEN NVARCHAR(100))
AS
	UPDATE USUARIOS SET TOKEN = @NEWTOKEN WHERE TOKEN = @OLDTOKEN
GO*/

#endregion

#region PROCEDURES EQUIPOS

/*CREATE OR ALTER PROCEDURE SP_INSERT_EQUIPO (@IDUSER INT, @NOMBRE NVARCHAR(50),@IMAGEN NVARCHAR(1000))
AS
	INSERT INTO EQUIPOS VALUES ((SELECT ISNULL(MAX(ID),0) FROM EQUIPOS)+1,@IDUSER,@NOMBRE,@IMAGEN)
GO

CREATE OR ALTER PROCEDURE SP_FIND_EQUIPOS_USER (@IDUSER INT)
AS
	SELECT * FROM EQUIPOS
	WHERE IDUSUARIO = @IDUSER
GO

CREATE OR ALTER PROCEDURE SP_FIND_EQUIPO_ID (@IDEQUIPO INT)
AS
	SELECT * FROM EQUIPOS
	WHERE ID = @IDEQUIPO
GO*/
#endregion

#region PROCEDURES JUGADORES

/*CREATE OR ALTER PROCEDURE SP_INSERT_JUGADOR (@IDEQUIPO INT, @IDPOSICION INT, @NOMBRE NVARCHAR(100), @DORSAL INT, @EDAD INT, @PESO INT, @ALTURA INT)
AS
    INSERT INTO JUGADORES VALUES (
	(SELECT ISNULL(MAX(ID),0) FROM JUGADORES)+1,@IDEQUIPO,@IDPOSICION,@NOMBRE,@DORSAL,@EDAD,@PESO,@ALTURA)

	INSERT INTO ESTADISTICAS VALUES 
	((SELECT ISNULL(MAX(ID),0) FROM ESTADISTICAS)+1,(SELECT ISNULL(MAX(ID),0) FROM JUGADORES),NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL)
GO

CREATE OR ALTER PROCEDURE SP_FIND_JUGADOR_ID (@IDJUGADOR INT)
AS
	SELECT * FROM JUGADORES
	WHERE ID = @IDJUGADOR
GO

CREATE OR ALTER PROCEDURE SP_FIND_JUGADORES_IDEQUIPO (@IDEQUIPO INT)
AS
	SELECT * FROM JUGADORES
	WHERE IDEQUIPO = @IDEQUIPO
GO

CREATE OR ALTER PROCEDURE SP_DELETE_JUGADOR_ID (@IDJUGADOR INT)
AS
	DELETE FROM JUGADORES_ENTRENAMIENTO
	WHERE IDJUGADOR = @IDJUGADOR

	DELETE FROM ESTADISTICAS
	WHERE IDJUGADOR = @IDJUGADOR

    DELETE FROM JUGADORES
	WHERE ID = @IDJUGADOR
GO

CREATE OR ALTER PROCEDURE SP_FIND_POSITIONS
AS
	SELECT * FROM POSICIONES
GO*/
#endregion

#region PROCEDURES ENTRENAMIENTOS

/*CREATE OR ALTER PROCEDURE SP_INSERTAR_ENTRENAMIENTO (@IDEQUIPO INT, @NOMBRE NVARCHAR(100))
AS
	INSERT INTO ENTRENAMIENTOS VALUES ((SELECT ISNULL(MAX(ID),0) FROM ENTRENAMIENTOS)+1,@IDEQUIPO,NULL,NULL,0,@NOMBRE)
GO

CREATE OR ALTER PROCEDURE SP_ENTRENAMIENTOS_EQUIPO(@IDEQUIPO INT)
AS
	SELECT * FROM ENTRENAMIENTOS
	WHERE IDEQUIPO = @IDEQUIPO
	ORDER BY ID DESC
GO

CREATE OR ALTER PROCEDURE SP_BUSCAR_ENTRENAMIENTO (@IDENTRENAMIENTO INT)
AS
	SELECT * FROM ENTRENAMIENTOS
	WHERE ID = @IDENTRENAMIENTO
GO

CREATE OR ALTER PROCEDURE SP_EMPEZAR_ENTRENAMIENTO (@IDENTRENAMIENTO INT, @FECHAINICIO DATETIME)
AS
	UPDATE ENTRENAMIENTOS SET ACTIVO = 1, FECHA_INICIO=@FECHAINICIO 
	WHERE ID = @IDENTRENAMIENTO
GO

CREATE OR ALTER PROCEDURE SP_FINALIZAR_ENTRENAMIENTO (@IDENTRENAMIENTO INT, @FECHAFIN DATETIME)
AS
	UPDATE ENTRENAMIENTOS SET ACTIVO = 0, FECHA_FIN=@FECHAFIN 
	WHERE ID = @IDENTRENAMIENTO
GO

CREATE OR ALTER PROCEDURE SP_BORRAR_ENTRENAMIENTO (@IDENTRENAMIENTO INT)
AS
	DELETE FROM ENTRENAMIENTOS
	WHERE ID = @IDENTRENAMIENTO
GO
*/

#endregion

#region VISTAS

/*CREATE VIEW V_ESTADISTICAS_JUGADOR
AS
    SELECT JUG.NOMBRE, EST.* FROM ESTADISTICAS EST INNER JOIN JUGADORES JUG ON EST.IDJUGADOR = JUG.ID
GO*/

#endregion

namespace ApiF2GTraining.Repositories
{
    public class RepositoryF2GTraining : IRepositoryF2GTraining
    {
		private F2GDataBaseContext context;
		public RepositoryF2GTraining(F2GDataBaseContext context)
		{
			this.context = context;
		}

        #region METODOSUSUARIO
        public int InsertIdUser()
        {
            if (this.context.Usuarios.Count() > 0)
            {
                return this.context.Usuarios.Max(x => x.IdUsuario) + 1;
            }
            else
            {
                return 1;
            }
        }
        public async Task InsertUsuario(Usuario user)
        {
            user.IdUsuario = this.InsertIdUser();
            user.Token = "SIN TOKEN";

            this.context.Usuarios.Add(user);
            await this.context.SaveChangesAsync();

        }

        public async Task<Usuario> GetUsuarioNamePass(string nombre, string contrasenia)
        {
            var consulta = await this.context.Usuarios.Where(x => x.Nombre == nombre && x.Contrasenia == contrasenia).ToListAsync();
            Usuario user = consulta.FirstOrDefault();
            return user;
        }

        public async Task<bool> CheckTelefonoRegistro(int telefono)
        {
            var consulta = await this.context.Usuarios.Where(x => x.Telefono == telefono).ToListAsync();
            Usuario user = consulta.FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public async Task<bool> CheckUsuarioRegistro(string nombre)
        {
            var consulta = await this.context.Usuarios.Where(x => x.Nombre == nombre).ToListAsync();
            Usuario user = consulta.FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public async Task<bool> CheckCorreoRegistro(string correo)
        {
            var consulta = await this.context.Usuarios.Where(x => x.Correo == correo).ToListAsync();
            Usuario user = consulta.FirstOrDefault();

            if (user == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        #endregion

        #region METODOSEQUIPOS
        public int InsertIdEquipo()
        {
            if (this.context.Equipos.Count() > 0)
            {
                return this.context.Equipos.Max(x => x.IdEquipo) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task InsertEquipo(int iduser, string nombre, string imagen)
        {
            Equipo equipo = new Equipo
            {
                IdEquipo = this.InsertIdEquipo(),
                IdUsuario = iduser,
                Nombre = nombre,
                Imagen = imagen
            };

            this.context.Equipos.Add(equipo);
            await this.context.SaveChangesAsync();

        }

        public async Task<List<Equipo>> GetEquiposUser(int iduser)
        {
            List<Equipo> equiposusuario = await this.context.Equipos.Where(x => x.IdUsuario == iduser).ToListAsync();
            return equiposusuario;
        }

        public async Task<Equipo> GetEquipo(int idequipo)
        {
            var consulta = await this.context.Equipos.Where(x => x.IdEquipo == idequipo).ToListAsync();
            Equipo equipo = consulta.AsEnumerable().FirstOrDefault();
            return equipo;
        }

        #endregion

        #region METODOSJUGADORES

        public int InsertIdJugador()
        {
            if (this.context.Jugadores.Count() > 0)
            {
                return this.context.Jugadores.Max(x => x.IdJugador) + 1;
            }
            else
            {
                return 1;
            }
        }

        public int InsertIdEstadisticas()
        {
            if (this.context.EstadisticasJugadores.Count() > 0)
            {
                return this.context.EstadisticasJugadores.Max(x => x.Id) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task InsertJugador(int idequipo, int idposicion, string nombre, int dorsal, int edad, int peso, int altura)
        {
            this.context.Jugadores.Add(new Jugador
            {
                IdJugador = this.InsertIdJugador(),
                IdEquipo = idequipo,
                IdPosicion = idposicion,
                Nombre = nombre,
                Dorsal = dorsal,
                Edad = edad,
                Peso = peso,
                Altura = altura
            });
            await this.context.SaveChangesAsync();

            this.context.EstadisticasJugadores.Add(new EstadisticaJugador
            {
                Id = this.InsertIdEstadisticas(),
                IdJugador = this.context.Jugadores.Max(x => x.IdJugador),
                TotalDefensaGKVelocidad = null,
                TotalFisicoGKPosicion = null,
                TotalPaseGKSaque = null,
                TotalRegateGKReflejo = null,
                TotalRitmoGKSalto = null,
                TotalTiroGKParada = null,
                DefensaGKVelocidad = null,
                FisicoGKPosicion = null,
                PaseGKSaque = null,
                RegateGKReflejo = null,
                RitmoGKSalto = null,
                TiroGKParada = null
            });

            await this.context.SaveChangesAsync();

        }

        public async Task<List<Posicion>> GetPosiciones()
        {
            List<Posicion> posiciones = await this.context.Posiciones.ToListAsync();
            return posiciones;

        }

        public async Task<Jugador> GetJugadorID(int id)
        {
            var consulta = await this.context.Jugadores.Where(x => x.IdJugador == id).ToListAsync();
            Jugador player = consulta.AsEnumerable().FirstOrDefault();
            return player;
        }

        public async Task<EstadisticaJugador> GetEstadisticasJugador(int id)
        {
            return await this.context.EstadisticasJugadores.Where(x => x.IdJugador == id).FirstOrDefaultAsync();
        }

        public async Task<List<Jugador>> GetJugadoresEquipo(int idequipo)
        {
            List<Jugador> players = await this.context.Jugadores.Where(x => x.IdEquipo == idequipo).ToListAsync();
            return players;
        }

        public async Task DeleteJugador(int idjugador)
        {
            List<JugadorEntrenamiento> jugentrena = await this.context.JugadoresEntrenamiento.Where(x => x.IdJugador == idjugador).ToListAsync();

            foreach (JugadorEntrenamiento je in jugentrena)
            {
                this.context.JugadoresEntrenamiento.Remove(je);
            }

            await this.context.SaveChangesAsync();

            EstadisticaJugador estadist = await this.context.EstadisticasJugadores.Where(x => x.IdJugador == idjugador).FirstOrDefaultAsync();
            this.context.EstadisticasJugadores.Remove(estadist);
            await this.context.SaveChangesAsync();

            Jugador jug = await this.context.Jugadores.Where(x => x.IdJugador == idjugador).FirstOrDefaultAsync();
            this.context.Jugadores.Remove(jug);
            await this.context.SaveChangesAsync();

        }

        public async Task<List<Jugador>> JugadoresXUsuario(int idusuario)
        {
            List<Equipo> equipos = await this.GetEquiposUser(idusuario);

            List<int> idsEquipos = new();

            foreach (Equipo equipo in equipos)
            {
                idsEquipos.Add(equipo.IdEquipo);
            }

            var consulta = from datos in this.context.Jugadores
                           where idsEquipos.Contains(datos.IdEquipo)
                           select datos;


            if (consulta.Count() == 0)
            {
                return null;
            }

            return await consulta.ToListAsync();
        }

        public async Task<List<Jugador>> JugadoresXSesion(int identrenamiento)
        {
            var consulta = (from datos in this.context.JugadoresEntrenamiento
                            where identrenamiento == datos.IdEntrenamiento
                            select datos.IdJugador);

            List<Jugador> jugadores = await this.context.Jugadores.Where(x => consulta.Contains(x.IdJugador)).ToListAsync();

            return jugadores;
        }

        public async Task AniadirPuntuacionesEntrenamiento(List<int> idsjugador, List<int> valoraciones, int identrenamiento)
        {
            var contadorPuntuacion = 0;

            foreach (int id in idsjugador)
            {
                JugadorEntrenamiento jugador =
                    this.context.JugadoresEntrenamiento.Where(x => x.IdJugador == id && x.IdEntrenamiento == identrenamiento).First();

                EstadisticaJugador estadisticas =
                    this.context.EstadisticasJugadores.Where(x => x.IdJugador == id).First();

                int ritmosalto = valoraciones[contadorPuntuacion];
                int tiroparada = valoraciones[contadorPuntuacion + 1];
                int pasesaque = valoraciones[contadorPuntuacion + 2];
                int regatereflejo = valoraciones[contadorPuntuacion + 3];
                int defensavelocidad = valoraciones[contadorPuntuacion + 4];
                int fisicoposicion = valoraciones[contadorPuntuacion + 5];

                jugador.RitmoGKSalto = ritmosalto;

                if (ritmosalto != 0)
                {
                    if (estadisticas.RitmoGKSalto == null && estadisticas.TotalRitmoGKSalto == null)
                    {
                        estadisticas.RitmoGKSalto = ritmosalto;
                        estadisticas.TotalRitmoGKSalto = 1;
                    }
                    else
                    {
                        estadisticas.RitmoGKSalto = estadisticas.RitmoGKSalto + ritmosalto;
                        estadisticas.TotalRitmoGKSalto++;
                    }

                }

                jugador.TiroGKParada = tiroparada;

                if (tiroparada != 0)
                {
                    if (estadisticas.TiroGKParada == null && estadisticas.TotalTiroGKParada == null)
                    {
                        estadisticas.TiroGKParada = tiroparada;
                        estadisticas.TotalTiroGKParada = 1;
                    }
                    else
                    {
                        estadisticas.TiroGKParada = estadisticas.TiroGKParada + tiroparada;
                        estadisticas.TotalTiroGKParada++;
                    }

                }

                jugador.PaseGKSaque = pasesaque;

                if (pasesaque != 0)
                {
                    if (estadisticas.PaseGKSaque == null && estadisticas.TotalPaseGKSaque == null)
                    {
                        estadisticas.PaseGKSaque = pasesaque;
                        estadisticas.TotalPaseGKSaque = 1;
                    }
                    else
                    {
                        estadisticas.PaseGKSaque = estadisticas.PaseGKSaque + pasesaque;
                        estadisticas.TotalPaseGKSaque++;
                    }

                }

                jugador.RegateGKReflejo = regatereflejo;

                if (regatereflejo != 0)
                {
                    if (estadisticas.RegateGKReflejo == null && estadisticas.TotalRegateGKReflejo == null)
                    {
                        estadisticas.RegateGKReflejo = regatereflejo;
                        estadisticas.TotalRegateGKReflejo = 1;
                    }
                    else
                    {
                        estadisticas.RegateGKReflejo = estadisticas.RegateGKReflejo + regatereflejo;
                        estadisticas.TotalRegateGKReflejo++;
                    }

                }

                jugador.DefensaGKVelocidad = defensavelocidad;

                if (defensavelocidad != 0)
                {
                    if (estadisticas.DefensaGKVelocidad == null && estadisticas.TotalDefensaGKVelocidad == null)
                    {
                        estadisticas.DefensaGKVelocidad = defensavelocidad;
                        estadisticas.TotalDefensaGKVelocidad = 1;
                    }
                    else
                    {
                        estadisticas.DefensaGKVelocidad = estadisticas.DefensaGKVelocidad + defensavelocidad;
                        estadisticas.TotalDefensaGKVelocidad++;
                    }

                }

                jugador.FisicoGKPosicion = fisicoposicion;

                if (fisicoposicion != 0)
                {
                    if (estadisticas.FisicoGKPosicion == null && estadisticas.TotalFisicoGKPosicion == null)
                    {
                        estadisticas.FisicoGKPosicion = fisicoposicion;
                        estadisticas.TotalFisicoGKPosicion = 1;
                    }
                    else
                    {
                        estadisticas.FisicoGKPosicion = estadisticas.FisicoGKPosicion + fisicoposicion;
                        estadisticas.TotalFisicoGKPosicion++;
                    }

                }

                jugador.Finalizado = true;
                contadorPuntuacion += 6;
            }

            await this.context.SaveChangesAsync();
        }

        public async Task AniadirJugadoresSesion(List<int> idsjugador, int identrenamiento)
        {
            List<Jugador> jugadores = this.context.Jugadores.Where(x => idsjugador.Contains(x.IdJugador)).ToList();
            int id = this.context.JugadoresEntrenamiento.Count();

            if (id == 0)
            {
                id = 1;
            }
            else
            {
                id = this.context.JugadoresEntrenamiento.Max(x => x.Id) + 1;
            }

            foreach (Jugador jug in jugadores)
            {
                JugadorEntrenamiento jugentre = new JugadorEntrenamiento
                {
                    Id = id,
                    IdJugador = jug.IdJugador,
                    IdEntrenamiento = identrenamiento,
                    RitmoGKSalto = null,
                    TiroGKParada = null,
                    PaseGKSaque = null,
                    RegateGKReflejo = null,
                    DefensaGKVelocidad = null,
                    FisicoGKPosicion = null,
                    Finalizado = false
                };

                this.context.JugadoresEntrenamiento.Add(jugentre);
                id++;

            }

            await this.context.SaveChangesAsync();
        }

        public async Task<List<JugadorEntrenamiento>> GetNotasSesion(int identrenamiento)
        {
            return await this.context.JugadoresEntrenamiento.Where(x => x.IdEntrenamiento == identrenamiento).ToListAsync();
        }
        #endregion

        #region METODOSENTRENAMIENTOS
        public int InsertIdEntrena()
        {
            if (this.context.Entrenamientos.Count() > 0)
            {
                return this.context.Entrenamientos.Max(x => x.Id) + 1;
            }
            else
            {
                return 1;
            }
        }

        public async Task InsertEntrenamiento(int idequipo, string nombre)
        {
            Entrenamiento entrenamiento = new Entrenamiento
            {
                Id = this.InsertIdEntrena(),
                IdEquipo = idequipo,
                FechaInicio = null,
                FechaFin = null,
                Activo = false,
                Nombre = nombre
            };

            this.context.Entrenamientos.Add(entrenamiento);
            await this.context.SaveChangesAsync();

        }

        public async Task<List<Entrenamiento>> GetEntrenamientosEquipo(int idequipo)
        {
            List<Entrenamiento> entrenamientos = await this.context.Entrenamientos.Where(x => x.IdEquipo == idequipo).ToListAsync();
            return entrenamientos;
        }

        public async Task<Entrenamiento> GetEntrenamiento(int identrena)
        {
            var consulta = await this.context.Entrenamientos.Where(x => x.Id == identrena).ToListAsync();
            Entrenamiento entrenamiento = consulta.AsEnumerable().FirstOrDefault();
            return entrenamiento;
        }

        public async Task EmpezarEntrenamiento(int identrenamiento)
        {
            Entrenamiento entrena = await this.context.Entrenamientos.Where(x => x.Id == identrenamiento).FirstOrDefaultAsync();
            entrena.FechaInicio = Convert.ToDateTime(DateTime.Now);
            entrena.Activo = true;
            await this.context.SaveChangesAsync();

        }

        public async Task FinalizarEntrenamiento(int identrenamiento)
        {
            Entrenamiento entrena = await this.context.Entrenamientos.Where(x => x.Id == identrenamiento).FirstOrDefaultAsync();
            entrena.FechaFin = Convert.ToDateTime(DateTime.Now);
            entrena.Activo = false;
            await this.context.SaveChangesAsync();
        }

        public async Task BorrarEntrenamiento(int identrenamiento)
        {
            Entrenamiento entrena = await this.context.Entrenamientos.Where(x => x.Id == identrenamiento).FirstOrDefaultAsync();
            this.context.Entrenamientos.Remove(entrena);
            await this.context.SaveChangesAsync();

        }
        #endregion
    }
}
