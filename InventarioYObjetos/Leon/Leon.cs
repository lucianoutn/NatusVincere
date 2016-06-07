using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Leon : Crafteable
    {
        public new int uses = 3;
        public new int type = 2;
        private float radioBB = 10.75f;
        TgcBoundingBox arbustoBB;

        public Leon(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 2;
            this.description = "Leon";
            this.minimumDistance = 200;
            this.storable = true;
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
            return this.arbustoBB;
        }
        
        public override void Render()
        {
            arbustoBB.render();
        }

        public override void borrarBB()
        {
            this.arbustoBB.dispose();
            this.arbustoBB = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.arbustoBB = new TgcBoundingBox(new Vector3(position.X + 25, position.Y, position.Z + 70), new Vector3(position.X - 10, position.Y + 38, position.Z + 25));
        }
    }
}
