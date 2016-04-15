using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Terrain;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        TgcBox suelo;
        List<TgcMesh> meshes;
        TgcMesh palmeraOriginal;

        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }
        
        public override string getName()
        {
            return "NatusVincere";
        }
        
        public override string getDescription()
        {
            return "Survival Craft – Supervivencia con creaciones.";
        }

        public override void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Crear suelo
            //TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\pasto.jpg");
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\pasto.jpg");
            suelo = TgcBox.fromSize(new Vector3(300, 69, 400), new Vector3(1250, 0, 1250), pisoTexture);

            //Cargar modelo de palmera original
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            palmeraOriginal = scene.Meshes[0];

            //Crear varias instancias del modelo original, pero sin volver a cargar el modelo entero cada vez
            int rows = 3;
            int cols = 4;
            float offset = 200;
            meshes = new List<TgcMesh>();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((j % 2 != 0) && (i % 2 == 0))
                    {
                        offset = offset + 4;
                    }
                    else
                    {
                        offset = offset + 5;
                    }
                    //Crear instancia de modelo
                    TgcMesh instance = palmeraOriginal.createMeshInstance(palmeraOriginal.Name + i + "_" + j);

                    //Desplazarlo
                    instance.move(i * 190, 70, j * offset - 180);
                    instance.Scale = new Vector3(0.25f, 0.25f, 0.25f);

                    meshes.Add(instance);

                }
            }


            //Camara en primera persona
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 400;
            GuiController.Instance.FpsCamera.JumpSpeed = 400;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(61.8657f, 403.7024f, -527.558f), new Vector3(379.7143f, 12.9713f, 336.3295f));

        }
        
        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Renderizar suelo
            suelo.render();

            //Renderizar instancias
            foreach (TgcMesh mesh in meshes)
            {
                mesh.render();
            }
        }

        public override void close()
        {
            suelo.dispose();

            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            palmeraOriginal.dispose();
        }
    }
}
