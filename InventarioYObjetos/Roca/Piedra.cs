using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Piedra : Crafteable
    {
        public new int uses = 3;
        public new int type = 2;

        public Piedra(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 2;
            this.description = "Piedra";
            this.minimumDistance = 200;
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

    }
}
