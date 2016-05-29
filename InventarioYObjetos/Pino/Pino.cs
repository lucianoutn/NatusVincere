using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Pino : Crafteable
    {
        public new int uses = 3;
        public new int type = 1;
        private float pinoBB = 20;
        private TgcBoundingSphere tronco;

        public Pino(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 1;
            this.description = "Pino";
            this.minimumDistance = 200;
            this.storable = false;
            this.tronco = new TgcBoundingSphere(new Vector3(position.X, position.Y + 90, position.Z), pinoBB);
        }
        public override void doAction(Human user)
        {
            Vector3 direction = this.getPosition() - user.getPosition();
            direction.Normalize();
            this.move(direction);
        }

        public override float getMinimumDistance()
        {
            return this.minimumDistance;
        }
        public override int getType()
        {
            return this.type;
        }

        public override TgcBoundingSphere getBB()
        {
            return this.tronco;
        }

        public override void Render()
        {
            tronco.render();
        }

        public override void borrarBB()
        {
            this.tronco.dispose();
            this.tronco = new TgcBoundingSphere(new Vector3(0f, 0f, 0f), pinoBB);
        }

        public override void setBB(Vector3 position)
        {
            this.tronco = new TgcBoundingSphere(new Vector3(position.X, position.Y + 45, position.Z), pinoBB);
        }
    }
}
