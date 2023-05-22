using ModelsF2GTraining;
using Microsoft.EntityFrameworkCore;

namespace ApiF2GTraining.Data
{
    public class F2GDataBaseContext : DbContext
    {
        public F2GDataBaseContext(DbContextOptions<F2GDataBaseContext> options) :
            base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Equipo> Equipos { get; set; }

        public DbSet<Jugador> Jugadores { get; set; }

        public DbSet<Posicion> Posiciones { get; set; }

        public DbSet<Entrenamiento> Entrenamientos { get; set; }

        public DbSet<JugadorEntrenamiento> JugadoresEntrenamiento { get; set; }

        public DbSet<EstadisticaJugador> EstadisticasJugadores { get; set; }
    }
}
