using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils;
using System.Drawing;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;

namespace AlumnoEjemplos.NatusVincere.NVSkyBoxes
{
    public class NVSkyBox : TgcSkyBox
    {
        /*public NVSkyBox()
        {
           new TgcSkyBox();
           // faces = new TgcMesh[6];
            //faceTextures = new string[6];
            //skyEpsilon = 5f;
            //color = Color.White;
            //center = new Vector3(0,0,0);
            //size = new Vector3(1000, 1000, 1000);
            //alphaBlendEnable = false;
        } 
        */

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

       /* public void updateSkyboxEnRender(Vector3 pos)
        {
            foreach (TgcMesh face in faces)
            {
                face.Transform = Matrix.Translation(pos.X, pos.Y, pos.Z);
                face.render();
            }
        
        }*/

        public void updateYRender(Vector3 pos)
        {
            foreach (TgcMesh face in base.Faces)
            {
                face.AutoTransformEnable = false;
                face.Transform = Matrix.Translation(pos.X, pos.Y, pos.Z);
                //face.move(pos.X, pos.Y, pos.Z);
                //this.Center = pos;
                face.render();
                
            }
        }


       }
    }
