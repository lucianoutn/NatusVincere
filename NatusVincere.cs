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
using System.IO;
using System.Windows.Forms;
using AlumnoEjemplos.NatusVincere.NVSkyBoxes;
using TgcViewer.Utils.Particles;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        TgcSprite spriteLogo;
        DateTime tiempoLogo;
        DateTime tiempoPresentacion;
        Hud hud;
        List<Crafteable> objects;
        NVSkyBox skyBox;
        Human personaje;
        NVCamaraFps cam;
        Vector3 targetCamara3, targetCamara1;
        //Vector3 eye; 
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;
        World currentWorld;
        World[][] worlds;
        ObjectsFactory objectsFactory;
        TgcD3dInput input;
        Microsoft.DirectX.Direct3D.Device d3dDevice;
        //TgcViewer.Utils.TgcD3dDevice d3dDevice;
        TgcViewer.Utils.Logger log; 
        Vector3 lookfrom = new Vector3(-2500, 3400, 2000);
        Vector3 lookAt = new Vector3(0, 0, 0);
        Size screenSize;
        
        //bool showPersonajeMesh = true;
        int flag = 0;

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

        ParticleEmitter emitter;
        string texturePath;
        string[] textureNames;
        string selectedTextureName;
        int selectedParticleCount;

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

        /*public float CalcularAltura(float x, float z)
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
        */

        public override void init()
        {
            //Inicializaciones
            input = GuiController.Instance.D3dInput;
            d3dDevice = GuiController.Instance.D3dDevice;
           
            //worlds
            int size = 7000;
            worlds = new World[3][];
            worlds[0] = new World[3];
            worlds[1] = new World[3];
            worlds[2] = new World[3];
            worlds[0][0] = new World(new Vector3(-size, 0, size), size);
            worlds[0][1] = new World(new Vector3(0, 0, size), size);
            worlds[0][2] = new World(new Vector3(size, 0, size), size);
            worlds[1][1] = new World(new Vector3(0, 0, 0), size);
            worlds[1][0] = new World(new Vector3(-size, 0, 0), size);
            worlds[1][2] = new World(new Vector3(size, 0, 0), size);
            worlds[2][0] = new World(new Vector3(-size, 0, -size), size);
            worlds[2][1] = new World(new Vector3(0, 0, -size), size);
            worlds[2][2] = new World(new Vector3(size, 0, -size), size);
            currentWorld = worlds[1][1];
                       
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
            screenSize = GuiController.Instance.Panel3d.Size;
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

            //configurando el frustum
            //Plane leftPlane = new Plane(0,0,0,1000);
            //Plane rightPlane = new Plane(0, 0, 0, 1000);
            //Plane topPlane = new Plane(0, 0, 0, 1000);
            //Plane bottomPlane = new Plane(0, 0, 0, 1000);
            //Plane nearPlane = new Plane(0, 0, 0, 1000);
            //Plane farPlane = new Plane(0, 0, 0, 1000);
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
            //frustum = new TgcFrustum();

            //*****MODIFICADORES*****
            //Modifier para la camara
            GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", true);
            GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", false);
            GuiController.Instance.Modifiers.addBoolean("ROT", "ROT (TEST)", false);
            
            //creo el personaje con su altura en el mapa
            Vector3 posicionPersonaje = new Vector3(1000, currentWorld.calcularAltura(1000, 1000), 1000);
            personaje = objectsFactory.createHuman(posicionPersonaje, new Vector3(2, 2, 2));

            //Hud
            hud = new Hud();

            //Camera en 3ra persona
            //GuiController.Instance.ThirdPersonCamera.Enable = true;
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));// le sumo 50y a la camara para que se vea mjor
            GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara3, 10f, 60f);

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
            cam.alturaPreseteada = 50;
            cam.setCamera(personaje.getPosition(), personaje.getPosition() + new Vector3(50f, 0, 0));
            input.EnableMouseSmooth = true;
            log.log("Inicio Juego", Color.Brown);

            //camara rotacional
            GuiController.Instance.RotCamera.setCamera(targetCamara3, 50f);

            ///////////////CONFIGURAR CAMARA PRIMERA PERSONA//////////////////
            //Camara en primera persona, tipo videojuego FPS
            //Solo puede haber una camara habilitada a la vez. Al habilitar la camara FPS se deshabilita la camara rotacional
            //Por default la camara FPS viene desactivada
            //Configurar posicion y hacia donde se mira
            eye = targetCamara3;
            //Vector3 eye = new Vector3(2,2,2);
            //Vector3 targetFps = personaje.getPosition();
            //GuiController.Instance.FpsCamera.setCamera(eye, targetFps + new Vector3(1.0f, 0.0f, 0.0f));

            //Directorio de texturas
            texturePath = GuiController.Instance.ExamplesMediaDir + "Texturas\\Particles\\";

            //Texturas de particulas a utilizar
            textureNames = new string[] {
                "pisada.png",
                "fuego.png",
                "humo.png",
                "hoja.png",
                "agua.png",
                "nieve.png"
            };

            selectedTextureName = textureNames[1];
            selectedParticleCount = 10;
            emitter = new ParticleEmitter(texturePath + selectedTextureName, selectedParticleCount);
            Vector3 posicionEmitter = new Vector3(10, currentWorld.calcularAltura(10, 10),10);
            emitter.Position = posicionPersonaje;
            
            //Actualizar los demás parametros
            emitter.MinSizeParticle = 20;
            emitter.MaxSizeParticle = 40;
            emitter.ParticleTimeToLive = 8;
            emitter.CreationFrecuency = 0.001f;
            emitter.Dispersion = 30;
            emitter.Speed = new Vector3(10, -10, 10);
            emitter.Enabled = true;

            Sounds sounds = new Sounds();
            sounds.playMusic();
            sounds.playViento();
        }

        public override void render(float elapsedTime)
        {

            
            //Renderizo el logo del inicio y el hud
            if (DateTime.Now < (tiempoPresentacion.AddSeconds((double)10)))
            {
                //animacion
                
                if (lookfrom.Y -250f > currentWorld.calcularAltura(lookfrom.X, lookfrom.Z)) lookfrom.Y += (elapsedTime * -150f);
                if (lookfrom.X < targetCamara3.X) lookfrom.X += (elapsedTime * 150f);
                if (lookfrom.Z > targetCamara3.Z) lookfrom.Z += (elapsedTime * -100f);
                
                lookAt = personaje.getPosition();
                 Matrix lookAtM = Matrix.LookAtLH(lookfrom, lookAt, vNormal);
                 Matrix result = lookAtM;
                 d3dDevice.Transform.View = result;
                 personaje.rotateY(elapsedTime* .3f);
                 personaje.render();
                 personaje.move(lookfrom-lookAt);

                if (DateTime.Now < (tiempoLogo.AddSeconds((double)5)))
                {
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    spriteLogo.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                }

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
                personaje.leaveObject(currentWorld);
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
           
            /* 
            //actualizando camaras
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));

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
                //personaje.render(); //para test
                Cursor.Hide();
                //GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara1, 0f, 10f);//provisorio
                //GuiController.Instance.ThirdPersonCamera.Enable = true; //provisorio
                //targetCamara3 = targetCamara1;//provisorio
            }
            else
            {
                Cursor.Show();
                //personaje.render();
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
           */

            //Frustum values FAR PLANE
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(((float)((45.0f)* Math.PI / 180)),
                (screenSize.Width/screenSize.Height), 1f, 99999999f);
            //GuiController.Instance.Frustum.render();
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();


            personaje.setWorld(currentWorld);

            //recalculo la vida del jugador segun el tiempo transcurrido
            personaje.recalcularStats();
            //Actualizar personaje
            personaje.inventory.update();
            personaje.inventory.render();

            //Actualizar Skybox
            skyBox.updateYRender(personaje.getPosition()); //lo dibuja y lo mueve con centro en el personaje
            

            currentXCam = cam.getPosition().X;
            currentZCam = cam.getPosition().Z;
            alturaCam = currentWorld.calcularAltura(currentXCam, currentZCam);
            cam.setPosition(new Vector3(currentXCam, alturaCam + cam.alturaPreseteada, currentZCam));
            personaje.setPosition(new Vector3(currentXCam, alturaCam, currentZCam));
            
            
            refreshWorlds();
            //personaje.refresh(currentWorld, -cam.viewDir, elapsedTime);
            skyBox.updateYRender(personaje.getPosition());
            refreshCamera(); //Necesita que se actualice primero el personaje


            personaje.setBB(personaje.getPosition());
            //personaje.render();
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    worlds[i][j].render();

                }
            }

            //Render de emisor
            emitter.render();

            personaje.Render();

        }



        private bool FullScreen()
        {
            DialogResult result = MessageBox.Show("Che, ¿queres verlo mejor en fullscreen?", "Confirmación", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }

        /*private void cambioHorario(Object myObject, EventArgs myEventArgs)
          {
              //skyBox.dispose();
              //skyBox = new NVSkyBox();
              skyBox.cambiarHorario();
              temporizador.Stop();
              temporizador.Enabled = false;
              skyBox = new NVSkyBox();
          }
          */   

        public void refreshCamera()
        {
            //actualizando camaras
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));

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
                //personaje.render(); //para test
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
                                   
            //Vector3 cameraPosition = personaje.getPosition() + new Vector3(0, 50, 0);
            //cam.setPosition(cameraPosition);
        }

        public void refreshWorlds()
        {
            if (true)
            {
                Vector3 logicPosition = personaje.getPosition() - currentWorld.position;
                //showAsText(logicPosition.X, 100, 300);
                //showAsText(logicPosition.Z, 100, 350);
                //showAsText(logicPosition.Y, 100, 400);
                int size = 7000 / 2;
                if (logicPosition.X > size)
                {
                    Vector3 newPosition = personaje.getPosition();
                    newPosition.X = -size;
                    //personaje.setPosition(newPosition);
                    /*
                    flag = 1;
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {

                            if (j == 2)
                            {
                                worlds[i][j] = new World(new Vector3(worlds[i][j].position.X + (size * 2), worlds[i][j].position.Y, worlds[i][j].position.Z));
                            }
                            if (j == 0)
                            {
                                worlds[i][j].dispose();
                                worlds[i][j] = worlds[i][j + 1];
                            }
                            if (j == 1)
                            {
                                worlds[i][j] = worlds[i][j + 1];
                            }
                        }
                    }*/
                }
                if (logicPosition.X < -size)
                {
                    flag = 1;
                    Vector3 newPosition = personaje.getPosition();
                    newPosition.X = size;
                    //personaje.setPosition(newPosition);
                    /*
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {
                            if (j == 0)
                            {
                                worlds[i][j] = new World(new Vector3(worlds[i][j].position.X - (size * 2), worlds[i][j].position.Y, worlds[i][j].position.Z));
                            }

                            if (j == 2)
                            {
                                worlds[i][j].dispose();
                                worlds[i][j] = worlds[i][j - 1];
                            }

                            if (j == 1)
                            {
                                worlds[i][j] = worlds[i][j - 1];
                            }
                        }
                    }*/
                }
                if (logicPosition.Z > size)
                {
                    flag = 1;
                    Vector3 newPosition = personaje.getPosition();
                    newPosition.Z = -size;
                    //personaje.setPosition(newPosition);
                    /*
                    for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {

                            if (i == 0)
                            {
                                worlds[i][j] = new World(new Vector3(worlds[i][j].position.X, worlds[i][j].position.Y, worlds[i][j].position.Z + (size * 2)));
                            }
                            if (i == 2)
                            {
                                worlds[i][j].dispose();
                                worlds[i][j] = worlds[i - 1][j];
                            }
                            if (i == 1)
                            {
                                worlds[i][j] = worlds[i - 1][j];
                            }
                        }
                    }*/
                }
                if (logicPosition.Z < -size)
                {
                    flag = 1;
                    flag = 1;
                    Vector3 newPosition = personaje.getPosition();
                    newPosition.Z = size;
                    //personaje.setPosition(newPosition);
                    /*for (int i = 0; i <= 2; i++)
                    {
                        for (int j = 0; j <= 2; j++)
                        {

                            if (i == 2)
                            {
                                worlds[i][j] = new World(new Vector3(worlds[i][j].position.X, worlds[i][j].position.Y, worlds[i][j].position.Z - (size * 2)));
                            }
                            if (i == 0)
                            {
                                worlds[i][j].dispose();
                                worlds[i][j] = worlds[i + 1][j];
                            }
                            if (i == 1)
                            {
                                worlds[i][j] = worlds[i + 1][j];
                            }

                        }
                    }*/
                }
            }
            currentWorld = worlds[1][1];
            currentWorld.refresh();
        }


        public void showAsText(float unNumero, int positionX, int positionY)
        {
            TextCreator textCreator = new TextCreator("Arial", 16, new Size(200, 200));
            TgcText2d text = textCreator.createText(unNumero.ToString() + "POSICIONES");
            text.Position = new Point(positionX, positionY);
            text.render();
        }

        public override void close()
        {
            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            //pasto.dispose();
            skyBox.dispose();
            personaje.dispose();
            currentWorld.dispose();
            spriteLogo.dispose();
            hud.dispose();
            cam.Enable = false; //para q deje de capturar el mouse

        }
    }
}
