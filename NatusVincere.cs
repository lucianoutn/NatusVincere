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

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        TgcSprite spriteLogo;
        DateTime tiempoLogo;
        DateTime tiempoPresentacion;
        Hud hud;

        const float MOVEMENT_SPEED = 200f;
        List<Crafteable> objects;
        NVSkyBox skyBox;
        Human personaje;
        NVCamaraFps cam;
        Vector3 targetCamara3, targetCamara1;
        Vector3 eye; 
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;
        World currentWorld;
        World[][] worlds;
        ObjectsFactory objectsFactory;
        TgcD3dInput input;
        TgcViewer.Utils.Logger log; 
        Vector3 lookfrom = new Vector3(-1500, 2000, 1000);
        Vector3 lookAt = new Vector3(0, 0, 0);
        bool showPersonajeMesh = true;

        int flag = 0;
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
            input = GuiController.Instance.D3dInput;
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);
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

            skyBox.Center = new Vector3(0, 500, 0);
            skyBox.Size = new Vector3(5000, 5000, 5000);

            string texturesPath = System.Environment.CurrentDirectory + @"\Examples\Media\Texturas\Quake\SkyBox LostAtSeaDay\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.updateValues();

            frustum = new TgcFrustum();

            //*****MODIFICADORES*****
            //Modifier para la camara
            GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", false);
            GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", true);
            GuiController.Instance.Modifiers.addBoolean("ROT", "ROT (TEST)", false);

            Vector3 posicionPersonaje = new Vector3(0, currentWorld.calcularAltura(0, 0), 0);
            personaje = objectsFactory.createHuman(posicionPersonaje, new Vector3(0.5f, 0.5f, 0.5f));

            //Hud
            hud = new Hud();

            //Camera en 3ra persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));// le sumo 50y a la camara para que se vea mjor
            GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara3, 10f, 60f);
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

        }

        public override void render(float elapsedTime)
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

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

            }
            else
            {
                hud.renderizate(personaje);
            }

            //Renderizo el logo del inicio y el hud
            if (DateTime.Now < (tiempoLogo.AddSeconds((double)5)))
            {
                GuiController.Instance.Drawer2D.beginDrawSprite();
                spriteLogo.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
            else //render del hud
            {
                hud.renderizate(personaje);
            }


            if (d3dInput.keyDown(Key.E))
            {
                currentWorld.transform(personaje);
            }

            if (d3dInput.keyDown(Key.R))
            {
                personaje.pickObject(currentWorld);
            }

            if (d3dInput.keyDown(Key.L))
            {
                personaje.leaveObject(currentWorld);
            }


            
            //actualizando camaras
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));
            d3dDevice.Transform.Projection.Scale(4f, 4f, 4f);
            frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);

            refreshWorlds();
            personaje.refresh(currentWorld, -cam.viewDir, elapsedTime);
            skyBox.updateYRender(personaje.getPosition());
            refreshCamera(); //Necesita que se actualice primero el personaje

            personaje.render(true);
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    worlds[i][j].render();

                }
            }
        }


        private bool hayColision(World currentWorld)
        {
            for (int i = 0; i < currentWorld.objects.Count; i++)
            {
                //if (TgcCollisionUtils.testSphereSphere(objects[i].getBB(), personaje.getBB()))
                if(TgcCollisionUtils.testSphereCylinder(currentWorld.objects[i].getBB(), personaje.getBB()))
                {
                    return true;
                }

            };

            return false;
        }

        private bool FullScreen()
        {
            DialogResult result = MessageBox.Show("Che, ¿queres verlo mejor en fullscreen?", "Confirmación", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
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
            cam.Enable = false;
           
        }

        public void refreshCamera()
        {
            //Vector3 mirarA = cam.getLookAt();
            //d3dDevice.Transform.Projection.Scale(400f, 400f, 400f);
            //frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);

            showPersonajeMesh = true;
            //Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            //Controlo los modificadores de la camara
            if ((bool)GuiController.Instance.Modifiers["3ra"])
            {
                GuiController.Instance.ThirdPersonCamera.Enable = (bool)GuiController.Instance.Modifiers["3ra"];
                //GuiController.Instance.D3dInput
            }
            GuiController.Instance.RotCamera.Enable = (bool)GuiController.Instance.Modifiers["ROT"];
            if ((bool)GuiController.Instance.Modifiers["FPS"])
            {
                cam.Enable = true;
                showPersonajeMesh = false;
                Cursor.Hide();
            }
            else
            {
                Cursor.Show();
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

            GuiController.Instance.ThirdPersonCamera.Target = targetCamara3;
            GuiController.Instance.RotCamera.setCamera(targetCamara3, 50f);
            //rotar(-GuiController.Instance.D3dInput.XposRelative * velocidadRotacion,
            //           -GuiController.Instance.D3dInput.YposRelative * velocidadRotacion);
            //GuiController.Instance.FpsCamera.setCamera(eye, targetCamara + new Vector3(1.0f, 0.0f, 0.0f));
            Vector3 cameraPosition = personaje.getPosition() + new Vector3(0, 50, 0);
            cam.setPosition(cameraPosition);
        }

        public void refreshWorlds()
        {
            if (true)
            {
                Vector3 logicPosition = personaje.getPosition() - currentWorld.position;
                showAsText(logicPosition.X, 100, 300);
                showAsText(logicPosition.Z, 100, 350);
                showAsText(logicPosition.Y, 100, 400);
                int size = 7000 / 2;
                if (logicPosition.X > size)
                {
                    Vector3 newPosition = personaje.getPosition();
                    newPosition.X = -size;
                    personaje.setPosition(newPosition);
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
                    personaje.setPosition(newPosition);/*
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
                    personaje.setPosition(newPosition);/*
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
                    personaje.setPosition(newPosition);
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
    }
}
