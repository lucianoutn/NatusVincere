using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using System;

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
        private DateTime tActual;
        private DateTime tAnterior;
        private TimeSpan tTranscurridoVida = TimeSpan.Zero;
        private TimeSpan tTranscurridoAgua = TimeSpan.Zero;
        private TimeSpan tTranscurridoSuenio = TimeSpan.Zero;


        public Human(Inventory inventory, TgcSkeletalMesh mesh, Vector3 position, Vector3 scale)
        {
            this.inventory = inventory;
            this.mesh = mesh;
            this.mesh.Position = position;
            this.mesh.Scale = scale;
            this.health = 101;
            this.agua = 101;
            this.suenio = -1;
        }

        public void recalcularStats()
        {
            this.tActual = DateTime.Now;
            tTranscurridoVida = tTranscurridoVida + tActual.Subtract(this.tAnterior);
            tTranscurridoAgua = tTranscurridoAgua + tActual.Subtract(this.tAnterior);
            tTranscurridoSuenio = tTranscurridoSuenio + tActual.Subtract(this.tAnterior);
         
            if (tTranscurridoVida.TotalSeconds > 30 )
            {
                this.health = health - 1;
                tTranscurridoVida = TimeSpan.Zero;
            }
            if (tTranscurridoAgua.TotalMinutes > 1)
            {
                this.agua = this.agua-1;
                tTranscurridoAgua = TimeSpan.Zero;
            }
            if (tTranscurridoSuenio.TotalMinutes > 2)
            {
                this.suenio = this.suenio + 1;
                tTranscurridoSuenio = TimeSpan.Zero;
            }
           
            this.tAnterior = this.tActual;
        }

        public void leaveObject()
        {
            this.inventory.leaveObject(this.getPosition());
        }

        public void render()
        {
            this.mesh.render();
        }

        public void store(Crafteable item) {
            this.inventory.addItem(item);
            item.addToInventory();
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
    }
}
