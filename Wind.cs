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
                    objetos[i].getMesh().Effect = TgcShaders.loadEffect("AlumnoEjemplos\\NatusVincere\\windShader.fx");
                    objetos[i].getMesh().Technique = "Viento";
                    objetos[i].getMesh().Effect.SetValue("time", vientoPlayedTime);
                }
                vientoPlaying = true;
                return;
            }

            sounds.playViento();

            for (i = 0; i < objetos.Count; i++)
            {
                objetos[i].getMesh().Effect.SetValue("time", vientoPlayedTime);
            }
        }
    }
}