using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Fogata : Crafteable
    {
        public new int uses = 20;
        public new int type = 6;

        public Fogata(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 6;
            this.description = "Fogata";
            this.minimumDistance = 200;
            this.storable = true;
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

    }
}
