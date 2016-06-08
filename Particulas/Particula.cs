using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.Los_Barto.Particulas
{
    public struct ParticulaVertex
	{
        public Vector3 Position;
		public float PointSize;
		public int Color;
		public static VertexFormats Format
		{
			get { return VertexFormats.Position | VertexFormats.Diffuse | VertexFormats.PointSize; }
		}
	}

    public class Particula
    {
        //Propiedades.
        private ParticulaVertex[] pointSprite = new ParticulaVertex[1];
        private float tiempoDeVidaTotal;
        private float tiempoDeVidaRestante;
        public Vector3 v3Velocidad;

        public ParticulaVertex[] PointSprite
        {
            get { return this.pointSprite; }
        }

        public float TiempoDeVidaTotal
        {
            get { return this.tiempoDeVidaTotal;}
            set { this.tiempoDeVidaTotal = value; }
        }

        public float TiempoDeVidaRestante
        {
            get { return this.tiempoDeVidaRestante; }
            set { this.tiempoDeVidaRestante = value; }
        }

        public Particula()
		{
            pointSprite[0].Position.X = 0.0f;
            pointSprite[0].Position.Y = 0.0f;
            pointSprite[0].Position.Z = 0.0f;
            pointSprite[0].PointSize = 1.0f;
            pointSprite[0].Color = System.Drawing.Color.FromArgb(0xff, 0xff, 0xff, 0xff).ToArgb();
		}
    }
}
