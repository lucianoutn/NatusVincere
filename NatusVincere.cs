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
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System.IO;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        const float MOVEMENT_SPEED = 200f;
        TgcBox suelo;
        List<Crafteable> objects;
        TgcMesh palmeraOriginal;
        TgcMesh pasto;
        TgcSkyBox skyBox;
        Human personaje;

        ObjectsFactory objectsFactory;

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
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcSceneLoader loader = new TgcSceneLoader();
            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);
            
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
            personaje = objectsFactory.createHuman(suelo.Position + new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            //Cargar modelo de palmera original
            TgcScene scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            palmeraOriginal = scene.Meshes[0];

            //Cargar pasto
            scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\Planta\Planta-TgcScene.xml");
            pasto = scene.Meshes[0];

            //Camara en primera persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(personaje.getPosition(), 200, 400);
            objects.Add(objectsFactory.createArbol(suelo.Position + new Vector3(30, 1, 0), new Vector3(0.75f, 0.75f, 0.75f)));
            objects.Add(objectsFactory.createHacha(suelo.Position + new Vector3(200, 1, 0), new Vector3(10, 10, 10)));
            objects.Add(objectsFactory.createPiedra(suelo.Position + new Vector3(100, 1, 0), new Vector3(0.75f, 0.75f, 0.75f)));
        }
        
        public override void render(float elapsedTime)
        {
            float velocidadCaminar = 5f;
            float velocidadRotacion = 100f;
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;

            String animation = "Walk";
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Jump
            if (d3dInput.keyDown(Key.Space))
            {
                jump = 30;
                moving = true;
            }



            if (d3dInput.keyDown(Key.E))
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) crafteable.use(personaje); });
            }

            if (d3dInput.keyDown(Key.R))
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) personaje.store(crafteable); });

            }

            if (d3dInput.keyDown(Key.L))
            {
                personaje.leaveObject();
            }


                //Si hubo rotacion
                if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                personaje.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Sin(personaje.getRotation().Y) * moveForward,
                    jump,
                    FastMath.Cos(personaje.getRotation().Y) * moveForward
                    );
                personaje.move(movementVector);
            }

            GuiController.Instance.ThirdPersonCamera.Target = personaje.getPosition();

            //Renderizar suelo
            suelo.render();
            personaje.inventory.update(elapsedTime);
            personaje.inventory.render();
            skyBox.render();

            personaje.playAnimation(animation, true);
            personaje.updateAnimation();
            personaje.render();
            objects.RemoveAll(crafteable => crafteable.getStatus() == 5);
            objects.ForEach(crafteable => crafteable.render());
            
        }

        public override void close()
        {
            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            palmeraOriginal.dispose();
            pasto.dispose();
            skyBox.dispose();
            personaje.dispose();
            objectsFactory.dispose();
        }
    }
}
