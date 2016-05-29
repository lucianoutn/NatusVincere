using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Fogata : Crafteable
    {
        public new int uses = 20;
        public new int type = 6;
        private float fogataR = 12.75f;
        TgcBoundingSphere fogataBB;

        public Fogata(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 6;
            this.description = "Fogata";
            this.minimumDistance = 200;
            this.storable = true;
            this.fogataBB = new TgcBoundingSphere(new Vector3(position.X+10, position.Y+ 10, position.Z+10), fogataR);
        }

        public override void doAction(Human user)
        {
            user.health++;
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
            return fogataBB;
        }


        public override void Render()
        {
            fogataBB.render();
        }

        public override void borrarBB()
        {
            this.fogataBB.dispose();
            this.fogataBB = new TgcBoundingSphere(new Vector3(0f, 0f, 0f), fogataR);
        }

        public override void setBB(Vector3 position)
        {
            this.fogataBB = new TgcBoundingSphere(new Vector3(position.X+10, position.Y + 10, position.Z + 10), fogataR);
        }
    }
}
