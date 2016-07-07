using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;
using TgcViewer.Utils._2D;
using System.Drawing;

namespace AlumnoEjemplos.NatusVincere
{
    public class Leon
    {
        private float minimumDistance = 400; //Default
        bool quieto = true;
        bool ataco = false;
        int coolDownTotal = 120;
        int coolDown = 120;
        private TgcMesh mesh;
        private TgcBoundingBox arbustoBB;

        TgcSprite spriteMordida;
        TgcSprite spriteObjetivos;
        //TgcText2d objetivos;
        float tiempoMordida;
        Size screenSize;

        public Leon(TgcMesh mesh, Vector3 position, Vector3 scale)
        {
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
            setBB(position);

            //Creo un sprite de logo inicial
            spriteMordida = new TgcSprite();
            spriteMordida.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\InventarioYObjetos\\Leon\\mordida.png");
            tiempoMordida = 15f;
            screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSizeLogo = spriteMordida.Texture.Size;
            spriteMordida.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSizeLogo.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSizeLogo.Height / 2, 0));

        }

        public void doAction(Human user)
        {
            Vector3 direction = this.getPosition() - user.getPosition();
            direction.Normalize();
            this.move(direction);
        }

        public void move(Vector3 movement)
        {
            this.mesh.move(movement);
        }

        public Vector3 distancia(Human user)
        {
            Vector3 distance = user.getPosition();
            distance.Multiply(-1);
            distance.Add(this.getPosition());

            return distance;
        }

        public bool isNear(Human user)
        {
            Vector3 distance = distancia(user);
            //TODO: Agregar checkear la dirección del personaje
            return distance.Length() < this.getMinimumDistance();
        }

        public virtual float getMinimumDistance()
        {
            return minimumDistance;
        }

        public Vector3 getPosition()
        {
            return this.mesh.Position;
        }

        public void setPosition(Vector3 position)
        {
            this.mesh.Position = position;
        }


        public TgcBoundingBox getBB()
        {
            return this.arbustoBB;
        }
        
        public void Render()
        {
            mesh.render();
        }

        public void borrarBB()
        {
            this.arbustoBB.dispose();
            this.arbustoBB = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public void setBB(Vector3 position)
        {
            this.arbustoBB = new TgcBoundingBox(new Vector3(position.X + 25, position.Y, position.Z + 70), new Vector3(position.X - 10, position.Y + 38, position.Z + 25));
        }

        public void acercateA(Human personaje, World currentWorld, float elapsedTime)
        {
            float xL = this.getPosition().X;
            float zL = this.getPosition().Z;
            float xP = personaje.getPosition().X;
            float zP = personaje.getPosition().Z - 40;

            quieto = true;

            if (Math.Abs(Math.Abs(xL) - Math.Abs(xP))>40)
            {
                if (xL > xP)
                {
                    xL = xL - 5;
                }
                else
                {
                    xL = xL + 5;
                }

                quieto = false;
            }
            if(Math.Abs(Math.Abs(zL) - Math.Abs(zP)) > 30)
            {
                if (zL > zP)
                {
                    zL = zL - 5;
                }
                else
                {
                    zL = zL + 5;
                }

                quieto = false;
            }


            if ( (distancia(personaje).Length() < 100) && (quieto==true) && (coolDown >= coolDownTotal))
            {
                this.atacarA(personaje);
                coolDown = 0;
                ataco = true;
            }
            

            if (coolDown < coolDownTotal)
            {
                coolDown++;
                ataco = false;
                if (coolDown < 5)
                {
                    GuiController.Instance.Drawer2D.beginDrawSprite();
                    spriteMordida.render();
                    GuiController.Instance.Drawer2D.endDrawSprite();
                }
            }

            this.setPosition(new Vector3(xL, currentWorld.calcularAltura(xL, zL),zL));
            this.setBB(new Vector3(xL, currentWorld.calcularAltura(xL, zL), zL));

          
            
            
        }

        private void atacarA(Human personaje)
        {
            personaje.causarDaño(70);
        }

        private bool hayColision(Human personaje)
        {
            if (TgcCollisionUtils.testAABBCylinder(this.getBB(), personaje.getBB()))
            {
                return true;
            }
            return false;
        }

        public void dispose()
        {
            spriteMordida.dispose();
        }

        
    }
}
