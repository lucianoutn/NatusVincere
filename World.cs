using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using TgcViewer.Utils._2D;
namespace AlumnoEjemplos.NatusVincere
{

    public class World
    {
        private ObjectsFactory objectsFactory;
        public List<Crafteable> objects;
        private TgcSimpleTerrain terrain;
        float currentScaleXZ;
        float  currentScaleY;
        public Vector3 position;
        int size;
        public Vector3 terrainPosition;
        string terrainTexture;
        public string terrainHeightmap;
        public bool rendered = false;
        public World(Vector3 worldPosition, int size)
        {

            this.size = size;
            this.terrainTexture = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "TerrainTexture2.jpg"; ;
            if (worldPosition.X > 3500)
            {
                this.terrainTexture = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "TerrainTexture4.jpg";
            }
            if (worldPosition.X < 3500)
            this.terrainTexture = GuiController.Instance.ExamplesMediaDir + "Heighmaps\\" + "TerrainTexture2.jpg";

            this.terrainHeightmap = GuiController.Instance.AlumnoEjemplosDir + "NatusVincere\\" + "heightmap.jpg";
            this.objects = new List<Crafteable>();
            this.objectsFactory = new ObjectsFactory(this.objects);
            this.currentScaleXZ = (79.4f / 5000) * size;
            this.currentScaleY = 2f;
            this.position = worldPosition;
            this.terrain = new TgcSimpleTerrain();
            this.refreshTerrain();
            this.agregarObjetos();
            this.terrain.loadTexture(terrainTexture);

        }

        public void refreshTerrain()
        {
            this.terrainPosition.X = this.position.X / this.currentScaleXZ;
            this.terrainPosition.Z = this.position.Z / this.currentScaleXZ;
            this.terrain.loadHeightmap(this.terrainHeightmap, this.currentScaleXZ, this.currentScaleY, this.terrainPosition);
        }

        public void render() {
            if (!this.rendered) { 
                this.terrain.render();
                this.objects.ForEach(crafteable => crafteable.render());
                this.rendered = true;
            }
        }

        public void transform(Human human)
        {
            this.objects.ForEach(crafteable => { if (crafteable.isNear(human)) this.objectsFactory.transform(crafteable); });
        }

        public void addObject(Crafteable crafteable)
        {
            objects.Add(crafteable);
        }

        public void refresh()
        {
            this.objects.RemoveAll(crafteable => crafteable == null || crafteable.getStatus() == 5 || crafteable.getStatus() == 3);
        }

        public float calcularAltura(float x, float z)
        {
            float largo = this.currentScaleXZ * 64;
            float pos_i = 64f * (0.5f + (x - this.position.X) / largo);
            float pos_j = 64f * (0.5f + (z - this.position.Z) / largo);

            int pi = (int)pos_i;
            float fracc_i = pos_i - pi;
            int pj = (int)pos_j;
            float fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else
                if (pi > 63)
                pi = 63;

            if (pj < 0)
                pj = 0;
            else
                if (pj > 63)
                pj = 63;

            int pi1 = pi + 1;
            int pj1 = pj + 1;
            if (pi1 > 63)
                pi1 = 63;
            if (pj1 > 63)
                pj1 = 63;

            // 2x2 percent closest filtering usual: 
            float H0 = this.terrain.HeightmapData[pi, pj] * currentScaleY;
            float H1 = this.terrain.HeightmapData[pi1, pj] * currentScaleY;
            float H2 = this.terrain.HeightmapData[pi, pj1] * currentScaleY;
            float H3 = this.terrain.HeightmapData[pi1, pj1] * currentScaleY;
            float H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                      (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            return H;
        }

        public void move(Vector3 distance)
        {
            this.setPosition(this.position + distance);
        }

        public void setPosition(Vector3 position)
        {
            this.objects.ForEach(crafteable => { crafteable.move(position); });
            this.position = position;
            this.refreshTerrain();
        }

        public void dispose()
        {
            this.refresh();
            this.objects.ForEach(crafteable => { crafteable.dispose(); });
            this.terrain.dispose();
        }

        public void agregarObjetos()
        {
            int qArboles = (size / 200);
            int j = 1;
            int k = 1;
            for (int i = 0; i <= qArboles; i++) {
                k = 1;
                if (i % 3 == 0)
                {
                    k = 15;
                }
                if (i % 2 == 0) {  
                    crearArbol(j * k * (i * qArboles), qArboles - 177*i);
                    
                } else
                {
                    crearPino(j * k * ((i + 30) * qArboles), qArboles - 89 * i);
                    crearArbusto(j * k * ((i - 40) * qArboles), qArboles - 45* i);
                }
                j *= -1;
            }

        }
        public void crearArbol(float x, float z)
        {
            if (x < size / 2 && z < size / 2)
            objectsFactory.createArbol(this.position + new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
        public void crearPino(float x, float z)
        {
            if (x < size / 2 && z < size / 2)
                objectsFactory.createPino(this.position + new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public void crearArbusto(float x, float z)
        {
            if (x < size / 2 && z < size / 2)
                objectsFactory.createArbusto(this.position + new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public void crearLeon(float x, float z)
        {
            if (x < size / 2 && z < size / 2)
                objectsFactory.createLeon(this.position + new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public void crearHacha(float x, float z)
        {
            if (x < size / 2 && z < size / 2)
                objectsFactory.createHacha(this.position + new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
    }
}
