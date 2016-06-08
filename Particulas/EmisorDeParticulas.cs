using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using System.Collections.Generic;
using System.Collections;

namespace AlumnoEjemplos.Los_Barto.Particulas
{
    /*
     * EJEMPLOS:
     * emisorParticulas1 = new EmisorDeParticulas(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Particulas\\fuego.png", 100, d3dDevice);
     * emisorParticulas1.Posicion = new Vector3(5, 1, 5);
     * emisorParticulas1.GradoDeDispersion = 50;
     * emisorParticulas1.TiempoFrecuenciaDeCreacion = 0.05f;
     * 
     * 
     * emisorParticulas2 = new EmisorDeParticulas(GuiController.Instance.AlumnoEjemplosMediaDir + "\\Particulas\\fuego.png", 500, d3dDevice);
     * emisorParticulas2.Posicion = new Vector3(-25, 10, 25);
     * emisorParticulas2.GradoDeDispersion = 1000;
     * emisorParticulas2.TiempoDeVidaParticula = 2.5f;
     * emisorParticulas2.TiempoFrecuenciaDeCreacion = 0.01f;
     * emisorParticulas2.FactorDeVelocidad = 2.5f;
     * emisorParticulas2.MinTamañoParticula = 0.5f;
     * emisorParticulas2.MaxTamañoParticula = 1.0f;
     * emisorParticulas2.ColorInicialParticula = System.Drawing.Color.FromArgb(0xff, 0x00, 0xff, 0x00);
     * emisorParticulas2.ColorFinalParticula = System.Drawing.Color.FromArgb(0x00, 0xff, 0xff, 0x00);
    */


    public class EmisorDeParticulas
    {
        //Propiedades.
        private Device d3dDevice;

        private int cantParticulas;

        private Particula[] particulas;
        private Stack<Particula> particulasVivas;
        private Queue<Particula> particulasMuertas;
        private Texture texturaParticula;
        private float tiempoAcumulado = 0;

        private System.Random random = new System.Random(0);

        private Vector3 posicion;
        private float tFrecCreacionParticula = 1.0f;
        private float tDeVidaParticula = 5.0f;
        private float minSizeParticula = 0.25f;
        private float maxSizeParticula = 0.5f;

        private Color colorInicial = Color.FromArgb(0xff, 0xff, 0x00, 0x00);
        private Color colorFinal   = Color.FromArgb(0x00, 0x00, 0x00, 0xff);

        private int dispersionParticulas = 100;
        private float factorVelocidad = 1.0f;
        private bool ejecutando;

        /// <summary>
        /// Posicion del emisor de particulas en la escena.
        /// </summary>
        public Vector3 Posicion
        {
            get { return this.posicion; }
            set { this.posicion = value; }
        }

        /// <summary>
        /// Tiempo de frecuencia de creacion de particulas.
        /// </summary>
        public float TiempoFrecuenciaDeCreacion
        {
            get { return this.tFrecCreacionParticula; }
            set { this.tFrecCreacionParticula = value; }
        }

        /// <summary>
        /// Tiempo de vida de las particulas.
        /// </summary>
        public float TiempoDeVidaParticula
        {
            get { return this.tDeVidaParticula; }
            set { this.tDeVidaParticula = value; }
        }

        /// <summary>
        /// Minimo tamaño que puede tener una particula.
        /// </summary>
        public float MinTamañoParticula
        {
            get { return this.minSizeParticula; }
            set { this.minSizeParticula = value; }
        }

        /// <summary>
        /// Maximo tamaño que puede tener una particula.
        /// </summary>
        public float MaxTamañoParticula
        {
            get { return this.maxSizeParticula; }
            set { this.maxSizeParticula = value; }
        }

        /// <summary>
        /// Color inicial que tienen las particulas.
        /// </summary>
        public Color ColorInicialParticula
        {
            set { this.colorInicial = value; }
        }

        /// <summary>
        /// Color final que tienen las particulas.
        /// </summary>
        public Color ColorFinalParticula
        {
            set { this.colorFinal = value; }
        }

        /// <summary>
        /// Valor que representa el grado de dispersion de las particulas.
        /// </summary>
        public int GradoDeDispersion
        {
            get { return this.dispersionParticulas; }
            set { this.dispersionParticulas = value;}
        }

        /// <summary>
        /// Valor que se le multiplica a la velocidad base (Por defecto es 1).
        /// </summary>
        public float FactorDeVelocidad
        {
            get { return this.factorVelocidad; }
            set { this.factorVelocidad = value; }
        }

        //Constructor.
        public EmisorDeParticulas(string bitmapURL, int cantMaxParticulas, Device device)
        {
            this.ejecutando = true;
            this.d3dDevice = device;
            this.cantParticulas = cantMaxParticulas;

            this.particulas = new Particula[cantMaxParticulas];

            this.particulasVivas   = new Stack<Particula>(cantMaxParticulas);
            this.particulasMuertas = new Queue<Particula>(cantMaxParticulas);

            //Creo todas las particulas. Inicialmente estan todas muertas.
            for (int i = 0; i < cantMaxParticulas; i++)
            {
                this.particulas[i] = new Particula();
                this.particulasMuertas.Enqueue(this.particulas[i]);
            }

            //Cargo la textura que tendra cada particula.(Color Key negro). 
            this.texturaParticula = TextureLoader.FromFile(this.d3dDevice,
                                                           bitmapURL,
                                                           128,
                                                           128,
                                                           D3DX.Default,
                                                           0,
                                                           Format.A32B32G32R32F,
                                                           Pool.Managed,
                                                           Filter.Point,
                                                           Filter.Point,
                                                           Color.Black.ToArgb());
        }

        public void cambiarTexturaParticula(string bitmapURL)
        {
            this.texturaParticula = TextureLoader.FromFile(this.d3dDevice, bitmapURL);
        }

        public void detener()
        {
            ejecutando = false;
        }
        /// <summary>
        /// Metodo que se debe ejecutar en cada frame. Emite las particulas.
        /// </summary>
        public void render(float elapsedTime)
        { 
            tiempoAcumulado += elapsedTime;

            if (tiempoAcumulado >= this.tFrecCreacionParticula && ejecutando)
            {
                tiempoAcumulado = 0.0f;

                //Inicializa y agrega una particula a la lista de particulas vivas.
                this.crearParticula();
            }

            //Esto tal vez deberia estar en init.
            //Habilito el dibujado de point sprites.
            this.d3dDevice.RenderState.PointSpriteEnable = true;
            //Habilito la escala por partícula.
            this.d3dDevice.RenderState.PointScaleEnable = true;
            this.d3dDevice.RenderState.PointScaleA = 1.0f;
            this.d3dDevice.RenderState.PointScaleB = 1.0f;
            this.d3dDevice.RenderState.PointScaleC = 0.0f;

            //Fijo la textura actual de la particula.
            this.d3dDevice.SetTexture(0, this.texturaParticula);

            Particula p = this.particulasVivas.Peek();

            // Va recorriendo la lista de particulas vivas,
            // actualizando el tiempo de vida restante, y dibujando.
            d3dDevice.RenderState.AlphaBlendEnable = true;
            //d3dDevice.Material = MaterialFactory.DEFAULT_MATERIAL;

            while (p != null)
            {
                p.TiempoDeVidaRestante -= elapsedTime;

                if (p.TiempoDeVidaRestante <= 0)
                {
                    //Saco la particula de la lista de particulas vivas.
                    this.particulasVivas.Pop();

                    //Inserto la particula en la lista de particulas muertas.
                    this.particulasMuertas.Enqueue(p);
                }
                else
                {
                    //Actualizo y Dibujo la partícula
                    this.actualizarParticula(elapsedTime, p);
                    this.dibujarParticula(p);
                }

                p = this.particulasVivas.Pop();
            }

            d3dDevice.RenderState.AlphaBlendEnable = false;
        }

        private void crearParticula()
        {
            Particula p = null;

            //Saco una particula de la lista de particulas muertas.
            if (this.particulasMuertas.Dequeue() != null)
            {
                //Agrego la partícula a la lista de partículas vivas.
                this.particulasVivas.Push(p);

                //Seteo valores iniciales de la partícula.
                p.TiempoDeVidaTotal = this.tDeVidaParticula;
                p.TiempoDeVidaRestante = this.tDeVidaParticula;
                p.PointSprite[0].Position.X = posicion.X;
                p.PointSprite[0].Position.Y = posicion.Y;
                p.PointSprite[0].Position.Z = posicion.Z;
                p.PointSprite[0].Color = this.colorInicial.ToArgb();

                float faux;

                // Según la dispersion asigno una velocidad inicial. 
                //(Si la dispersion es 0 la velocidad inicial sera (0,1,0)).
                faux = random.Next(this.dispersionParticulas) / 1000.0f;
                faux *= (faux * 1000 % 2 == 0 ? 1.0f : -1.0f);
                p.v3Velocidad.X = faux * 2.0f;

                faux = 1.0f - (2.0f * random.Next(this.dispersionParticulas) / 1000.0f);
                p.v3Velocidad.Y = faux * 2.0f;

                faux = random.Next(this.dispersionParticulas) / 1000.0f;
                faux *= (faux * 1000 % 2 == 0 ? 1.0f : -1.0f);
                p.v3Velocidad.Z = faux * 2.0f;

                //Modifico el tamaño de manera aleatoria.
                float size = (float)random.NextDouble() * this.maxSizeParticula;
                if (size < this.minSizeParticula) size = this.minSizeParticula;
                p.PointSprite[0].PointSize = size;
            }
        }

        private void actualizarParticula(float elapsedTime, Particula p)
        {

            //Actulizo posicion de la particula.
            p.PointSprite[0].Position.X += (p.v3Velocidad.X * elapsedTime) * this.factorVelocidad;
            p.PointSprite[0].Position.Y += (p.v3Velocidad.Y * elapsedTime) * this.factorVelocidad;
            p.PointSprite[0].Position.Z += (p.v3Velocidad.Z * elapsedTime) * this.factorVelocidad;
        }

        private void dibujarParticula(Particula p)
        {
            float parametroInterpolacion;

            // Actulizo color de la particula. Calculo parámetro de interpolación.
            //(para variar el color de la partícula a medida que va llegando al final de su vida)
            parametroInterpolacion = 1.0f - (p.TiempoDeVidaRestante / p.TiempoDeVidaTotal);

            //Realizo la interpolación lineal
            int iAlphaComp = System.Convert.ToInt32(this.colorInicial.A + ((this.colorFinal.A - this.colorInicial.A) * parametroInterpolacion));
            int iRedComp = System.Convert.ToInt32(this.colorInicial.R + ((this.colorFinal.R - this.colorInicial.R) * parametroInterpolacion));
            int iGreenComp = System.Convert.ToInt32(this.colorInicial.G + ((this.colorFinal.G - this.colorInicial.G) * parametroInterpolacion));
            int iBlueComp = System.Convert.ToInt32(this.colorInicial.B + ((this.colorFinal.B - this.colorInicial.B) * parametroInterpolacion));

            p.PointSprite[0].Color = Color.FromArgb(iAlphaComp, iRedComp, iGreenComp, iBlueComp).ToArgb();

            //Moudulacion del canal alpha. 
            
            int color = this.d3dDevice.RenderState.TextureFactor;

            this.d3dDevice.RenderState.TextureFactor = p.PointSprite[0].Color;
            this.d3dDevice.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            this.d3dDevice.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            this.d3dDevice.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

            //Le indico a DirectX el formato de ParticulaVertex.
            this.d3dDevice.VertexFormat = ParticulaVertex.Format;

            this.d3dDevice.DrawUserPrimitives(PrimitiveType.PointList, 1, p.PointSprite);

            this.d3dDevice.RenderState.TextureFactor = color;
        }
    }
}
