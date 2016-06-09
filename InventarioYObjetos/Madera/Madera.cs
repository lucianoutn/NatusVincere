using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Madera : Crafteable
    {
        public new int uses = 3;
        public new int type = 3;
        private float maderaR = 10.75f;
        private TgcBoundingBox tronco;

        public Madera(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 5;
            this.description = "Madera";
            this.minimumDistance = 200;
            this.status = 1;
            this.tronco = new TgcBoundingBox(new Vector3(position.X-10, position.Y+10, position.Z-40), new Vector3(position.X + 20, position.Y + 20, position.Z + 30));
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
            this.tronco = new TgcBoundingBox(new Vector3(position.X+10, position.Y + 10, position.Z), new Vector3(position.X + 10, position.Y + 10, position.Z));
        }
    }
}
