using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System;
using TgcViewer.Utils.Input;
using System.Collections.Generic;
using TgcViewer;


namespace AlumnoEjemplos.NatusVincere
{
    public class Human
    {

        public int type = 0; //For future
        public string description = "Human";
        public int status = 1; //1: New. 2: Used; 3: inInventory; 4: Usign; 5: Disabled;
        public int health;// = 100;
        public int agua;
        public int suenio;
        public float minimumDistance = 100; //Default
        private TgcSkeletalMesh mesh;
        public Inventory inventory;
        List<Crafteable> objects;
        private DateTime tActual;
        private DateTime tAnterior;
        private TimeSpan tTranscurridoVida = TimeSpan.Zero;
        private TimeSpan tTranscurridoAgua = TimeSpan.Zero;
        private TimeSpan tTranscurridoSuenio = TimeSpan.Zero;
        private TgcBoundingSphere BB;
        private TgcBoundingCylinder BC;
        private String animation = "Walk";
        private World currentWorld;

        public Human(Inventory inventory, TgcSkeletalMesh mesh, Vector3 position, Vector3 scale)
        {
            this.inventory = inventory;
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
            this.health = 101;
            this.agua = 101;
            this.suenio = -1;
            //this.BB = new TgcBoundingSphere(positionBS(position), 5.75f);
            this.BC = new TgcBoundingCylinder(positionBS(position), 5.75f, 15f);
            this.playAnimation(animationCaminar, false);
        }

        private Vector3 positionBS(Vector3 position)
        {
            return new Vector3(position.X, position.Y + 20, position.Z);
        }


        #region Movimientos //aplica para el personaje en 3era persona

        TgcD3dInput input;
        ObjectsFactory objectsFactory;
        string animationCaminar = "Walk";
        float velocidadCaminar = 150f;
        //float velocidadRotacion = 100f;
        //Calcular proxima posicion de personaje segun Input
        float moveForward = 0f;
        float rotate = 0;
        bool moving = false;
        bool rotating = false;
        float jump = 0;
        public const float DEFAULT_ROTATION_SPEED = 2f;

        public void movete(Key input, float rotAngle, float elapsedTimeSec) //heading = rotAngle
        {
            //this.move(

            //this.setPosition(vec);
            //Adelante
            if (input == Key.W)
            {
                moveForward = -velocidadCaminar;
                moving = true;
                //cam.getMovementDirection(input);
            }

            //Atras
            if (input == Key.S)
            {
                moveForward = velocidadCaminar;
                moving = true;
            }
            /*
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
            */
            //Jump
            if (input == Key.Space)
            {
                jump = 30;
                moving = true;
            }


            /*
            if (input == Key.E)
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(this)) objectsFactory.transform(crafteable); });
            }

            if (input == Key.R)
            {
                objects.ForEach(crafteable => { if (crafteable.isNear(this)) this.store(crafteable); });

            }

            if (input == Key.W)
            {
                this.leaveObject();
            }
            */
            //cam.getMovementDirection(input);




            //this.
            //this.setPosition(pos);
            //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
            //float rotAngle = ((float)Math.PI / 180) * (rotate);
            rotAngle *= DEFAULT_ROTATION_SPEED * elapsedTimeSec;
            this.rotateY(rotAngle);
            GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            //GuiController.Instance.FpsCamera.updateViewMatrix(d3dDevice);
            //this.playAnimation(animationCaminar, true);
            //this.updateAnimation();


            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Sin(this.getRotation().Y) * moveForward ,
                    jump,
                    FastMath.Cos(this.getRotation().Y) * moveForward );
                this.move(movementVector);

                //this.playAnimation(animationCaminar, true);
                //this.updateAnimation();
                moving = false;
            }

        }
        #endregion Movimientos

        public void recalcularStats()
        {
            this.tActual = DateTime.Now;
            tTranscurridoVida = tTranscurridoVida + tActual.Subtract(this.tAnterior);
            tTranscurridoAgua = tTranscurridoAgua + tActual.Subtract(this.tAnterior);
            tTranscurridoSuenio = tTranscurridoSuenio + tActual.Subtract(this.tAnterior);

            if (tTranscurridoVida.TotalSeconds > 30)
            {
                this.health = health - 1;
                tTranscurridoVida = TimeSpan.Zero;
            }
            if (tTranscurridoAgua.TotalMinutes > 1)
            {
                this.agua = this.agua - 1;
                tTranscurridoAgua = TimeSpan.Zero;
            }
            if (tTranscurridoSuenio.TotalMinutes > 2)
            {
                this.suenio = this.suenio + 1;
                tTranscurridoSuenio = TimeSpan.Zero;
            }

            this.tAnterior = this.tActual;
        }

        public void leaveObject(World world)
        {
            Crafteable crafteable = this.inventory.leaveObject(this.getPosition());
            world.addObject(crafteable);
            //Dejar objeto un poco mas lejos
        }

        public void render()
        {
            this.mesh.render();
            
        }

        public void store(Crafteable item) {
            this.inventory.addItem(item);
        }

        public void move(Vector3 movement)
        {
            this.mesh.move(movement);
            //this.playAnimation(animation, true);
            //this.updateAnimation();

        }

        public void scale(Vector3 scale)
        {
            this.mesh.Scale = scale;
        }

        public Vector3 getPosition()
        {
            return this.mesh.Position;
        }

        public void setPosition(Vector3 position)
        {
            this.mesh.Position = position;
        }

        public virtual float getMinimumDistance()
        {
            return this.minimumDistance;
        }

        public void rotateX(float angle)
        {
            this.mesh.rotateX(angle);
        }

        public void rotateY(float angle)
        {
            this.mesh.rotateY(angle);
        }

        public void rotateZ(float angle)
        {
            this.mesh.rotateZ(angle);
        }

        public Vector3 getRotation()
        {
            return this.mesh.Rotation;
        }

        public TgcSkeletalMesh getMesh()
        {
            return this.mesh;
        }

        public void playAnimation(string animation, bool playLoop)
        {
            this.mesh.playAnimation(animation, playLoop);
        }
        public void updateAnimation()
        {
            this.mesh.updateAnimation();
        }
        public void dispose()
        {
            this.mesh.dispose();
        }
        
        /* public void refresh(World currentWorld, Vector3 direction, float elapsedTime) {

            float velocidadCaminar = 5f;
            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            
            bool moving = false;
            float jump = 0;


            

            if (d3dInput.keyDown(Key.W))
            {
                moveForward = velocidadCaminar;
                moving = true;
                //cam.getMovementDirection(input);
            }

            //Atras
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            if (d3dInput.keyDown(Key.Space))
            {
                jump = 30;
                moving = true;
            }
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                direction.Normalize();
                
                movementVector = new Vector3(direction.X * moveForward,
                    jump,
                    direction.Z * moveForward);

                this.move(movementVector);
                this.updateAnimation();
                moving = false;
            }

            this.recalcularStats();
            this.inventory.update();
            Vector3 position = new Vector3(this.mesh.Position.X, currentWorld.calcularAltura(this.mesh.Position.X, this.mesh.Position.Z), this.mesh.Position.Z);
            this.setPosition(position);
            this.setBB(position);
        }
        */
       
        public void pickObject(World world)
        {
            world.objects.ForEach(crafteable => {
                if (crafteable.isNear(this))
                {
                    this.store(crafteable);
                }
            });
        }

        //public TgcBoundingSphere getBB()
        public TgcBoundingCylinder getBB()
        {
            //return BB;
            return BC;
        }

        public void setWorld(World world)
        {
            currentWorld = world;
        }
        public World getWorld()
        {
            return currentWorld;
        }

        public void setBB(Vector3 position)
        {
            //BB = new TgcBoundingSphere(positionBS(position), 5.75f);
            this.BC = new TgcBoundingCylinder(positionBS(position), 5.75f, 15f);
        }

        public void Render()
        {
            this.BC.render();

        }
    }
}
