using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Arbusto : Crafteable
    {
        public new int uses = 3;
        public new int type = 2;
        TgcBoundingSphere arbustoBB;

        public Arbusto(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 2;
            this.description = "Arbusto";
            this.minimumDistance = 200;
            this.arbustoBB = new TgcBoundingSphere(new Vector3(position.X, position.Y + 8, position.Z), 10.75f);
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
            return this.arbustoBB;
        }
        public override void Render()
        {
            arbustoBB.render();
        }
    }
}
