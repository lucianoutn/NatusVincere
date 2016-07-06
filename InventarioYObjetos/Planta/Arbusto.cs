using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Arbusto : Crafteable
    {
        public new int uses = 3;
        public new int type = 3;
        TgcBoundingBox BC;

        public Arbusto(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 3;
            this.description = "Arbusto";
            this.minimumDistance = 230f;
            storable = true;
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
            return this.BC;
        }

        public override void Render()
        {
            BC.render();
        }

        public override void borrarBB()
        {
            this.BC.dispose();
            this.BC = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.BC = new TgcBoundingBox(new Vector3(position.X - 30, position.Y, position.Z - 15), new Vector3(position.X + 10, position.Y + 35, position.Z + 10));
        }
    }
}
