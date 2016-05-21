using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.Input;
using TgcViewer.Utils._2D;
using Microsoft.DirectX.DirectInput;
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
        TgcMesh palmeraOriginal;
        TgcSkyBox skyBox;
        Human personaje; 
        Vector3 targetCamara3, targetCamara1;
        Vector3 eye; 
        Vector3 targetFps;
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;

        ObjectsFactory objectsFactory;

        TgcSimpleTerrain terrain;
        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ;
        float currentScaleY;
        float currentX;
        float currentZ;
        float altura;

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
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

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
            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);
            
            //Crear SkyBox
            skyBox = new TgcSkyBox();
            skyBox.Center = new Vector3(0, 500, 0);
            skyBox.Size = new Vector3(40000, 40000, 40000);
            string texturesPath = System.Environment.CurrentDirectory + @"\Examples\Media\Texturas\Quake\SkyBox LostAtSeaDay\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.updateValues();
            
            //configurando el frustum  Plane leftPlane = new Plane(0,0,0,1000);  Plane rightPlane = new Plane(0, 0, 0, 1000);  Plane topPlane = new Plane(0, 0, 0, 1000);  Plane bottomPlane = new Plane(0, 0, 0, 1000);  Plane nearPlane = new Plane(0, 0, 0, 1000);  Plane farPlane = new Plane(0, 0, 0, 1000);
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
            frustum = new TgcFrustum();

            //Path de Heightmap default del terreno
            currentHeightmap = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "Heightmap2.jpg";

            //Path de Textura default del terreno 
            currentTexture = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "TerrainTexture2.jpg";

            //*****MODIFICADORES*****
            //Modifier para la camara
            GuiController.Instance.Modifiers.addBoolean("FPS", "FPS", false);
            GuiController.Instance.Modifiers.addBoolean("3ra", "3ra (TEST)", true);
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

            //Calculo altura del terreno para parar al personaje
            altura = CalcularAltura(0, 0);
            Vector3 terrainPosition = new Vector3(0, altura, 0);
            
            //creo el personaje
            personaje = objectsFactory.createHuman(terrainPosition + new Vector3(-100, 1, 0), new Vector3(1, 1, 1));
        
            //Hud
            hud = new Hud();
                                   
            //Camera en 3ra persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));// le sumo 50y a la camara para que se vea mjor
            GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara3, 10f, 60f);

            agegarObjetos(terrainPosition);

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
                objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) objectsFactory.transform(crafteable); });
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
                
                bool collide = false;

                foreach (Crafteable objeto in objects)
                {
                    //collide = TgcCollisionUtils.testAABBAABB(personaje.getMesh().BoundingBox,objeto.getMesh().BoundingBox);
                    objeto.getBB().render();
                };

                if (!collide)
                {
                    personaje.move(movementVector);
                }
                personaje.getMesh().BoundingBox.render();
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
            }
            else
            {
                //  Cursor.Show();
                personaje.render();
            }
                        
            GuiController.Instance.ThirdPersonCamera.Target = targetCamara3;
            GuiController.Instance.RotCamera.setCamera(targetCamara3, 50f);
            //rotar(-GuiController.Instance.D3dInput.XposRelative * velocidadRotacion,
            //           -GuiController.Instance.D3dInput.YposRelative * velocidadRotacion);
            //GuiController.Instance.FpsCamera.setCamera(eye, targetCamara + new Vector3(1.0f, 0.0f, 0.0f));
            

            
            //recalculo la vida del jugador segun el tiempo transcurrido
            personaje.recalcularStats();
            //Actualizar personaje
            personaje.inventory.update(elapsedTime);
            personaje.inventory.render();
            skyBox.render();

            //Calculo altura del terreno para parar al personaje
            currentX = personaje.getPosition().X;
            currentZ = personaje.getPosition().Z;
            altura = CalcularAltura(currentX, currentZ);
            personaje.setPosition(new Vector3(currentX, altura, currentZ));
            personaje.setBB(new Vector3(currentX, altura, currentZ));


            personaje.playAnimation(animation, true);
            personaje.updateAnimation();
            
            objects.RemoveAll(crafteable => crafteable.getStatus() == 5);
            objects.ForEach(crafteable => crafteable.render());

            terrain.render();

        }

        public void agegarObjetos(Vector3 terrainPosition)
        {
            int col = 5;
            int x = 0;
            int z = 0;
            int i, j;

            for (i = 0; i < col; i++)
            {
                for (j = 0; j < col; j++)
                {
                    x = j * 530 - 2000;
                    z = i * 530 - 4000;

                    objects.Add(objectsFactory.createArbol(terrainPosition + new Vector3(x, CalcularAltura(x, z) - 800, z), new Vector3(0.75f, 1.75f, 0.75f)));
                    objects.Add(objectsFactory.createArbusto(terrainPosition + new Vector3(x-100, CalcularAltura(x-100, z-100) - 740, z-100), new Vector3(0.75f, 0.75f, 0.75f)));
                }
            }

            for (i = 0; i < col; i++)
            {
                for (j = 0; j < col; j++)
                {
                    x = j * 530 - 1700;
                    z = i * 530 + 2000;

                    objects.Add(objectsFactory.createPino(terrainPosition + new Vector3(x, CalcularAltura(x, z) - 790, z), new Vector3(5.75f, 8.75f, 5.75f)));
                    if( (i==2 && j==3) || (i==4 && j==1) || (i==0 && j==0) )
                        objects.Add(objectsFactory.createPiedra(terrainPosition + new Vector3(x + 100, CalcularAltura(x + 100, z + 100) - 740, z + 100), new Vector3(0.75f, 0.75f, 0.75f)));
                    if(i == 4 && j == 4)
                        objects.Add(objectsFactory.createPiedra(terrainPosition + new Vector3(x, CalcularAltura(x, z + 50) - 740, z + 50), new Vector3(0.75f, 0.75f, 0.75f)));
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
            skyBox.dispose();
            personaje.dispose();
            objectsFactory.dispose();
            spriteLogo.dispose();
            hud.dispose();
           
        }
    }
}
