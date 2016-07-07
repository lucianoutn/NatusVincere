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
using TgcViewer.Utils.Interpolation;
using TgcViewer.Utils;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.NatusVincere
{
    public class NatusVincere : TgcExample
    {
        bool victoria = false;
        TgcSprite spriteLogo;
        TgcSprite spriteWin;
        TgcSprite spriteObjetivos;
        TgcSprite hachaEnMano;
        Hud hud;
        List<Crafteable> objects;
        NVSkyBox []skyBox;
        Human personaje;
        Leon leon;
        NVCamaraFps cam;
        Vector3 targetCamara3, targetCamara1;
        Vector3 eye; 
        Vector3 vNormal = new Vector3(0,1,0);
        TgcFrustum frustum;
        World currentWorld;
        World[][] worlds;
        World[][] savedWorlds;
        ObjectsFactory objectsFactory;
        TgcD3dInput input;
        TgcViewer.Utils.Logger log; 
        Vector3 lookfrom = new Vector3(-2500, 2400, 2000);
        Vector3 lookAt = new Vector3(0, 0, 0);
        Size screenSize;
        int horaDelDia = 2; //0: maniana, 1:dia, 2:tarde, 3:noche
        int worldSize = 7500;
        //bool showPersonajeMesh = true;
        int flag = 0;

        //string animationCaminar = "Walk";
        const float MOVEMENT_SPEED = 200f;
        int sectorToRender;
        float altura;
        float currentXCam;
        float currentZCam;
        float alturaCam;

        Sounds sounds;
        
        TgcMesh wilson;
        
        VertexBuffer screenQuadVB;
        Texture renderTarget2D;
        Surface g_pDSShadow;     // Depth-stencil buffer for rendering to shadow map
        Surface pOldRT;
        Microsoft.DirectX.Direct3D.Effect effect;
        TgcTexture lluviaTexture;
        TgcTexture alarmaTexture;
        TgcText2d cartelContinuar = new TgcText2d(); //presentacion
        float timeRed;
        Matrix matPreview = new Matrix(); //presentacion

        InterpoladorVaiven intVaivenAlarm;

        Microsoft.DirectX.Direct3D.Device d3dDevice;

        float time;
        float timeAcumParaCambioDeHorario;
        float timeAcumParaLluvia;
        bool presentacion; //presentacion
        bool continuar; //presentacion

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
            //Inicializaciones
            input = GuiController.Instance.D3dInput;
            d3dDevice = GuiController.Instance.D3dDevice;
           
            //worlds
            int size = worldSize;
            worlds = new World[3][];
            worlds[0] = new World[3];
            worlds[1] = new World[3];
            worlds[2] = new World[3];
            savedWorlds = new World[3][];
            savedWorlds[0] = new World[3];
            savedWorlds[1] = new World[3];
            savedWorlds[2] = new World[3];
            
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
            spriteLogo.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi\\NaVi_LOGO.png");
            spriteWin = new TgcSprite();
            spriteWin.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi\\NaVi_LOGO2.png");

            screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSizeLogo = spriteLogo.Texture.Size;
            spriteLogo.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeLogo.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSizeLogo.Height / 2, 0));
            spriteWin.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeLogo.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSizeLogo.Height / 2, 0));
            spriteObjetivos = new TgcSprite();
            spriteObjetivos.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\NaVi\\objetivos.png");
            spriteObjetivos.Position = new Vector2(2, 2);
            hachaEnMano = new TgcSprite();
            hachaEnMano.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\hud\\hachaMina.png");
            hachaEnMano.Position = new Vector2(screenSize.Width - hachaEnMano.Texture.Size.Width, screenSize.Height - hachaEnMano.Texture.Size.Height);

            //creacion de la escena
            TgcSceneLoader loader = new TgcSceneLoader();
            objects = new List<Crafteable>();
            objectsFactory = new ObjectsFactory(objects);

            //Crear SkyBox
            skyBox = new NVSkyBox[4];
            for (int i = 0; i < 4; i++) skyBox[i] = new NVSkyBox();
            skyBox[0].horario("maniana"); //cambiarlo "maniana" "dia" "tarde" "noche"
            skyBox[1].horario("dia"); //cambiarlo "maniana" "dia" "tarde" "noche"
            skyBox[2].horario("tarde"); //cambiarlo "maniana" "dia" "tarde" "noche"
            skyBox[3].horario("noche"); //cambiarlo "maniana" "dia" "tarde" "noche"
            for (int i = 0; i < 4; i++) skyBox[i].init();
            GuiController.Instance.BackgroundColor = Color.DarkGreen;

            //creo el personaje con su altura en el mapa
            Vector3 posicionPersonaje = new Vector3(1000, currentWorld.calcularAltura(1000, 1000), 1000);
            personaje = objectsFactory.createHuman(posicionPersonaje, new Vector3(2, 2, 2));

            leon = currentWorld.crearLeon(posicionPersonaje.X - 390, posicionPersonaje.Z - 490);

            currentWorld.crearMadera(posicionPersonaje.X - 90, posicionPersonaje.Z - 90);
            currentWorld.crearPiedra(posicionPersonaje.X + 90, posicionPersonaje.Z + 90);
            currentWorld.crearArbustoFruta(posicionPersonaje.X - 40, posicionPersonaje.Z - 90);
            currentWorld.crearArbustoFruta(posicionPersonaje.X - 40, posicionPersonaje.Z -590);
            currentWorld.crearFruta(posicionPersonaje.X + 70, posicionPersonaje.Z + 90);
            
            //Hud
            hud = new Hud();
            
            //log
            log = GuiController.Instance.Logger;
            log.clear();
            log.log("Inicio Juego", Color.Brown);

            //cam
            cam = new NVCamaraFps(personaje);
            cam.alturaPreseteada = 100;
            cam.setCamera(personaje.getPosition(), personaje.getPosition() + new Vector3(50f, 0, 0));
            input.EnableMouseSmooth = true;
            
            

            sounds = new Sounds();
            sounds.playMusic();
            personaje.setSounds(sounds);

            //Cargar mesh WILSON
            wilson = loader.loadSceneFromFile("AlumnoEjemplos\\NatusVincere\\InventarioYObjetos\\Wilson\\wilson-TgcScene.xml").Meshes[0];
            float wilsonX = 1000 - 300;
            float wilsonZ = 1000 - 300;
            wilson.Position = new Vector3(wilsonX,
                currentWorld.calcularAltura(wilsonX, wilsonZ) + 10,
                wilsonZ);
            
            //Cargar shader con efectos de Post-Procesado
            effect = TgcShaders.loadEffect("AlumnoEjemplos\\NatusVincere\\EfectosEspeciales\\PostProcess.fx");
            //Configurar Technique dentro del shader
            effect.Technique = "RainTechnique";    
            //Cargar textura que se va a dibujar arriba de la escena del Render Target
            lluviaTexture = TgcTexture.createTexture(d3dDevice, "AlumnoEjemplos\\NatusVincere\\EfectosEspeciales\\efecto_rain.png");

            alarmaTexture = TgcTexture.createTexture(d3dDevice, "AlumnoEjemplos\\NatusVincere\\EfectosEspeciales\\efecto_alarma.png");

            //Interpolador para efecto de variar la intensidad de la textura de alarma
            intVaivenAlarm = new InterpoladorVaiven();
            intVaivenAlarm.Min = 0;
            intVaivenAlarm.Max = 1;
            intVaivenAlarm.Speed = 5;
            intVaivenAlarm.reset();

            //Activamos el renderizado customizado. De esta forma el framework nos delega control total sobre como dibujar en pantalla
            //La responsabilidad cae toda de nuestro lado
            GuiController.Instance.CustomRenderEnabled = true;

            //Se crean 2 triangulos (o Quad) con las dimensiones de la pantalla con sus posiciones ya transformadas
            // x = -1 es el extremo izquiedo de la pantalla, x = 1 es el extremo derecho
            // Lo mismo para la Y con arriba y abajo
            // la Z en 1 simpre
            CustomVertex.PositionTextured[] screenQuadVertices = new CustomVertex.PositionTextured[]
            {
                new CustomVertex.PositionTextured( -1, 1, 1, 0,0),
                new CustomVertex.PositionTextured(1,  1, 1, 1,0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0,1),
                new CustomVertex.PositionTextured(1,-1, 1, 1,1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                    4, d3dDevice, Usage.Dynamic | Usage.WriteOnly,
                        CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            //Creamos un Render Targer sobre el cual se va a dibujar la pantalla
            renderTarget2D = new Texture(d3dDevice, d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                        Format.X8R8G8B8, Pool.Default);
            g_pDSShadow = d3dDevice.CreateDepthStencilSurface(d3dDevice.PresentationParameters.BackBufferWidth
                    , d3dDevice.PresentationParameters.BackBufferHeight,
                                                             DepthFormat.D24S8,
                                                             MultiSampleType.None,
                                                             0,
                                                             true);

            time = 0;
            timeAcumParaCambioDeHorario = 0;
            timeAcumParaLluvia = 0;
            timeRed = 0;

            //presentacion init
            presentacion = true;
            continuar = false;
            cartelContinuar.Text = "PRESIONE ENTER PARA CONTINUAR";
            cartelContinuar.Color = Color.Red;
            cartelContinuar.changeFont(TgcDrawText.VERDANA_10);
            cartelContinuar.Align = TgcText2d.TextAlign.CENTER;
            cartelContinuar.Position = new Point(0, (int)screenSize.Height / 6 * 5);
            Cursor.Hide();

            
        }

        public override void render(float elapsedTime)
        {
            renderVictoria();
            time += elapsedTime;
            timeAcumParaCambioDeHorario += elapsedTime;
            timeAcumParaLluvia += elapsedTime;
            //Cargamos el Render Targer al cual se va a dibujar la escena 3D. Antes nos guardamos el surface original
            //En vez de dibujar a la pantalla, dibujamos a un buffer auxiliar, nuestro Render Target.
            pOldRT = d3dDevice.GetRenderTarget(0);
            Surface pSurf = renderTarget2D.GetSurfaceLevel(0);
            d3dDevice.SetRenderTarget(0, pSurf);

            Surface pOldDS = d3dDevice.DepthStencilSurface;
            d3dDevice.DepthStencilSurface = g_pDSShadow;

            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            //Arrancamos el renderizado. Esto lo tenemos que hacer nosotros a mano porque estamos en modo CustomRenderEnabled = true
            d3dDevice.BeginScene();
            
            //Como estamos en modo CustomRenderEnabled, tenemos que dibujar todo nosotros, incluso el contador de FPS
            GuiController.Instance.Text3d.drawText("FPS: " + HighResolutionTimer.Instance.FramesPerSecond, 0, 0, Color.Yellow);

            //Tambien hay que dibujar el indicador de los ejes cartesianos
            //GuiController.Instance.AxisLines.render();

            //Renderizo el logo del inicio y el hud
            #region presentacion
            if (presentacion)
            {

                //animacion

                if (lookfrom.Y - 250f > currentWorld.calcularAltura(lookfrom.X, lookfrom.Z)) lookfrom.Y += (elapsedTime * -150f); //animacion
                if (lookfrom.X < targetCamara3.X & lookfrom.X < 15) lookfrom.X += (elapsedTime * 150f); //animacion
                if (lookfrom.Z > targetCamara3.Z & lookfrom.Z > -15) lookfrom.Z += (elapsedTime * -100f); //animacion
               
                lookAt = personaje.getPosition(); //animacion
                Matrix lookAtM = Matrix.LookAtLH(lookfrom, lookAt, vNormal); //animacion
                Matrix result = lookAtM; //animacion
                d3dDevice.Transform.View = result; //animacion
                personaje.rotateY(elapsedTime * .3f);  //animacion
                personaje.meshRender();
                personaje.move(lookfrom - lookAt); //animacion
                

                if (!continuar)
                {
                    lookfrom = new Vector3(-2500, 2400, 2000); //animacion
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    spriteLogo.render();

                    if ( (timeRed>14))
                    {
                        cartelContinuar.Color = Color.Transparent;
                        timeRed ++;
                        if (timeRed == 30) timeRed = 0;
                    }
                    else
                    {
                        cartelContinuar.Color = Color.Red;
                        timeRed ++;
                    }

                    cartelContinuar.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    if (input.keyDown(Key.Return) && !victoria)
                    {
                        victoria = false;
                        continuar = true;
                        spriteLogo.dispose();
                    }
                    if (input.keyDown(Key.Return) && victoria)
                    {
                        lookfrom = new Vector3(-2500, 2400, 2000);
                        continuar = false;
                        spriteWin.dispose();
                        presentacion = true;
                    }
                }
                else
                {
                    
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    spriteObjetivos.render();
                    cartelContinuar.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                    
                    if (input.keyDown(Key.Return))
                    {
                        presentacion = false;
                        spriteObjetivos.dispose();
                        cartelContinuar.dispose();
                    }
                }

            }
            else //render del hud
            {
                
                hud.renderizate(personaje);
                if (personaje.inventory.hachaEquipada)
                {
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    hachaEnMano.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                }
            }

            #endregion presentacion
            
            Wind.generarViento(getAllObjects(), elapsedTime, sounds);

           
            #region crafteo

            if (input.keyDown(Key.E))
            {
                currentWorld.transform(personaje, sounds);
            }

            if (input.keyDown(Key.R))
            {
                currentWorld.objects.ForEach(crafteable => { if (crafteable.isNear(personaje)) personaje.store(crafteable); });
            }

            if (input.keyDown(Key.L))
            {
                personaje.leaveObject(currentWorld);
            }

            #endregion crafteo
            
            //Frustum values FAR PLANE
            d3dDevice.Transform.Projection =
                Matrix.PerspectiveFovLH(((float)((45.0f)* Math.PI / 180)),
                (screenSize.Width/screenSize.Height), 1f, 99999999f);
            


            personaje.setWorld(currentWorld);

            //recalculo la vida del jugador segun el tiempo transcurrido
            personaje.recalcularStats(elapsedTime);
            //Actualizar personaje
            personaje.inventory.update();
            personaje.inventory.render();

            //Actualizar Skybox
            skyBox[horaDelDia].updateYRender(personaje.getPosition()); //lo dibuja y lo mueve con centro en el personaje
            if (timeAcumParaCambioDeHorario > 15) cambioHorario();
            

            currentXCam = cam.getPosition().X;
            currentZCam = cam.getPosition().Z;
            alturaCam = currentWorld.calcularAltura(currentXCam, currentZCam);
            cam.Enable = true;
            cam.setPosition(new Vector3(currentXCam, alturaCam + cam.alturaPreseteada, currentZCam));
            personaje.setPosition(new Vector3(currentXCam, alturaCam, currentZCam));
            personaje.render(); //no renderiza el mesh, solo actualiza valores y lo mata
            refreshWorlds();
            //personaje.refresh(currentWorld, -cam.viewDir, elapsedTime);
            


            GuiController.Instance.Frustum.updateVolume(GuiController.Instance.D3dDevice.Transform.View,
            GuiController.Instance.D3dDevice.Transform.Projection);

            personaje.setBB(personaje.getPosition());
            
            renderWorlds();

            //personaje.Render(); //renderiza solo el BC
            leon.Render();
            wilson.render();
            chequearVictoria();
            
            //Terminamos manualmente el renderizado de esta escena. Esto manda todo a dibujar al GPU al Render Target que cargamos antes
            d3dDevice.EndScene();

            //Liberar memoria de surface de Render Target
            pSurf.Dispose();

            //Ahora volvemos a restaurar el Render Target original (osea dibujar a la pantalla)
            d3dDevice.DepthStencilSurface = pOldDS;
            d3dDevice.SetRenderTarget(0, pOldRT);

            //Luego tomamos lo dibujado antes y lo combinamos con una textura con efecto de alarma
            if (timeAcumParaLluvia > 20) activarLluvia();
            if (timeAcumParaLluvia > 50) desactivarLluvia();

            PostProcessing.drawPostProcess(d3dDevice, effect, screenQuadVB, intVaivenAlarm, renderTarget2D, lluviaTexture, time);


            if (leon.isNear(personaje))
            {
                leon.acercateA(personaje, currentWorld, elapsedTime);
                PostProcessing.drawPostProcess(d3dDevice, effect, screenQuadVB, intVaivenAlarm, renderTarget2D, alarmaTexture, time);
            }
        }

        private void chequearVictoria()
        {
            if (TgcCollisionUtils.testAABBAABB(wilson.BoundingBox, personaje.getMesh().BoundingBox))
            {
                PostProcessing.lluviaActivada = false;
                sounds.playVictoria();
                victoria = true;
            }
        }

        private void activarLluvia()
        {
            PostProcessing.lluviaActivada = true;
            sounds.playRain();
        }
        private void desactivarLluvia()
        {
            timeAcumParaLluvia = 0;
            PostProcessing.lluviaActivada = false;
            sounds.stopRain();
        }

        private bool FullScreen()
        {
            DialogResult result = MessageBox.Show("Che, ¿queres verlo mejor en fullscreen?", "Confirmación", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }

        private void cambioHorario()
        {
            timeAcumParaCambioDeHorario = 0;
            if (horaDelDia < 3)
            {
                horaDelDia++;
            }
            else
            {
                horaDelDia = 0;
            }
        }
            

        
        public void refreshWorlds()
        {
            Vector3 logicPosition = personaje.getPosition() - currentWorld.position;
            
            int size = worldSize / 2;
            if (logicPosition.X > size)
            {
                Vector3 newPosition = personaje.getPosition();
                newPosition.X = -size;

                copyWorlds();
                for (int i = 0; i <= 2; i++)
                {
                    savedWorlds[i][0].move(new Vector3(size * 6, 0, 0));
                    worlds[i][2] = savedWorlds[i][0];
                    worlds[i][1] = savedWorlds[i][2];
                    worlds[i][0] = savedWorlds[i][1];
                }
            }
            if (logicPosition.X < -size)
            {
                flag = 1;
                Vector3 newPosition = personaje.getPosition();
                newPosition.X = size;
                copyWorlds();
                for (int i = 0; i <= 2; i++)
                {
                    savedWorlds[i][2].move(new Vector3(size * -6, 0, 0));
                    worlds[i][2] = savedWorlds[i][1];
                    worlds[i][1] = savedWorlds[i][0];
                    worlds[i][0] = savedWorlds[i][2];
                }
            }
            if (logicPosition.Z > size)
            {
                flag = 1;
                Vector3 newPosition = personaje.getPosition();
                newPosition.Z = -size;
                copyWorlds();

                for (int i = 0; i <= 2; i++)
                {
                    savedWorlds[2][i].move(new Vector3(0, 0, size * 6));
                    worlds[0][i] = savedWorlds[2][i];
                    worlds[1][i] = savedWorlds[0][i];
                    worlds[2][i] = savedWorlds[1][i];
                }
            }
            if (logicPosition.Z < -size)
            {
                flag = 1;
                flag = 1;
                Vector3 newPosition = personaje.getPosition();
                newPosition.Z = size;
                copyWorlds();
                //personaje.setPosition(newPosition);
                for (int i = 0; i <= 2; i++)
                {
                    savedWorlds[0][i].move(new Vector3(0, 0, size * -6));
                    worlds[1][i] = savedWorlds[2][i];
                    worlds[2][i] = savedWorlds[0][i];
                    worlds[0][i] = savedWorlds[1][i];
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
            text.dispose();
        }

        public void renderVictoria()
        {
            if (victoria) {
               /* lookfrom = new Vector3(-2500, 2400, 2000);
                GuiController.Instance.Drawer2D.beginDrawSprite();
                spriteWin.render();

                if ((timeRed > 14))
                {
                    cartelContinuar.Color = Color.Transparent;
                    timeRed++;
                    if (timeRed == 30) timeRed = 0;
                }
                else
                {
                    cartelContinuar.Color = Color.Red;
                    timeRed++;
                }

                cartelContinuar.render();
                GuiController.Instance.Drawer2D.endDrawSprite();*/
            }
        }
        public void copyWorlds()
        {
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    savedWorlds[i][j] = worlds[i][j];
                }
            }
        }

         
        public void renderWorlds()
        {
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    worlds[i][j].rendered = false;
                }

            }

            int constante = -200;
            Vector3 viewDir = new Vector3(cam.viewDir.X, 0, cam.viewDir.Z);
            Vector3 logicPosition = personaje.getPosition() - currentWorld.position;
            Vector3 personajePosition = personaje.getPosition();
            worlds[1][1].render();
           /* if (logicPosition.X < -3000)
            {
                worlds[1][0].render();
            } else if (logicPosition.X > 3000)
            {
                worlds[1][2].render();
            }

            if (logicPosition.Z < -3000)
            {
                worlds[2][1].render();
            }
            else if (logicPosition.Z > 3000)
            {
                worlds[0][1].render();
            }*/

            if ((viewDir.X > constante && viewDir.Z > constante) || sectorToRender == 1)
            {
                sectorToRender = 1;
                worlds[0][1].render();
                worlds[0][2].render();
                worlds[1][2].render();
            }

            if ((viewDir.X < -constante && viewDir.Z > constante) || sectorToRender == 2)
            {
                sectorToRender = 2;
                worlds[0][1].render();
                worlds[0][0].render();
                worlds[1][0].render();
            }

            if ((viewDir.X > constante && viewDir.Z < -constante) || sectorToRender == 3)
            {
                sectorToRender = 3;
                worlds[1][2].render();
                worlds[2][1].render();
                worlds[2][2].render();
            }

            if ((viewDir.X < -constante && viewDir.Z < -constante) || sectorToRender == 4)
            {
                sectorToRender = 4;
                worlds[1][0].render();
                worlds[2][0].render();
                worlds[2][1].render();
            }
        }

        public List<Crafteable> getAllObjects()
        {
            List<Crafteable> crafteables = new List<Crafteable>();
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    crafteables.AddRange(worlds[i][j].objects);
                }

            }
            return crafteables;
        }

        public override void close()
        {
            //Al hacer dispose del original, se hace dispose automáticamente de todas las instancias
            //pasto.dispose();
            for (int i = 0; i < 4; i++) skyBox[i].dispose();
            personaje.dispose();
            currentWorld.dispose();
            leon.dispose();
            hachaEnMano.dispose();
            //hud.dispose();
            cam.Enable = false; //para q deje de capturar el mouse
            Cursor.Show();
        }
        

    }
}
