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

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        TgcSprite spriteLogo;
        
       // TextCreator textCreator;
        DateTime tiempoLogo;
        Hud hud;

        const float MOVEMENT_SPEED = 200f;
        List<Crafteable> objects;
        TgcSkyBox skyBox;
        Human personaje; 
        Vector3 targetCamara3, targetCamara1;
        Vector3 eye; 
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;
        World currentWorld;
        World[][] worlds;
        ObjectsFactory objectsFactory;
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
            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
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
            //Ubicarlo centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;

            Size textureSizeLogo = spriteLogo.Texture.Size;
            spriteLogo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeLogo.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSizeLogo.Height / 2, 0));

            //creaion de la escena
            TgcSceneLoader loader = new TgcSceneLoader();

            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 500, 0);
            skyBox.Size = new Vector3(10000, 10000, 10000);
            string texturesPath = System.Environment.CurrentDirectory + @"\Examples\Media\Texturas\Quake\SkyBox LostAtSeaDay\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.updateValues();

            //configurando el frustum
            //Plane leftPlane = new Plane(0,0,0,1000);
            //Plane rightPlane = new Plane(0, 0, 0, 1000);
            //Plane topPlane = new Plane(0, 0, 0, 1000);
            //Plane bottomPlane = new Plane(0, 0, 0, 1000);
            //Plane nearPlane = new Plane(0, 0, 0, 1000);
            //Plane farPlane = new Plane(0, 0, 0, 1000);
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
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
            TgcD3dInput input = GuiController.Instance.D3dInput;


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

            
            float velocidadCaminar = 5f;
            float velocidadRotacion = 100f;
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;

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
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));
            d3dDevice.Transform.Projection.Scale(4f, 4f, 4f);
            frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);


            //Controlo los modificadores de la camara
            GuiController.Instance.ThirdPersonCamera.Enable = (bool)GuiController.Instance.Modifiers["3ra"];
            GuiController.Instance.RotCamera.Enable = (bool)GuiController.Instance.Modifiers["ROT"];
            if (GuiController.Instance.FpsCamera.Enable = (bool)GuiController.Instance.Modifiers["FPS"])
            {
                //habilitar luego cuando este la camara fps mejorada
                //Control focusWindows = GuiController.Instance.D3dDevice.CreationParameters.FocusWindow;
                //Cursor.Position = focusWindows.PointToScreen(new Point(focusWindows.Width / 2, focusWindows.Height / 2));
                //Cursor.Hide();
                GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara1, 0f, 10f);//provisorio
                GuiController.Instance.ThirdPersonCamera.Enable = true; //provisorio
                targetCamara3 = targetCamara1;//provisorio
                personaje.render(false);
            }
            else
            {
                //  Cursor.Show();
                personaje.render(true);
            }
                        
            GuiController.Instance.ThirdPersonCamera.Target = targetCamara3;
            GuiController.Instance.RotCamera.setCamera(targetCamara3, 50f);
            //rotar(-GuiController.Instance.D3dInput.XposRelative * velocidadRotacion,
            //           -GuiController.Instance.D3dInput.YposRelative * velocidadRotacion);
            //GuiController.Instance.FpsCamera.setCamera(eye, targetCamara + new Vector3(1.0f, 0.0f, 0.0f));

            refreshWorlds();
            personaje.refresh(currentWorld);
            currentWorld.render();

            Vector3 position = personaje.getPosition();
            skyBox.Center = new Vector3(position.X, position.Y, position.Z);
            skyBox.updateValues();
            skyBox.render();
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    worlds[i][j].render();

                }
            }
        }


        private bool FullScreen()
        {
            DialogResult result = MessageBox.Show("Che, ¿queres mejor en fullscreen?", "Confirmación", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }

        public override void close()
        {
            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            //pasto.dispose();
            skyBox.dispose();
            personaje.dispose();
            objectsFactory.dispose();
            spriteLogo.dispose();
            hud.dispose();
           
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
