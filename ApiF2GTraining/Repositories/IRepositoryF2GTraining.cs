using ModelsF2GTraining;

namespace ApiF2GTraining.Repositories
{
    public interface IRepositoryF2GTraining
    {
        #region METODOSUSUARIOS
        Task InsertUsuario(Usuario user);

        Task<Usuario> GetUsuarioNamePass(string nombre, string contrasenia);

        Task<bool> CheckTelefonoRegistro(int telefono);

        Task<bool> CheckUsuarioRegistro(string nombre);

        Task<bool> CheckCorreoRegistro(string correo);

        #endregion

        #region METODOSEQUIPOS
        Task InsertEquipo(int iduser, string nombre, string imagen);

        Task<List<Equipo>> GetEquiposUser(int iduser);

        Task<Equipo> GetEquipo(int idequipo);

        #endregion

        #region METODOSJUGADORES
        Task InsertJugador(int idequipo, int idposicion, string nombre, int dorsal, int edad, int peso, int altura);

        Task <List<Posicion>> GetPosiciones();

        Task<Jugador> GetJugadorID(int id);

        Task<EstadisticaJugador> GetEstadisticasJugador(int id);

        Task<List<Jugador>> GetJugadoresEquipo(int idequipo);

        Task DeleteJugador(int idjugador);

        Task<List<Jugador>> JugadoresXUsuario(int idusuario);

        Task<List<Jugador>> JugadoresXSesion(int identrenamiento);

        Task AniadirPuntuacionesEntrenamiento(List<int> idsjugador, List<int> valoraciones, int identrenamiento);

        Task AniadirJugadoresSesion(List<int> idsjugador, int identrenamiento);

        Task<List<JugadorEntrenamiento>> GetNotasSesion(int identrenamiento);

        #endregion

        #region METODOSENTRENAMIENTOS
        Task InsertEntrenamiento(int idequipo, string nombre);

        Task<List<Entrenamiento>> GetEntrenamientosEquipo(int idequipo);

        Task<Entrenamiento> GetEntrenamiento(int identrena);

        Task EmpezarEntrenamiento(int identrenamiento);

        Task FinalizarEntrenamiento(int identrenamiento);

        Task BorrarEntrenamiento(int identrenamiento);
        #endregion
    }
}
