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
        private TgcBoundingBox tronco;

        public Pino(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 1;
            this.description = "Pino";
            this.minimumDistance = 200;
            this.storable = false;
            setBB(position);
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

        public override TgcBoundingBox getBB()
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
            this.tronco = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.tronco = new TgcBoundingBox(new Vector3(position.X - 10, position.Y, position.Z - 10), new Vector3(position.X + 10, position.Y + 80, position.Z + 10));
        }
    }
}
