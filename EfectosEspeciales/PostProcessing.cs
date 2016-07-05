using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    class PostProcessing
    {
        public static bool lluviaActivada = false;

        /// <summary>
        /// Se toma todo lo dibujado antes, que se guardo en una textura, y se combina con otra textura, que en este ejemplo
        /// es para generar un efecto de alarma.
        /// Se usa un shader para combinar ambas texturas y lograr el efecto de alarma.
        /// </summary>
        public static void drawPostProcess(Microsoft.DirectX.Direct3D.Device d3dDevice,
            Effect effect, VertexBuffer screenQuadVB, InterpoladorVaiven intVaivenAlarm,
            Texture renderTarget2D, TgcTexture lluviaTexture, float time)
        {
            //Arrancamos la escena
            d3dDevice.BeginScene();

            //Cargamos para renderizar el unico modelo que tenemos, un Quad que ocupa toda la pantalla, con la textura de todo lo dibujado antes
            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);
            
            if (lluviaActivada)
            {
                effect.Technique = "RainTechnique";
            }
            else
            {
                effect.Technique = "DefaultTechnique";
            }

            //Cargamos parametros en el shader de Post-Procesado
            effect.SetValue("render_target2D", renderTarget2D);
            effect.SetValue("textura_alarma", lluviaTexture.D3dTexture);
            effect.SetValue("alarmaScaleFactor", intVaivenAlarm.update());
            effect.SetValue("time", time);

            //Limiamos la pantalla y ejecutamos el render del shader
            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            //Terminamos el renderizado de la escena
            d3dDevice.EndScene();
        }
    }
}
