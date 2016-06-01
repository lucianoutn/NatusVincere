using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.NatusVincere.NVSkyBoxes
{
    public class NVSkyBox : TgcSkyBox
    {
        public void init()
        {
            
            Size = new Vector3(8000, 8000, 8000);
            string texturesPath = System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\NVSkyBoxes\";
            setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            SkyEpsilon = 1f;
            updateValues();
            
        } 
        

        //public TgcMesh[] faces;

        public void horario(string horario)
        {
            switch (horario)
            {
                case "maniana": this.Color = Color.Coral; break;
                case "dia": this.Color = Color.Transparent; break;
                case "tarde": this.Color = Color.DarkGoldenrod; break;
                case "noche": this.Color = Color.DarkBlue; break;
            }
        }


        public void cambiarHorario()
        {
            this.Color = Color.Red;
            //this.updateValues();
                 
            string horarioActual = Color.ToString();
            
            switch (horarioActual)
            {
                case "Coral": this.horario("dia"); break;
                case "Transparent": this.horario("tarde"); break;
                case "DarkGoldenrod": this.horario("noche"); break;
                case "DarkBlue": this.horario("maniana"); break;
            }
            
            
        }


        public void updateYRender(Vector3 pos)
        {
            this.Center = pos;
            this.updateValues();
            this.render();
        }


       }
}
