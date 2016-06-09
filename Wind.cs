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
        static List<Crafteable> lastObjetos = null;
        static float vientoPlayedTime = 0;
        
        public static void generarViento(List<Crafteable> objetos, float elapsedTime, Sounds sounds)
        {

            vientoPlayedTime += elapsedTime;

            
            if (vientoPlaying) {
                sounds.playViento();
                objetos.ForEach(crafteable => {
                    crafteable.getMesh().Effect.SetValue("time", vientoPlayedTime);
                });
            }
        }

        public static void initialize(List<Crafteable> objetos)
        {
            objetos.ForEach(crafteable =>
            {
                crafteable.getMesh().Effect = TgcShaders.loadEffect("AlumnoEjemplos\\NatusVincere\\windShader.fx");
                crafteable.getMesh().Technique = "Viento";
                crafteable.getMesh().Effect.SetValue("time", vientoPlayedTime);
            });
        }
    }
}
