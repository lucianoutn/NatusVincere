using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using System.IO;
using System.Collections.Generic;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.NatusVincere
{
    public class ObjectsFactory
    {
        private TgcMesh arbolMesh;
        private TgcMesh pinoMesh;
        private TgcMesh arbustoMesh;
        private TgcMesh piedraMesh;
        private TgcMesh hachaMesh;
        private TgcMesh maderaMesh;
        private TgcMesh fogataMesh;
        private TgcMesh leonMesh;
        private TgcMesh frutaMesh;
        private TgcMesh arbustoFrutaMesh;

        private List<Crafteable> objectList;
        int objectId = 0;


        TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
        string skeletalPath = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\";
        string[] animationsPath;
        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
        DirectoryInfo dirAnim;
        FileInfo[] animFiles;
        string[] animationList;

        public ObjectsFactory(List<Crafteable> objectList)
        {
            this.objectList = objectList;

            TgcSceneLoader loader = new TgcSceneLoader();
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcScene arbolScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\ArbolSelvatico\ArbolSelvatico-TgcScene.xml");
            this.arbolMesh = arbolScene.Meshes[0];

            TgcScene pinoScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Pino\Roca-TgcScene.xml");
            this.pinoMesh = pinoScene.Meshes[0];

            TgcScene arbustoScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Planta\Planta3-TgcScene.xml");
            this.arbustoMesh = arbustoScene.Meshes[0];

            TgcScene piedraScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Roca\Roca-TgcScene.xml");
            this.piedraMesh = piedraScene.Meshes[0];

            TgcScene hachaScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Hacha\Hacha-TgcScene.xml");
            this.hachaMesh = hachaScene.Meshes[0];

            TgcScene maderaScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Madera\Madera-TgcScene.xml");
            this.maderaMesh = maderaScene.Meshes[0];

            TgcScene fogataScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Fogata\wood+fire-TgcScene.xml");
            this.fogataMesh = fogataScene.Meshes[0];

            TgcScene leonScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Leon\Cheetah+(adult)+TEXTURE-TgcScene.xml");
            this.leonMesh = leonScene.Meshes[0];

            TgcScene frutaScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\Fruta\Fruta-TgcScene.xml");
            this.frutaMesh = frutaScene.Meshes[0];

            TgcScene arbustoFrutaScene = loader.loadSceneFromFile(System.Environment.CurrentDirectory + @"\AlumnoEjemplos\NatusVincere\InventarioYObjetos\ArbustoFruta\groseille+fruit-TgcScene.xml");
            this.arbustoFrutaMesh = arbustoFrutaScene.Meshes[0];

            dirAnim = new DirectoryInfo(skeletalPath + "Animations\\");
            animFiles = dirAnim.GetFiles("*-TgcSkeletalAnim.xml", SearchOption.TopDirectoryOnly);
            animationList = new string[animFiles.Length];
            animationsPath = new string[animFiles.Length];

            for (int i = 0; i < animFiles.Length; i++)
            {
                string name = animFiles[i].Name.Replace("-TgcSkeletalAnim.xml", "");
                animationList[i] = name;
                animationsPath[i] = animFiles[i].FullName;
            }
        }

        public Human createHuman(Vector3 position, Vector3 scale)
        {
            Inventory inventory = new Inventory(this, new Vector2(20, 20));
            TgcSkeletalMesh humanMesh;
            humanMesh = skeletalLoader.loadMeshAndAnimationsFromFile(skeletalPath + "WomanJeans-TgcSkeletalMesh.xml", skeletalPath, animationsPath);
            humanMesh.buildSkletonMesh();
            return new Human(inventory, humanMesh, position, scale);
        }

        public Arbol createArbol(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.arbolMesh.createMeshInstance("arbol_" + objectId);
            Arbol arbol = new Arbol(meshInstance, position, scale);
            this.objectList.Add(arbol);
            return arbol;
        }

        public Arbusto createArbusto(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.arbustoMesh.createMeshInstance("arbusto_" + objectId);
            Arbusto arbusto = new Arbusto(meshInstance, position, scale);
            this.objectList.Add(arbusto);
            return arbusto;
        }


        public Pino createPino(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.pinoMesh.createMeshInstance("pino_" + objectId);
            Pino pino = new Pino(meshInstance, position, scale);
            this.objectList.Add(pino);
            return pino;
        }

        public Piedra createPiedra(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.piedraMesh.createMeshInstance("piedra_" + objectId);
            Piedra piedra = new Piedra(meshInstance, position, scale);
            this.objectList.Add(piedra);
            return piedra;
        }

        public Hacha createHacha(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.hachaMesh.createMeshInstance("hacha_" + objectId);
            Hacha hacha = new Hacha(meshInstance, position, scale);
            this.objectList.Add(hacha);
            return hacha;
        }

        public Madera createMadera(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.maderaMesh.createMeshInstance("madera_" + objectId);
            Madera madera = new Madera(meshInstance, position, scale);
            this.objectList.Add(madera);
            return madera;
        }

        public Fogata createFogata(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.fogataMesh.createMeshInstance("fogata_" + objectId);
            Fogata fogata = new Fogata(meshInstance, position, scale);
            this.objectList.Add(fogata);
            return fogata;
        }

        public ArbustoFruta createArbustoFruta(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.arbustoFrutaMesh.createMeshInstance("arbustoFruta_" + objectId);
            ArbustoFruta arbustoFruta = new ArbustoFruta(meshInstance, position, scale);
            this.objectList.Add(arbustoFruta);
            return arbustoFruta;
        }

        public Fruta createFruta(Vector3 position, Vector3 scale)
        {
            objectId++;
            position.Y += 20;
            TgcMesh meshInstance = this.frutaMesh.createMeshInstance("fruta_" + objectId);
            Fruta fruta = new Fruta(meshInstance, position, scale);
            this.objectList.Add(fruta);
            return fruta;
        }

        public Leon createLeon(Vector3 position, Vector3 scale)
        {
            objectId++;
            TgcMesh meshInstance = this.leonMesh.createMeshInstance("leon_" + objectId);
            Leon leon = new Leon(meshInstance, position, scale);
            
            return leon;
        }

        public void transform(Crafteable crafteable, Sounds sounds)
        {
            if (crafteable.getType() == 1 && crafteable.getStatus() == 1)
            {
                sounds.playTalarArbol();
                this.createMadera(crafteable.getPosition(), new Vector3(1f, 1f, 1f));
                crafteable.destroy();
            }
            if (crafteable.getType() == 3 && crafteable.getStatus() == 1)
            {
                sounds.playTalarArbol();
                this.createFruta(crafteable.getPosition(), new Vector3(20f, 20f, 20f));
                crafteable.destroy();
            }

        }

        public void dispose()
        {
            arbolMesh.dispose();
            piedraMesh.dispose();
            hachaMesh.dispose();
            pinoMesh.dispose();
        }
    }
}
