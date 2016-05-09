using System.Collections;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using TgcViewer.Utils._2D;
using System.Drawing;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.NatusVincere
{


    class Inventory
    {
        Crafteable[] items;
        TgcText2d[] texts;
        bool[] selections;
        TgcText2d title;
        TextCreator textCreator = new TextCreator("Arial", 16, new Size(300, 16));
        Point position;
        ObjectsFactory objectsFactory;

        public Inventory(ObjectsFactory objectsFactory, Vector2 position)
        {
            int inventorySize = 8; //Por el momento, no hay mas teclas de numeros..
            this.objectsFactory = objectsFactory;
            this.setPosition(position);
            this.items = new Crafteable[inventorySize];
            this.texts = new TgcText2d[inventorySize];
            this.selections = new bool[inventorySize];
            this.title = textCreator.createText("Inventario");
            for (int i = 0; i < selections.Length; i++) this.selections[i] = false;
        }

        public void addItem(Crafteable item)
        {
            int freeIndex = this.findFreeIndex();
            
            if (freeIndex != -1 && item.status != 3)
            {
                this.items[freeIndex] = item;
                this.texts[freeIndex] = textCreator.createText((freeIndex+1) + " - " + item.description + item.getType());
                this.selections[freeIndex] = false;
            }
        }


        public void render()
        {
            int i = 0;
            Point position = this.position;
            this.title.render();
            for (i = 0; i < this.items.Length; i++) {
                if (this.items[i] != null)
                {
                    position.Y += 40;
                    this.texts[i].Position = position;
                    this.texts[i].render();
                }
            }
        }

        public void setPosition(Vector2 position)
        {
            int x = (int)FastMath.Floor(position.X);
            int y = (int)FastMath.Floor(position.Y);
            this.position = new Point(x, y);
        }

        public void update(float elapsedTime) {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcD3dInput input = GuiController.Instance.D3dInput;

            if (input.keyDown(Key.D0))
            {
                this.unselectAll();
            }

            if (input.keyDown(Key.D1))
            {
                this.selectItem(0);
            }
            if (input.keyDown(Key.D2))
            {
                this.selectItem(1);
            }
            if (input.keyDown(Key.D3))
            {
                this.selectItem(2);
            }
            if (input.keyDown(Key.D4))
            {
                this.selectItem(3);
            }
            if (input.keyDown(Key.D5))
            {
                this.selectItem(4);
            }
            if (input.keyDown(Key.D6))
            {
                this.selectItem(5);
            }
            if (input.keyDown(Key.D7))
            {
                this.selectItem(6);
            }
            if (input.keyDown(Key.D8))
            {
                this.selectItem(7);
            }
            if (input.keyDown(Key.D9))
            {
                this.selectItem(8);
            }
            if (input.keyDown(Key.C))
            {
                if (checkCombinationPosible()) { 
                    this.doCombine();
                }
            }
        }
        
        private void selectItem(int index)
        {
            Vector2 selectedIndexes = getSelectedIndexes();

            if (this.items[index] != null && selectedIndexes.Y == -1) { 
                this.selections[index] = true;
                this.texts[index].Color = Color.Blue;
                this.togglePossibleCombinationText();
            }
        }

        private void unselectAll()
        {
            int i = 0;
            for (i = 0; i < this.items.Length && this.items[i] != null; i++) {
                this.unselect(i);
            }
        }

        private void unselect(int index)
        {
            if (this.items[index] != null) { 
                this.selections[index] = false;
                this.texts[index].Color = Color.White;
                this.togglePossibleCombinationText();
            }
        }

        private bool checkCombinationPosible()
        {
            int i = 0;
            int j = 0;

            Crafteable itemSelected;
            bool combinationPosible = false;

            for (i = 0; i < this.selections.Length; i++)
            {
                if (this.selections[i])
                {
                    itemSelected = this.items[i];
                    for (j = 0; j < this.selections.Length; j++)
                    {
                        if (this.selections[j])
                        {
                            combinationPosible |= itemSelected.checkIfCombine(this.items[j]);
                        }
                    }
                }
            }

            return combinationPosible;
        }

        private void togglePossibleCombinationText()
        {
            if (this.checkCombinationPosible())
            {
                this.title.Text = "Inventario: Aprete C para combinar los objetos";
                return;
            }

            this.title.Text = "Inventario";

        }

        private void combine(int firstIndex, int secondIndex)
        {
            Crafteable firstItem = this.items[firstIndex];
            Crafteable secondItem = this.items[secondIndex];
            Crafteable newObject = null;
            //TODO: REFACTOR.

            if (firstItem.getType() == 1 && secondItem.getType() == 2)
            {
                newObject = this.objectsFactory.createHacha(new Vector3(0, 0, 200), new Vector3(1, 1, 1));
            }

            if (newObject != null)
            {
                this.dropObject(firstIndex);
                this.dropObject(secondIndex);
                firstItem.dispose();
                secondItem.dispose();
                this.addItem(newObject);
                newObject.addToInventory(); 
            }
            
        }

        private void doCombine()
        {
            Vector2 indexes = this.getSelectedIndexes();
            this.combine((int)indexes.X, (int)indexes.Y);

        }

        public void dropObject(int index)
        {
            this.items[index] = null;
            this.texts[index] = null;
            this.selections[index] = false;
         }

        public void leaveObject(Vector3 position)
        {
            Vector2 indexes = this.getSelectedIndexes();
            int firstIndex = (int)indexes.X;
            int secondIndex = (int)indexes.Y;
            
            if (secondIndex != -1) unselect(secondIndex); //Prevengo que deje los dos objetos

            if (firstIndex >= 0) { 
                Crafteable firstItem = this.items[firstIndex];
                dropObject(firstIndex);
                firstItem.drop(position);
            }
        }

        public Vector2 getSelectedIndexes()
        {
            int firstIndex = -1;
            int secondIndex = -1;
            for (int i = 0; i < selections.Length; i++)
            {
                if (selections[i] && firstIndex == -1)
                {
                    firstIndex = i;
                }
                else if (selections[i] && secondIndex == -1)
                {
                    secondIndex = i;
                }
            }
            return new Vector2(firstIndex, secondIndex);
        }

        public int findFreeIndex()
        {
            int freeIndex = -1;
            for (int i = 0; i < this.items.Length && freeIndex == -1; i++)
            {
                if (this.items[i] == null)
                {
                    freeIndex = i;
                }
            }
            return freeIndex;
        }

    }
}
