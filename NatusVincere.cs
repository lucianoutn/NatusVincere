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
using TgcViewer.Utils._2D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System.IO;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        TgcSprite spriteLogo;
        DateTime tiempoLogo;
       
        const float MOVEMENT_SPEED = 200f;
        TgcBox suelo;
        List<Crafteable> objects;
        TgcMesh palmeraOriginal;
        TgcMesh pasto;
        TgcSkyBox skyBox;
        Human personaje;
        Vector3 targetCamara;

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
            
            //Creo un sprite de logo inicial
            spriteLogo = new TgcSprite();
            spriteLogo.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi_LOGO.png");
            tiempoLogo = DateTime.Now;
            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = spriteLogo.Texture.Size;
            spriteLogo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));


                        
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
            suelo = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(3000, 0, 4900), pisoTexture);
            personaje = objectsFactory.createHuman(suelo.Position + new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            //Cargar modelo de palmera original
            TgcScene scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            palmeraOriginal = scene.Meshes[0];

            //Cargar pasto
            scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\Planta\Planta-TgcScene.xml");
            pasto = scene.Meshes[0];


            //Modifier para la camara
            GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", false);
            GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", true);
            GuiController.Instance.Modifiers.addBoolean("ROT", "ROT (TEST)", false);
            //Camera en 3ra persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            targetCamara = ((personaje.getPosition()) + new Vector3(0, 50f, 0));// le sumo 50y a la camara para que se vea mjor
            GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara, 10f, 60f);objects.Add(objectsFactory.createArbol(suelo.Position + new Vector3(30, 1, 0), new Vector3(0.75f, 0.75f, 0.75f)));
            objects.Add(objectsFactory.createHacha(suelo.Position + new Vector3(200, 1, 0), new Vector3(10, 10, 10)));
            objects.Add(objectsFactory.createPiedra(suelo.Position + new Vector3(100, 1, 0), new Vector3(0.75f, 0.75f, 0.75f)));
            //camara rotacional
            GuiController.Instance.RotCamera.setCamera(targetCamara, 50f);
            
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            //Configurar posicion y hacia donde se mira
            GuiController.Instance.FpsCamera.setCamera(personaje.getPosition(), new Vector3(2, 2, 2));
            
        }
        
        public override void render(float elapsedTime)
        {
            //Renderizo el logo del inicio
            if (DateTime.Now < (tiempoLogo.AddSeconds((double)5)))
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();
                spriteLogo.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }

            //Controlo los modificadores de la camara
            GuiController.Instance.ThirdPersonCamera.Enable = (bool)GuiController.Instance.Modifiers["3ra"];
            GuiController.Instance.FpsCamera.Enable = (bool)GuiController.Instance.Modifiers["FPS"];
            GuiController.Instance.RotCamera.Enable = (bool)GuiController.Instance.Modifiers["ROT"];

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
                float rotAngle = ((float)Math.PI / 180) * (rotate * elapsedTime);
                personaje.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
                //GuiController.Instance.FpsCamera.updateViewMatrix;
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

            //actualizando camaras
            targetCamara = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            GuiController.Instance.ThirdPersonCamera.Target = targetCamara;
            GuiController.Instance.RotCamera.setCamera(targetCamara, 50f);
            GuiController.Instance.FpsCamera.setCamera(targetCamara, lookAt);
            


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
            spriteLogo.dispose();
        }
    }
}
