using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.NatusVincere
{
    public class Crafteable
    {
        public int type;
        public string description = "Crafteable Object Description ";
        public int uses = 8; // Cantidad de usos
        public int status = 1; //1: New. 2: Used; 3: inInventory; 4: Usign; 5: Disabled;
        public float minimumDistance = 100; //Default
        private Human owner;
        private TgcMesh mesh;
        public bool storable;
        public bool consumible;
        private TgcBoundingBox BB;

        public Crafteable(TgcMesh mesh, Vector3 position, Vector3 scale)
        {
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
            this.storable = true;
            this.consumible = false;
            this.setBB(position);
            this.mesh.AlphaBlendEnable = true;
        }

       public void use(Human user)
        {
            if (!this.isNear(user)) return;

            this.doAction(user);
            this.uses--;

            if (uses == 0)
            {
                this.destroy();
            }
        }

        public bool isStorable()
        {
            return this.storable;
        }

       
        public void drop(Vector3 position)
        {
            this.setPosition(position);
            this.setBB(position);
            this.status = 1;
        }

        public virtual void doAction(Human user)
        {
            return;
        }

        public void addUses(int uses)
        {
            this.uses += uses;
        }

        public void addToInventory()
        {
            this.borrarBB();
            this.status = 3;
        }

        public bool isNear(Human user)
        {
            Vector3 distance = user.getPosition();
            distance.Multiply(-1);
            distance.Add(this.getPosition());
            //TODO: Agregar checkear la dirección del personaje
            return distance.Length() < this.getMinimumDistance();
        }

        public void destroy()
        {
            this.uses = 0;
            this.status = 5;
        }

        public void render()
        {
            if (this.getStatus() == 3) return;

            this.mesh.render();
        }

        public void move(Vector3 movement)
        {
            this.setPosition(this.getPosition() + movement);
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
            this.setBB(position);
            this.mesh.Position = position;
        }

        public virtual float getMinimumDistance()
        {
            return 100;
        }

        public virtual int getType()
        {
            return this.type;
        }

        public virtual bool getConsumible()
        {
            return this.consumible;
        }

        public virtual void consumir(Human dueño)
        {
            
        }


        public virtual int getStatus()
        {
            return this.status;
        }

        public TgcMesh getMesh()
        {
            return this.mesh;
        }

        public virtual TgcBoundingBox getBB()
        {
            return this.BB;
        }

        public virtual void borrarBB()
        {
        }

        public virtual void setBB(Vector3 position)
        {
            Vector3 centro = getMesh().BoundingBox.calculateBoxCenter();
            this.BB = new TgcBoundingBox(new Vector3(centro.X, centro.Y - 4, centro.Z), new Vector3(centro.X + 50, centro.Y + 40, centro.Z + 50));
        }
        public void dispose()
        {
            this.status = 5;
            this.mesh.dispose();
        }

        public virtual void Render()
        {
        }

        public void setEfecto(string shaderPath, string technique)
        {
            this.getMesh().Effect = TgcShaders.loadEffect(shaderPath);
            this.getMesh().Technique = technique;
        }

        public  Effect getEfecto()
        {
            return this.getMesh().Effect;
        }

    }
}
