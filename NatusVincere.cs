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
        TgcMesh pasto;
        TgcSkyBox skyBox;

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
            
            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 500, 0);
            skyBox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\SkyBox1\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "phobos_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "phobos_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "phobos_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "phobos_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "phobos_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "phobos_ft.jpg");
            skyBox.updateValues();

            //Crear suelo
            //TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\pasto.jpg");
            TgcTexture pisoTexture = TgcTexture.createTexture(d3dDevice, System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\pasto.jpg");
            suelo = TgcBox.fromSize(new Vector3(980, 69, 1980), new Vector3(3000, 0, 4900), pisoTexture);

            //Cargar modelo de palmera original
            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            palmeraOriginal = scene.Meshes[0];

            //Cargar pasto
            scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\Planta\Planta-TgcScene.xml");
            pasto = scene.Meshes[0];

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
                        offset = offset + 80;
                    }
                    else
                    {
                        offset = offset + 95;
                    }
                    //Crear instancia de modelo
                    TgcMesh instance = palmeraOriginal.createMeshInstance(palmeraOriginal.Name + i + "_" + j);

                    //Desplazarlo
                    instance.move(i * offset - 190, 70, j * offset - 180);
                    //Tamaño de os arboles
                    instance.Scale = new Vector3(0.75f, 1.45f, 0.75f);

                    meshes.Add(instance);

                }
            }

            offset = 200;

            for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((j % 2 != 0) && (i % 2 == 0))
                        {
                            offset = offset + 80;
                        }
                        else
                        {
                            offset = offset + 95;
                        }
                        //Crear instancia de modelo
                        TgcMesh instance = pasto.createMeshInstance(palmeraOriginal.Name + i + "_" + j);

                        //Desplazarlo
                        instance.move(i * 590, 70, j * 490);
                        //Tamaño de os arboles
                        instance.Scale = new Vector3(0.75f, 0.75f, 0.75f);

                        meshes.Add(instance);

                    }
                }


            //Camara en primera persona
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.MovementSpeed = 400;
            GuiController.Instance.FpsCamera.JumpSpeed = 400;
            GuiController.Instance.FpsCamera.setCamera(new Vector3(-200, 140, -10), new Vector3(379.7143f, 100, 336.3295f));

        }
        
        public override void render(float elapsedTime)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Renderizar suelo
            suelo.render();
            skyBox.render();

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
            pasto.dispose();
            skyBox.dispose();
        }
    }
}
