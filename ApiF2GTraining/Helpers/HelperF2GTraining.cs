using ApiF2GTraining.Repositories;
using ModelsF2GTraining;

namespace ApiF2GTraining.Helpers
{
    public static class HelperF2GTraining
    {
        public static bool HayRepetidos(List<int> listanum)
        {
            List<int> listaaux = new List<int>();

            foreach (int num in listanum)
            {
                if (listaaux.Contains(num))
                {
                    return true;
                }
                else
                {
                    listaaux.Add(num);
                }
            }

            return false;
        }

        public static bool ComprobarIDJugadoresEntrena(List<int> idsjugador, List<Jugador> jugadores)
        {
            if (idsjugador.Count() != jugadores.Count())
            {
                return false;
            }

            List<int> idsjugapuntados = new List<int>();
            foreach (Jugador j in jugadores)
            {
                idsjugapuntados.Add(j.IdJugador);
            }

            idsjugapuntados.Sort();
            idsjugador.Sort();

            for (int i=0; i < idsjugador.Count; i++)
            {
                if (idsjugador[i] != idsjugapuntados[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool JugadoresEquipoCorrecto(List<Jugador> jugadores, int idequipo)
        {
            foreach (Jugador j in jugadores)
            {
                if (j.IdEquipo != idequipo)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool PosicionCorrecta(List<Posicion> posiciones, int idposicion)
        {
            List<int> idsposiciones = new List<int>();

            foreach (Posicion p in posiciones)
            {
                idsposiciones.Add(p.IdPosicion);
            }

            if (idsposiciones.Contains(idposicion))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
