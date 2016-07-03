using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class ArbustoFruta : Crafteable
    {
        public new int uses = 3;
        public new int type = 3;
        private TgcBoundingBox piedraBB;

        public ArbustoFruta(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 3;
            this.description = "ArbustoFruta";
            this.minimumDistance = 200;
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
            return this.piedraBB;
        }

        public override void Render()
        {
            piedraBB.render();
        }

        public override void borrarBB()
        {
            this.piedraBB.dispose();
            this.piedraBB = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.piedraBB = new TgcBoundingBox(new Vector3(position.X +20, position.Y + 48, position.Z + 15), new Vector3(position.X + 55, position.Y + 228, position.Z + 55));
        }
    }
}
