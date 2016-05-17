using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere.InventarioYObjetos.Pino
{
    public class Pino : Crafteable
    {
        public Pino(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 1;
            this.description = "Pino";
            this.minimumDistance = 200;
            this.storable = false;
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
