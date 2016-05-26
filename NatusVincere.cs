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
        //TgcMesh pasto;
        NVSkyBox skyBox;
        Human personaje;
        CamaraFps cam;
        TgcFpsCamera camera;
        Vector3 targetCamara3, targetCamara1;
        Vector3 eye; 
        Vector3 targetFps;
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;
        TgcD3dInput input;
        Microsoft.DirectX.Direct3D.Device d3dDevice;
        ObjectsFactory objectsFactory;
        String animationCaminar = "Walk";


        TgcSimpleTerrain terrain;
        string currentHeightmap;
        string currentTexture;
        float currentScaleXZ;
        float currentScaleY;
        float currentX;
        float currentZ;
        float altura;

        //GrillaRegular grilla;

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
            d3dDevice = GuiController.Instance.D3dDevice;

            //FullScreen
            GuiController.Instance.FullScreenEnable = this.FullScreen();
            GuiController.Instance.FullScreenPanel.ControlBox = false;
            GuiController.Instance.FullScreenPanel.Text = null; //"NatusVincere";
            
            //Inicializaciones
            input = GuiController.Instance.D3dInput; 

            //Creo un sprite de logo inicial
            spriteLogo = new TgcSprite();
            spriteLogo.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi_LOGO.png");
            tiempoLogo = DateTime.Now;
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
            //skyBox.Center = new Vector3(0, 1200, 0); //ajustando altura
            skyBox.Size = new Vector3(8000, 8000, 8000);
            string texturesPath = System.Environment.CurrentDirectory + @"\Examples\Media\Texturas\Quake\SkyBox LostAtSeaDay\";
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skyBox.SkyEpsilon =1f;
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
            
         
            //TgcViewer.Utils.TgcD3dDevice.zFarPlaneDistance = 1f;
            
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
            camera = new TgcFpsCamera(); 
            //GuiController.Instance.FpsCamera.setCamera(eye, targetFps + new Vector3(1.0f, 0.0f, 0.0f));
            camera.setCamera(eye, targetFps + new Vector3(1.0f, 0.0f, 0.0f));
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
            //fin logo y hud

            
            float velocidadCaminar = 5f;
            float velocidadRotacion = 100f;
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            bool moving = false;
            bool rotating = false;
            float jump = 0;

            
           
            

            //Adelante
            if (input.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

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

            //actualizando camaras
            targetCamara3 = ((personaje.getPosition()) + new Vector3(0, 50f, 0));
            targetCamara1 = ((personaje.getPosition()) + new Vector3(0, 30f, 0));
            Vector3 mirarA = camera.getLookAt();
            d3dDevice.Transform.Projection.Scale(400f, 400f, 400f);
            //frustum.updateVolume(d3dDevice.Transform.View, d3dDevice.Transform.Projection);


            //Controlo los modificadores de la camara
            GuiController.Instance.ThirdPersonCamera.Enable = (bool)GuiController.Instance.Modifiers["3ra"];
            GuiController.Instance.RotCamera.Enable = (bool)GuiController.Instance.Modifiers["ROT"];
            if (camera.Enable = (bool)GuiController.Instance.Modifiers["FPS"])
            {
               
                //habilitar luego cuando este la camara fps mejorada
                Control focusWindows = d3dDevice.CreationParameters.FocusWindow;
                Cursor.Position = focusWindows.PointToScreen(new Point(focusWindows.Width / 2, focusWindows.Height / 2));
                //GuiController.Instance.FpsCamera.updateViewMatrix(d3dDevice);
               float xpos = GuiController.Instance.D3dInput.XposRelative;
                float ypos = GuiController.Instance.D3dInput.YposRelative;
                mirarA = new Vector3(xpos, ypos, 0);
                camera.setCamera(targetCamara1, mirarA);
                //camera.updateViewMatrix(d3dDevice);
                camera.updateCamera();
                // personaje.render(); //para test
                
                
                Cursor.Hide();
               // GuiController.Instance.ThirdPersonCamera.setCamera(targetCamara1, 0f, 10f);//provisorio
                //GuiController.Instance.ThirdPersonCamera.Enable = true; //provisorio
                //targetCamara3 = targetCamara1;//provisorio
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

            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();
            //GuiController.Instance.Frustum.updateMesh(personaje.getPosition(),targetCamara1);
            GuiController.Instance.BackgroundColor = Color.AntiqueWhite;
            frustum.render();
            
            //GuiController.Instance.Frustum.render();
            //GuiController.Instance.Frustum.FrustumPlanes.Initialize();

            
            //recalculo la vida del jugador segun el tiempo transcurrido
            personaje.recalcularStats();
            //Actualizar personaje
            personaje.inventory.update(elapsedTime);
            personaje.inventory.render();
            
            //Actualizar Skybox
            //skyBox.render();
            //skyBox.Center = personaje.getPosition();
            //movementVector
            //skyBox.updateYRender(movementVector);
            skyBox.updateYRender(personaje.getPosition());
            //skyBox.updateValues();

            //Calculo altura del terreno para parar al personaje
            currentX = personaje.getPosition().X;
            currentZ = personaje.getPosition().Z;
            altura = CalcularAltura(currentX, currentZ);
            personaje.setPosition(new Vector3(currentX, altura, currentZ));
          

                       
            objects.RemoveAll(crafteable => crafteable.getStatus() == 5);
            objects.ForEach(crafteable => crafteable.render());

            terrain.render();

        }


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
           
        }
    }
}
