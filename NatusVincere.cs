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
using System.Windows.Forms;
using AlumnoEjemplos.NatusVincere.NVSkyBoxes;
using TgcViewer.Utils;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
              
        TgcSprite spriteLogo;
        DateTime tiempoLogo;
        DateTime tiempoPresentacion;
        Hud hud;
        List<Crafteable> objects;
        TgcMesh palmeraOriginal;
        //TgcMesh pasto;
        NVSkyBox skyBox;
        Human personaje;
        NVCamaraFps cam;
        //TgcFpsCamera cam;
        Vector3 targetCamara3, targetCamara1;
        //Vector3 eye; 
        //Vector3 targetFps;
        Vector3 vNormal = new Vector3(0,1,0);
        //TgcFrustum frustum;
        TgcD3dInput input;
        Microsoft.DirectX.Direct3D.Device d3dDevice;
        ObjectsFactory objectsFactory;
        TgcSimpleTerrain terrain;
        //GrillaRegular grilla;
        //static Timer temporizador;
        TgcViewer.Utils.Logger log; 
        Vector3 lookfrom = new Vector3(-1500, 2000, 1000);
        Vector3 lookAt = new Vector3(0, 0, 0);
        

        //string animationCaminar = "Walk";
        const float MOVEMENT_SPEED = 200f;
        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ;
        float currentScaleY;
        float currentX;
        float currentZ;
        float altura;
        float currentXCam;
        float currentZCam;
        float alturaCam;

        
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

        public float CalcularAltura(float x, float z)
        {
            float largo = currentScaleXZ * 64;
            float pos_i = 64f * (0.5f + x / largo);
            float pos_j = 64f * (0.5f + z / largo);

            int pi = (int)pos_i;
            float fracc_i = pos_i - pi;
            int pj = (int)pos_j;
            float fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else
                if (pi > 63)
                pi = 63;

            if (pj < 0)
                pj = 0;
            else
                if (pj > 63)
                pj = 63;

            int pi1 = pi + 1;
            int pj1 = pj + 1;
            if (pi1 > 63)
                pi1 = 63;
            if (pj1 > 63)
                pj1 = 63;

            // 2x2 percent closest filtering usual: 
            float H0 = terrain.HeightmapData[pi, pj] * currentScaleY;
            float H1 = terrain.HeightmapData[pi1, pj] * currentScaleY;
            float H2 = terrain.HeightmapData[pi, pj1] * currentScaleY;
            float H3 = terrain.HeightmapData[pi1, pj1] * currentScaleY;
            float H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                      (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            return H;
        }

        public override void init()
        {
            //Inicializaciones
            input = GuiController.Instance.D3dInput; 
            d3dDevice = GuiController.Instance.D3dDevice;

            //FullScreen
            GuiController.Instance.FullScreenEnable = this.FullScreen();
            GuiController.Instance.FullScreenPanel.ControlBox = false;
            GuiController.Instance.FullScreenPanel.Text = null; //"NatusVincere";
            
            //Creo un sprite de logo inicial
            spriteLogo = new TgcSprite();
            spriteLogo.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi_LOGO.png");
            tiempoLogo = DateTime.Now;
            tiempoPresentacion = DateTime.Now;
            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSizeLogo = spriteLogo.Texture.Size;
            spriteLogo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeLogo.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSizeLogo.Height / 2, 0));
                       
            //creacion de la escena
            TgcSceneLoader loader = new TgcSceneLoader();
            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);
            
            //Crear SkyBox
            skyBox = new NVSkyBox();
            skyBox.horario("maniana"); //cambiarlo "maniana" "dia" "tarde" "noche"
            skyBox.init();
            /*
            temporizador = new Timer();
            temporizador.Interval = 200;
            temporizador.Enabled = true;
            temporizador.Start();
            temporizador.Tick += new EventHandler(cambioHorario);
            */

            
            //configurando el frustum
            //Plane leftPlane = new Plane(0,0,0,1000);
            //Plane rightPlane = new Plane(0, 0, 0, 1000);
            //Plane topPlane = new Plane(0, 0, 0, 1000);
            //Plane bottomPlane = new Plane(0, 0, 0, 1000);
            //Plane nearPlane = new Plane(0, 0, 0, 1000);
            //Plane farPlane = new Plane(0, 0, 0, 1000);
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
            //frustum = new TgcFrustum();
            
         
            //TgcViewer.Utils.TgcD3dDevice.zFarPlaneDistance = 1f;
            
            //Path de Heightmap default del terreno
            currentHeightmap = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "Heightmap2.jpg";

            //Path de Textura default del terreno 
            currentTexture = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "TerrainTexture2.jpg";

            //*****MODIFICADORES*****
            //Modifier para la camara
            GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", true);
            GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", false);
            GuiController.Instance.Modifiers.addBoolean("ROT", "ROT (TEST)", false);
            //Modifier para cambiar el heightmap
            GuiController.Instance.Modifiers.addTexture("heightmap", currentHeightmap);
            //Modifier para cambiar la textura del terreno
            GuiController.Instance.Modifiers.addTexture("texture", currentTexture);
            //Modifiers para variar escala del mapa
            currentScaleXZ = 500f;
            currentScaleY = 8f;

                 

            //Cargar terreno: cargar heightmap y textura de color
            terrain = new TgcSimpleTerrain();
            terrain.loadHeightmap(currentHeightmap, currentScaleXZ, currentScaleY, new Vector3(0, 0, 0));
            terrain.loadTexture(currentTexture);


            //grilla = new GrillaRegular();
            
            //Calculo altura del terreno para parar al personaje
            altura = CalcularAltura(0, 0);
            Vector3 terrainPosition = new Vector3(0, altura, 0);
            
            //creo el personaje
            personaje = objectsFactory.createHuman(terrainPosition + new Vector3(-100, 1, 0), new Vector3(1, 1, 1));
        
            //Hud
            hud = new Hud();
                        
            //Cargar modelo de palmera original
            TgcScene scene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            palmeraOriginal = scene.Meshes[0];
            
                       
            //Camera en 3ra persona
            //GuiController.Instance.ThirdPersonCamera.Enable = true;
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));// le sumo 50y a la camara para que se vea mjor
            GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara3, 10f, 60f);

            agegarObjetos(terrainPosition);

            //camara rotacional
            GuiController.Instance.RotCamera.setCamera(targetCamara3, 500f);
            
            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            //Configurar posicion y hacia donde se mira
            //eye = targetCamara3;
            //Vector3 eye = new Vector3(2,2,2);
            //Vector3 targetFps = personaje.getPosition();
            //cam = new TgcFpsCamera(); 
            //cam.setCamera(eye, targetFps + new Vector3(1.0f, 0.0f, 0.0f));
            log = GuiController.Instance.Logger;
            log.clear();
            cam = new NVCamaraFps(personaje);
            cam.alturaPreseteada =50;
            cam.setCamera(personaje.getPosition(), personaje.getPosition() + new Vector3(50f,0,0));
            input.EnableMouseSmooth = true;
            //d3dDevice.Transform.View = Matrix.LookAtLH(lookfrom, lookAt, new Vector3(0, 1, 0));
            
            
            log.log("Inicio Juego", Color.Brown);
        }
        
        public void agegarObjetos(Vector3 terrainPosition)
        {
            objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(30, 1, 0), new Vector3(0.75f, 1.75f, 0.75f)));
            objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(230, 311, 1800), new Vector3(0.75f, 1.75f, 0.75f)));
            objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(2030, 271, 800), new Vector3(0.75f, 1.75f, 0.75f)));
            objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(230, -311, -3000), new Vector3(0.75f, 1.75f, 0.75f)));
            objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(-430, -61, -410), new Vector3(0.75f, 1.75f, 0.75f)));

            objects.Add(objectsFactory.createHacha(terrainPosition + new Vector3(200, 1, 0), new Vector3(10, 10, 10)));
            objects.Add(objectsFactory.createPiedra(terrainPosition + new Vector3(100, 1, 0), new Vector3(0.75f, 0.75f, 0.75f)));
        }

        
        

        public override void render(float elapsedTime)
        {
                            
            
            //Renderizo el logo del inicio y el hud
            if (DateTime.Now < (tiempoPresentacion.AddSeconds((double)10)))
            {
                //animacion
                //GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", true);
                //personaje.render();
                //personaje.updateAnimation();
               
                //GuiController.Instance.ThirdPersonCamera.rotateY(0.5f * elapsedTime);
                //lookfrom.Z -= elapsedTime * 500;
               // lookfrom.Scale(2*elapsedTime);
                if (lookfrom.Y > targetCamara3.Y) lookfrom.Y += (elapsedTime * -100f);
                if (lookfrom.X < targetCamara3.X) lookfrom.X += (elapsedTime * 100f);
                //if (lookfrom.Z > targetCamara3.Z) 
                lookfrom.Z += (elapsedTime * -100f);
                lookAt = personaje.getPosition();
                //lookfrom.Scale(-0.2f/elapsedTime);
               Matrix lookAtM = Matrix.LookAtLH(lookfrom, lookAt, new Vector3(0, 1, 0));
               //Matrix rotM = Matrix.RotationY(2f * elapsedTime);
               //Matrix scaleM = Matrix.Scaling(0.2f*elapsedTime,0.2f*elapsedTime,0.2f*elapsedTime);
               Matrix result = lookAtM;
               d3dDevice.Transform.View = result;
               //d3dDevice.Transform.View.RotateAxis(new Vector3(0,1,0), 222f*elapsedTime);
               //d3dDevice.Transform.View.Invert();
                //d3dDevice.Transform.View.RotateY(20f * elapsedTime);
                //d3dDevice.Transform.View.RotateY(20f * elapsedTime);

                
                if (DateTime.Now < (tiempoLogo.AddSeconds((double)5)))
                {
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    spriteLogo.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                }
                //GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", true);
                
            }
            else //render del hud
            {
                
                hud.renderizate(personaje);
               // GuiController.Instance.CurrentCamera = cam;
                //GuiController.Instance.ThirdPersonCamera.
                //GuiController.Instance.FpsCamera.Enable = true;

               // cam.Enable = true;
                
                
            }
            //fin logo y hud

            #region borrar dsp
            /*
           
            float velocidadCaminar = 5f;
            float velocidadRotacion = 100f;
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            bool moving = false;
            bool rotating = false;
            float jump = 0;
            */
            
           
            

            //Adelante
            if (input.keyDown(Key.W))
            {
                Key key = Key.W;
                personaje.movete(key, 0, elapsedTime);
            }
            /*
            //Atras
            if (input.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (input.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (input.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            //Jump
            if (input.keyDown(Key.Space))
            {
                jump = 30;
                moving = true;
            }


            */
     #region crafteo

            if (input.keyDown(Key.E))
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) objectsFactory.transform(crafteable); });
            }

            if (input.keyDown(Key.R))
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) personaje.store(crafteable); });

            }

            if (input.keyDown(Key.L))
            {
                personaje.leaveObject();
            }

            #endregion crafteo
            
            /*
            //cam.getMovementDirection(input);

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = ((float)Math.PI / 180) * (rotate * elapsedTime);
                personaje.rotateY(rotAngle);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
                //GuiController.Instance.FpsCamera.updateViewMatrix(d3dDevice);
                personaje.playAnimation(animationCaminar, true);
                personaje.updateAnimation();
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
               
                personaje.playAnimation(animationCaminar, true);
                personaje.updateAnimation();
            }

            */
            #endregion borrar dsp

            //actualizando camaras
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));
            //Vector3 mirarA = cam.getLookAt();
            //d3dDevice.Transform.Projection.Scale(400f, 400f, 400f);
            //frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);

            //Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Controlo los modificadores de la camara
            if ((bool)GuiController.Instance.Modifiers["3ra"])
            {
                GuiController.Instance.ThirdPersonCamera.Enable = (bool)GuiController.Instance.Modifiers["3ra"];
                personaje.render();
                //GuiController.Instance.D3dInput
            }
            GuiController.Instance.RotCamera.Enable = (bool)GuiController.Instance.Modifiers["ROT"];
            if ((bool)GuiController.Instance.Modifiers["FPS"])
            {
                cam.Enable = true;
                personaje.render(); //para test
                Cursor.Hide();
               //GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara1, 0f, 10f);//provisorio
                //GuiController.Instance.ThirdPersonCamera.Enable = true; //provisorio
                //targetCamara3 = targetCamara1;//provisorio
            }
            else
            {
                Cursor.Show();
                personaje.render();
            }
                        
            GuiController.Instance.ThirdPersonCamera.Target = targetCamara3;
            //GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara3, 100f, 200);
            //GuiController.Instance.RotCamera.setCamera(targetCamara3, 50f);
            //rotar(-GuiController.Instance.D3dInput.XposRelative * velocidadRotacion,
            //           -GuiController.Instance.D3dInput.YposRelative * velocidadRotacion);
            //GuiController.Instance.FpsCamera.setCamera(eye, targetCamara + new Vector3(1.0f, 0.0f, 0.0f));

            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
            //GuiController.Instance.Frustum.updateMesh(personaje.getPosition(),targetCamara1);
            GuiController.Instance.BackgroundColor = Color.AntiqueWhite;
            //frustum.render();
            
            //GuiController.Instance.Frustum.render();
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();

            
            //recalculo la vida del jugador segun el tiempo transcurrido
            personaje.recalcularStats();
            //Actualizar personaje
            personaje.inventory.update(elapsedTime);
            personaje.inventory.render();
            
            //Actualizar Skybox
            skyBox.updateYRender(personaje.getPosition()); //lo dibuja y lo mueve con centro en el personaje
            /*
            temporizador.Tick += new EventHandler(cambioHorario);
            temporizador.Interval = 200;
            temporizador.Enabled = true;
            temporizador.Start();
            //skyBox.updateValues();
            */


            //Calculo altura del terreno para parar al personaje
            //currentX = personaje.getPosition().X;
            //currentZ = personaje.getPosition().Z;
            //altura = CalcularAltura(currentX, currentZ);
            
            currentXCam = cam.getPosition().X;
            currentZCam = cam.getPosition().Z;
            alturaCam = CalcularAltura(currentXCam, currentZCam);
            cam.setPosition(new Vector3(currentXCam, alturaCam+cam.alturaPreseteada, currentZCam));
            personaje.setPosition(new Vector3(currentXCam, alturaCam, currentZCam));
          

                       
            objects.RemoveAll(crafteable => crafteable.getStatus() == 5);
            objects.ForEach(crafteable => crafteable.render());
            
            terrain.render();

        }

        /*
        private void cambioHorario(Object myObject, EventArgs myEventArgs)
        {
            //skyBox.dispose();
            //skyBox = new NVSkyBox();
            skyBox.cambiarHorario();
            temporizador.Stop();
            temporizador.Enabled = false;
            skyBox = new NVSkyBox();
        }
        */

        private bool FullScreen()
        {
            DialogResult result = MessageBox.Show("Che, ¿queres verlo mejor en fullscreen?", "Confirmación", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }

        public override void close()
        {
            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            palmeraOriginal.dispose();
            //pasto.dispose();
            skyBox.dispose();
            personaje.dispose();
            objectsFactory.dispose();
            spriteLogo.dispose();
            hud.dispose();
            cam.Enable = false; //para q deje de capturar el mouse
            
           
        }
    }
}
