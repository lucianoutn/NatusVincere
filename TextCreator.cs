using System;
using System.Linq;
using System.Text;
using TgcViewer.Utils._2D;
using System.Drawing;

namespace AlumnoEjemplos.NatusVincere
{
    class TextCreator
    {
        string font;
        int fontSize;
        Size boxSize;

        public TextCreator(string font, int fontSize, Size boxSize)
        {
            this.font = font;
            this.fontSize = fontSize;
            this.boxSize = boxSize;
        }

        public TgcText2d createText(string text)
        {
            TgcText2d plainText = new TgcText2d();
            plainText.Text = text;
            plainText.Color = Color.White;
            plainText.Align = TgcText2d.TextAlign.LEFT;
            plainText.Size = this.boxSize;
            plainText.changeFont(new System.Drawing.Font(this.font, this.fontSize));

            return plainText;
        }

    }
}
