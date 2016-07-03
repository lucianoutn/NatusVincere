using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.NatusVincere
{
    class Wind
    {

        static Boolean vientoPlaying = false;

        static float vientoPlayedTime = 0;

        public static void generarViento(List<Crafteable> objetos, float elapsedTime, Sounds sounds)
        {
            vientoPlayedTime += elapsedTime;

            int i;
            if (!vientoPlaying)
            {
                for (i = 0; i < objetos.Count; i++)
                {
                    if (objetos[i].getType() == 1 || objetos[i].getType() == 3)
                    {
                        objetos[i].setEfecto("AlumnoEjemplos\\NatusVincere\\windShader.fx", "Viento");
                        objetos[i].getEfecto().SetValue("time", vientoPlayedTime);
                    }
                }
                vientoPlaying = true;
            }

            sounds.playViento();

            for (i = 0; i < objetos.Count; i++)
            {
                try
                {
                    objetos[i].getMesh().Effect.SetValue("time", vientoPlayedTime);
                }
                catch(Exception)
                {

                }
            }
        }
    }
}