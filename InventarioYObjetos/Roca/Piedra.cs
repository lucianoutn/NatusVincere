using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Piedra : Crafteable
    {
        public new int uses = 3;
        public new int type = 2;
        private float piedraRadio = 12.75f;
        private TgcBoundingSphere piedraBB;

        public Piedra(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 2;
            this.description = "Piedra";
            this.minimumDistance = 200;
            this.piedraBB = new TgcBoundingSphere(new Vector3(position.X, position.Y + 14, position.Z), piedraRadio);
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
            return this.piedraBB;
        }

        public override void Render()
        {
            piedraBB.render();
        }

        public override void borrarBB()
        {
            this.piedraBB.dispose();
            this.piedraBB = new TgcBoundingSphere(new Vector3(0f, 0f, 0f), piedraRadio);
        }

        public override void setBB(Vector3 position)
        {
            this.piedraBB = new TgcBoundingSphere(new Vector3(position.X, position.Y + 8, position.Z), piedraRadio);
        }
    }
}
