using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Arbol : Crafteable
    {
        public new int uses = 3;
        public new int type = 1;
        private float radioBC = 98f;
        TgcBoundingSphere BC;

        public Arbol(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 1;
            this.description = "Arbol";
            this.minimumDistance = 130;
            this.storable = false;
            this.BC = new TgcBoundingSphere(new Vector3(position.X, position.Y + 187, position.Z), radioBC);
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
            return this.BC;
        }
        
        public override void Render()
        {
            BC.render();
        }

    }
}
