﻿using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils._2D;
using TgcViewer;
using System.Drawing;

namespace AlumnoEjemplos.NatusVincere
{
    public class Piedra : Crafteable
    {
        public new int uses = 3;
        public new int type = 2;
        private TgcBoundingBox tronco;
        public TgcSprite invImg;

        public Piedra(TgcMesh mesh, Vector3 position, Vector3 scale) : base(mesh, position, scale)
        {
            this.type = 2;
            this.description = "Piedra";
            this.invImg = new TgcSprite();
            invImg.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\InventarioYObjetos\\Roca\\inv.png");
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


        public override void renderImg(Point pos)
        {
            GuiController.Instance.Drawer2D.beginDrawSprite();
            this.invImg.Position = new Vector2(pos.X, pos.Y);
            this.invImg.render();
            GuiController.Instance.Drawer2D.endDrawSprite();
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
            return this.tronco;
        }

        public override void Render()
        {
            tronco.render();
        }

        public override void borrarBB()
        {
            this.tronco.dispose();
            this.tronco = new TgcBoundingBox(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f));
        }

        public override void setBB(Vector3 position)
        {
            this.tronco = new TgcBoundingBox(new Vector3(position.X - 10, position.Y, position.Z - 10), new Vector3(position.X + 10, position.Y + 80, position.Z + 10));
        }
    }
}
