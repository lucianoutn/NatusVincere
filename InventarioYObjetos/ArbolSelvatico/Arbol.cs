﻿using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NatusVincere
{
    public class Arbol : Crafteable
    {
        public new int uses = 3;
        public new int type = 3;
        TgcBoundingBox BC;

        public Arbol(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 1;
            this.description = "Arbol";
            this.minimumDistance = 230f;
            storable = false;
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
            this.BC = new TgcBoundingBox(new Vector3(0f,0f,0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.BC = new TgcBoundingBox(new Vector3(position.X- 90, position.Y, position.Z - 75), new Vector3(position.X + 70, position.Y + 225, position.Z + 70));
        }

       /* public override void aplicarTransformacion(Crafteable crafteable, Sounds sounds, World currentWorld)
        {
            //sounds.playTalarArbol();
            //this.createMadera(crafteable.getPosition(), new Vector3(1f, 1f, 1f));
            Madera newMadera = currentWorld.crearMadera(crafteable.getPosition().X, crafteable.getPosition().Z);
            crafteable.destroy();
        }*/
    }
}
