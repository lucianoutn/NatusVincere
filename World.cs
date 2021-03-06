﻿using System.Collections.Generic;
using Microsoft.DirectX;
using TgcViewer.Utils.Terrain;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Input;
using TgcViewer.Utils._2D;
using System;

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
            this.terrainTexture = GuiController.Instance.ExamplesMediaDir + "Texturas\\" + "Pasto.jpg"; ;

            this.terrainHeightmap = GuiController.Instance.AlumnoEjemplosDir + "NatusVincere\\" + "heightmap3.jpg";
            this.objects = new List<Crafteable>();
            this.objectsFactory = new ObjectsFactory(this.objects);
            this.currentScaleXZ = (79.5f / 5000) * size;
            this.currentScaleY = 2f;
            this.position = worldPosition;
            this.terrain = new TgcSimpleTerrain();
            this.refreshTerrain();
            this.terrain.loadTexture(terrainTexture);
            this.agregarObjetos();

        }

        public void refreshTerrain()
        {
            this.terrainPosition.X = this.position.X / this.currentScaleXZ;
            this.terrainPosition.Z = this.position.Z / this.currentScaleXZ;
            this.terrain.loadHeightmap(this.terrainHeightmap, this.currentScaleXZ, this.currentScaleY, this.terrainPosition);
        }

        public void render()
        {
            if (!this.rendered)
            {
                this.terrain.render();
                int i;
                for(i=0; i < objects.Count; i++)
                {
                    TgcCollisionUtils.FrustumResult frustumResult = TgcCollisionUtils.classifyFrustumAABB(GuiController.Instance.Frustum, objects[i].getBB()); //por cada modelo. haceer if antes de render()
                    if (frustumResult.Equals(TgcCollisionUtils.FrustumResult.INTERSECT)
                        || frustumResult.Equals(TgcCollisionUtils.FrustumResult.INSIDE))
                    {
                        objects[i].render();
                    };
                }
                
                this.rendered = true;
            }
        }

        public void transform(Human human, Sounds sounds)
        {
            this.objects.ForEach(crafteable => { if (crafteable.isNear(human)) this.objectsFactory.transform(crafteable, sounds); });
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
            this.objects.ForEach(crafteable => { crafteable.move(distance); });
        }

        public void setPosition(Vector3 position)
        {
            
            this.position = position;
            this.refreshTerrain();
        }

        public void dispose()
        {
            this.refresh();
            this.objects.ForEach(crafteable => { crafteable.dispose(); });
            this.terrain.dispose();
        }

        Random rnd = new Random();

        public void agregarObjetos()
        {
            int x, z;

            for (int i = 0; i <= 5; i++)
            {
                for (int j = 0; j <8; j++)
                {
                    x = rnd.Next(-size/2, size/2);
                    z = rnd.Next(-size / 2, size / 2);
                    crearArbusto(x, z);
                    x = rnd.Next(-size / 2, size / 2);
                    z = rnd.Next(-size / 2, size / 2);
                    crearArbol(x, z);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    x = rnd.Next(-size / 2, size / 2);
                    z = rnd.Next(-size / 2, size / 2);
                    crearPiedra(x, z);
                }
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    x = rnd.Next(-size / 2, size / 2);
                    z = rnd.Next(-size / 2, size / 2);
                    crearArbustoFruta(x, z);
                }
            }
        }

        public bool isInWorld(float x, float z)
        {
            float maxDistance = this.size / 2;
            return (FastMath.Abs(x) < maxDistance && FastMath.Abs(z) < maxDistance);
        }
        public Arbol crearArbol(float x, float z)
        {
            x += position.X;
            z += position.Z;

            return objectsFactory.createArbol(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
        public Piedra crearPiedra(float x, float z)
        {
            x += position.X;
            z += position.Z;

            return objectsFactory.createPiedra(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
        public Pino crearPino(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createPino(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
        public Arbusto crearArbusto(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createArbusto(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public Leon crearLeon(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createLeon(new Vector3(x, calcularAltura(x, z), z), new Vector3(20.75f,21.75f, 20.75f));
        }

        public Fruta crearFruta(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createFruta(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.15f, 0.25f, 0.15f));
        }
        public ArbustoFruta crearArbustoFruta(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createArbustoFruta(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public Hacha crearHacha(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createHacha(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }

        public Madera crearMadera(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createMadera(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 1.75f, 0.75f));
        }
        public Fogata crearFogata(float x, float z)
        {
            x += position.X;
            z += position.Z;
            return objectsFactory.createFogata(new Vector3(x, calcularAltura(x, z), z), new Vector3(0.75f, 0.55f, 0.75f));
        }
    }
}
