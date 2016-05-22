using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Fogata : Crafteable
    {
        public new int uses = 20;
        public new int type = 6;
        private TgcBoundingSphere fogataBB;

        public Fogata(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 6;
            this.description = "Fogata";
            this.minimumDistance = 200;
            this.storable = true;
            this.fogataBB = new TgcBoundingSphere(new Vector3(position.X, position.Y + 14, position.Z), 12.75f);
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
    }
}
