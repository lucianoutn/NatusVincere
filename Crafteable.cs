using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    class Crafteable
    {
        public int type;
        public string description = "Crafteable Object Description ";
        public int uses = 8; // Cantidad de usos
        public int status = 1; //1: New. 2: Used; 3: inInventory; 4: Usign; 5: Disabled;
        public float minimumDistance = 100; //Default
        private Human owner;
        private TgcMesh mesh;
        public int[] canCombineWith;

        public Crafteable(TgcMesh mesh, Vector3 position, Vector3 scale)
        {
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
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

        public void drop(Vector3 position)
        {
            this.setPosition(position);
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

        private void destroy()
        {
            this.uses = 0;
            this.status = 4;
        }

        public void render()
        {
            if (this.getStatus() == 3) return;

            this.mesh.render();
        }

        public void move(Vector3 movement)
        {
            this.mesh.move(movement);
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
            return 100;
        }

        public bool checkIfCombine(Crafteable item)
        {
            int i = 0;
            bool canCombine = false;
            for (i = 0; i < this.canCombineWith.Length; i++)
            {
                canCombine |= item.getType() == this.canCombineWith[i];
                
            }
            return canCombine;
        }

        public virtual int getType()
        {
            return this.type;
        }

        public virtual int getStatus()
        {
            return this.status;
        }

        public void dispose()
        {
            this.status = 5;
            this.mesh.dispose();
        }
    }
}
