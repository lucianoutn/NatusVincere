using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer;

namespace AlumnoEjemplos.NatusVincere
{
    public class Leon
    {
        private int health;
        private float minimumDistance = 400; //Default
        private TgcMesh mesh;
        private TgcBoundingBox arbustoBB;

        public Leon(TgcMesh mesh, Vector3 position, Vector3 scale)
        {
            this.health = 20;
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
            setBB(position);
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
            arbustoBB.render();
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

        public void acercateA(Human personaje, World currentWorld)
        {
            float xL = this.getPosition().X;
            float zL = this.getPosition().Z;
            float xP = personaje.getPosition().X;
            float zP = personaje.getPosition().Z - 40;

            //if(distancia(personaje).Length()>80)
            //{
            if(Math.Abs(Math.Abs(xL) - Math.Abs(xP))>40)
            {
                if (xL > xP)
                {
                    xL = xL - 1;
                }
                else
                {
                    xL = xL + 1;
                }
            }
            if(Math.Abs(Math.Abs(zL) - Math.Abs(zP)) > 40)
            {
                if (zL > zP)
                {
                    zL = zL - 1;
                }
                else
                {
                    zL = zL + 1;
                }
            }
            //}
            this.setPosition(new Vector3(xL, currentWorld.calcularAltura(xL, zL),zL));
            this.setBB(new Vector3(xL, currentWorld.calcularAltura(xL, zL), zL));
        }

        private bool hayColision(Human personaje)
        {
            if (TgcCollisionUtils.testAABBCylinder(this.getBB(), personaje.getBB()))
            {
                return true;
            }
            return false;
        }
    }
}
