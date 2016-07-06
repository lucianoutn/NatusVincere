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
using System;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.NatusVincere
{


    public class Inventory
    {
        public Crafteable[] items;
        TgcText2d[] texts;
        bool[] selections;
        TgcText2d title;
        TextCreator textCreator = new TextCreator("Arial", 16, new Size(300, 16));
        Point position;
        ObjectsFactory objectsFactory;
        Human dueño;
        public bool hachaEquipada = false;

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
            
            if (item.isStorable() && freeIndex != -1 && item.status != 3)
            {
                this.items[freeIndex] = item;
                this.texts[freeIndex] = textCreator.createText((freeIndex+1) + " - " + item.description);
                this.selections[freeIndex] = false;
                item.addToInventory();
                ////para que la clase HACHA renderice el hacha en mano
                if (item.getType() == 9)
                {
                    this.hachaEquipada = true;

                }
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

        public void setDueño(Human human)
        {
            dueño = human;
        }

        public void setPosition(Vector2 position)
        {
            int x = (int)FastMath.Floor(position.X);
            int y = (int)FastMath.Floor(position.Y);
            this.position = new Point(x, y);
        }

        public void update() {
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
                else
                {
                    int item = checkConsumible();

                    if (item!=-1)
                    {
                        doConsumir(item);
                        dropObject(item);
                    }
                }
            }
        }
        
        private void selectItem(int index)
        {
            int[] selectedIndexes = new int[2];
            selectedIndexes = this.getSelectedIndexes();

            if (this.items[index] != null && selectedIndexes[1] == -1) { 
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
        private int checkConsumible()
        {
            int i = 0;
            int j = 0;

            int[] selected = new int[2];
            selected = getSelectedIndexes();
            
            if (selected[0] != -1)
            {
                Crafteable firstItem = items[selected[0]];

                if (firstItem.getConsumible())
                {
                    return selected[0];
                }
            }
            if (selected[1] != -1)
            {
                Crafteable secondItem = items[selected[1]];

                if (secondItem.getConsumible())
                {
                    return selected[1];
                }
            }

            return -1;
        }

        private bool checkCombinationPosible()
        {
            int i = 0;
            int j = 0;

            int[] selected = new int[2];
            selected = getSelectedIndexes();
            /*for (i = 0; i < this.selections.Length; i++)
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
            }*/

            if (selected[0] != -1 && selected[1] != -1)
            {
                Crafteable firstItem = items[selected[0]];
                Crafteable secondItem = items[selected[1]];

                return firstItem.getType() == 2 && secondItem.getType() == 5 ||
                    firstItem.getType() == 5 && secondItem.getType() == 2 ||
                    firstItem.getType() == 5 && secondItem.getType() == 5;
            }

            return false;
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
            
            //TODO: REFACTOR URGENTE: Crear un COMBINADOR de objetos perteneciente al humano.

            if ( (firstItem.getType() == 5 && secondItem.getType() == 2) || (firstItem.getType() == 2 && secondItem.getType() == 5) )
            {
                newObject = this.objectsFactory.createHacha(new Vector3(0, 0, 200), new Vector3(5, 5,5));
                this.hachaEquipada = true;
            }
            else
            {
                if (firstItem.getType() == 5 && secondItem.getType() == 5)
                {
                    newObject = this.objectsFactory.createFogata(new Vector3(0, 0, 200), new Vector3(0.75f, 0.55f, 0.75f));
                }
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
            int[] indexes = new int[2];
            indexes = this.getSelectedIndexes();
            int firstIndex = indexes[0];
            int secondIndex = indexes[1];

            if (firstIndex >= 0 && secondIndex >= 0) {
                this.combine(firstIndex, secondIndex);
            }

        }

        private void doConsumir(int item)
        {
            items[item].consumir(dueño);
        }

        public void dropObject(int index)
        {
            this.items[index] = null;
            this.texts[index] = null;
            this.selections[index] = false;
         }

        public Crafteable leaveObject(Vector3 position)
        {
            Crafteable leftObject = null;
            int[] indexes = new int[2]; 
            indexes = this.getSelectedIndexes();
            int firstIndex = indexes[0];
            int secondIndex = indexes[1];
            
            if (secondIndex != -1) unselect(secondIndex); //Prevengo que deje los dos objetos

            if (firstIndex >= 0) {
                leftObject = this.items[firstIndex];
                ////para que la clase HACHA deje de renderizar el hacha en mano
                if (leftObject.getType() == 9)
                {
                    this.hachaEquipada = false;

                }
                dropObject(firstIndex);
                leftObject.drop(position);
            }
            
            
            return leftObject;
        }

        public int[] getSelectedIndexes()
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

            int[] indexes = new int[2];

            indexes[0] = firstIndex;
            indexes[1] = secondIndex;
            return indexes;
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
