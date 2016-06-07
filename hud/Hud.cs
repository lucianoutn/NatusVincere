using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.TgcSceneLoader;
using System.Drawing;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.NatusVincere
{
    public class Hud 
    {
        TgcSprite hudSprite;  
        TgcText2d vida; 
        TgcText2d agua; 
        TgcText2d suenio;
        Size screenSize = GuiController.Instance.Panel3d.Size;
       //Human personaje;


        public Hud()
        {
            hudSprite = new TgcSprite();
            hudSprite.Texture = TgcTexture.createTexture("AlumnoEjemplos\\NatusVincere\\Hud\\hud.png");
            vida = new TgcText2d();
            agua = new TgcText2d();
            suenio = new TgcText2d();

            Size tamañoTextoHud = new Size(40, 60);
            vida.Size = tamañoTextoHud;
            vida.changeFont(new System.Drawing.Font("ComicSands", 16, FontStyle.Bold));

            vida.Color = Color.Crimson;
            agua.Size = tamañoTextoHud;
            agua.changeFont(new System.Drawing.Font("ComicSands", 16, FontStyle.Bold));
            agua.Color = Color.Cyan;
            suenio.Size = tamañoTextoHud;
            suenio.changeFont(new System.Drawing.Font("ComicSands", 16, FontStyle.Bold));
            suenio.Color = Color.DarkGray;


            //ubico el hud abajo a la izq
            Size textureSizeHud = hudSprite.Texture.Size;
            hudSprite.Position = new Vector2(FastMath.Max(screenSize.Width / 6 - textureSizeHud.Width / 2, 0), (FastMath.Max(screenSize.Height - textureSizeHud.Height * (1.5f), 0)));
            vida.Position = new Point(screenSize.Width * 1 / 12 - (int)vida.Size.Width / 2, (int)hudSprite.Position.Y + (int)vida.Size.Height); //lo posiciono debajo del corazon
            agua.Position = new Point(screenSize.Width * 2 / 12 - (int)agua.Size.Width / 2, (int)hudSprite.Position.Y + (int)agua.Size.Height); //lo posiciono debajo del vaso
            suenio.Position = new Point(screenSize.Width * 3 / 12 - (int)suenio.Size.Width / 2, (int)hudSprite.Position.Y + (int)suenio.Size.Height); //lo posiciono debajo del sofa
        }     
  
        public void renderizate(Human personaje)
        {
            if (personaje.muerto) this.dispose();
            else
            {
                vida.Text = personaje.health.ToString();  //hud tiene q ir en render
                agua.Text = personaje.agua.ToString();  //hud tiene q ir en render
                suenio.Text = personaje.suenio.ToString();  //hud tiene q ir en render

                GuiController.Instance.Drawer2D.beginDrawSprite();
                hudSprite.render();
                vida.render();
                agua.render();
                suenio.render();
                GuiController.Instance.Drawer2D.endDrawSprite();
            }
        }
        
       
        public void dispose()
        {
            hudSprite.dispose();
            vida.dispose();
            agua.dispose();
            suenio.dispose();
        }
    }
}
