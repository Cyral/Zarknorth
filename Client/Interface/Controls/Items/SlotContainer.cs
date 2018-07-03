using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient
{
    public delegate bool CheckItemEventHandler(object o, Slot s);
    public class SlotContainer : Control
    {
        public SlotControl[,] Slots;
        public int SlotsWidth;
        public int SlotsHeight;
        private Button SelectedInventory;
        public int Selected;
        public Slot[] ItemSlots;

        public bool CanTake = true, CanAdd = true;

        public event TomShane.Neoforce.Controls.EventHandler MoveItems;
        private void OnMoveItem(object o, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (MoveItems != null) MoveItems(o, e);
        }
        public event TomShane.Neoforce.Controls.EventHandler SelectItem;
        private void OnSelectItem(object o, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (SelectItem != null) SelectItem(o, e);
        }
        public event TomShane.Neoforce.Controls.EventHandler DeSelectItem;
        private void OnDeSelectItem(object o, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (DeSelectItem != null) DeSelectItem(o, e);
        }
        public event ShiftClickItemsEventHandler ShiftClickItems;
        private void OnShiftClickItem(Slot s)
        {
            if (ShiftClickItems != null) { ShiftClickItems(s); };
        }
        public delegate void ShiftClickItemsEventHandler(Slot s);
        public event CheckItemEventHandler CheckItems;
        private bool OnCheckAdd(object o, Slot s)
        {
            if (CheckItems != null) return CheckItems(o, s);
            else return true;
        }
        public bool Mini { get; private set; }
        public SlotContainer(Manager manager,int slotsWidth, int slotsHeight, bool mini = false) : base(manager)
        {
            Mini = mini;
            MaximumHeight = MaximumWidth = 10000;
            Passive = true;
            SlotsWidth = slotsWidth;
            SlotsHeight = slotsHeight;
            Slots = new SlotControl[SlotsWidth, SlotsHeight];
            for (int x = 0; x < slotsWidth; x++)
            {
                for (int y = 0; y < slotsHeight; y++)
                {
                    SlotControl s = new SlotControl(manager, mini);
                    s.Init();
                    s.Height = s.Width = Mini ? 32 : 48;
                    s.Left = (s.Width + (Mini ? 4 : 8)) * x;
                    s.Top = (s.Width + (Mini ? 4 : 8)) * y;
                    s.button.MouseDown += button_MouseDown;
                    s.button.MouseUp += button_MouseUp;
                    
                    s.ID = y * SlotsWidth+ x;
                    Slots[x, y] = s;
                    Add(Slots[x, y]);
                }
            }
            ClientWidth = (Slots[0, 0].Width + (Mini ? 4 : 8)) * SlotsWidth;
            ClientHeight = (Slots[0, 0].Height + (Mini ? 4 : 8)) * SlotsHeight;
        }

        void button_MouseUp(object sender, MouseEventArgs e)
        {
            if (Interface.MainWindow.mouseSlot.Item == Item.Blank)
                Interface.MainWindow.isDragging = false;
            if (e.Button == MouseButton.Left && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                OnShiftClickItem(ItemSlots[((SlotControl)(((Button)sender)).Parent).ID]);
            }
            else if ((Interface.MainWindow.expanded && !Interface.MainWindow.hiding) || (ZarknorthClient.Interface.MainWindow.CraftingWindow != null && ZarknorthClient.Interface.MainWindow.CraftingWindow.Visible))
            {
                SlotControl sc = ((SlotControl)(((Button)sender)).Parent);
                if (!Interface.MainWindow.isDragging && CanTake)
                {

                    if (e.Button == MouseButton.Left)
                    {
                        Interface.MainWindow.mouseSlot = (Slot)ItemSlots[sc.ID].Clone();
                        ItemSlots[sc.ID] = new Slot(Item.Blank);
                        Interface.MainWindow.isDragging = true;
                    }
                    else if (e.Button == MouseButton.Right)
                    {
                        Interface.MainWindow.mouseSlot = new Slot(ItemSlots[sc.ID].Item, ItemSlots[sc.ID].Stack / 2 + ItemSlots[sc.ID].Stack % 2);
                        ItemSlots[sc.ID] = new Slot(ItemSlots[sc.ID].Item, ItemSlots[sc.ID].Stack / 2);
                        Interface.MainWindow.isDragging = true;
                    }
                }
                else if (Interface.MainWindow.isDragging && CanAdd && CheckAdd(Interface.MainWindow.mouseSlot)) //If we are currently dragging an item
                {
                    if (ItemSlots[sc.ID].Item == Item.Blank) //If the slot we are attmepting to put it in is blank
                    {
                        if (e.Button == MouseButton.Left) //btnLeft click? Just dump the whole thing!
                        {
                            Interface.MainWindow.isDragging = false; //Not dragging anymore 
                            ItemSlots[sc.ID] = new Slot(Interface.MainWindow.mouseSlot.Item, Interface.MainWindow.mouseSlot.Stack); //Add the new item stack
                            Interface.MainWindow.mouseSlot = new Slot(Item.Blank, 0);
                        }
                        else if (e.Button == MouseButton.Right) //btnRight click? Dump 1 from the stack
                        {
                            ItemSlots[sc.ID] = new Slot(Interface.MainWindow.mouseSlot.Item, 1); //Create the new slot with 1 item
                            Interface.MainWindow.mouseSlot.Stack--;
                            if (Interface.MainWindow.mouseSlot.Stack <= 0)
                            {

                                Interface.MainWindow.mouseSlot = new Slot(Item.Blank, 0);
                                Interface.MainWindow.isDragging = false;
                                Interface.MainWindow.mouseSlot.Stack = 0;
                            }
                        }
                    }
                    else //If the slot we are attempting to put it in ISNT blank
                    {

                        Slot s = ItemSlots[sc.ID];
                        //If the slot type is the same
                        if (Interface.MainWindow.mouseSlot.Item.ID == s.Item.ID)
                        {
                            if (e.Button == MouseButton.Left && Interface.MainWindow.mouseSlot.Stack + s.Stack <= s.Item.MaxStack) //try to merge the stacks
                            {
                                Interface.MainWindow.isDragging = false; //Not dragging anymore 

                                ItemSlots[sc.ID].Stack += Interface.MainWindow.mouseSlot.Stack;
                                Interface.MainWindow.mouseSlot = new Slot(Item.Blank, 0);
                            }
                            else if (e.Button == MouseButton.Right && s.Stack + 1 <= s.Item.MaxStack) //Add one to the current stack
                            {
                                ItemSlots[sc.ID].Stack++;
                                Interface.MainWindow.mouseSlot.Stack--;
                                if (Interface.MainWindow.mouseSlot.Stack <= 0)
                                {

                                    Interface.MainWindow.mouseSlot = new Slot(Item.Blank, 0);
                                    Interface.MainWindow.isDragging = false;
                                    Interface.MainWindow.mouseSlot.Stack = 0;
                                }

                            }
                        }
                        else if (Interface.MainWindow.mouseSlot.Item.ID != s.Item.ID)
                        {
                            Slot tempMouse = (Slot)Interface.MainWindow.mouseSlot.Clone();
                            Interface.MainWindow.mouseSlot = (Slot)ItemSlots[sc.ID].Clone();
                            ItemSlots[sc.ID] = tempMouse;
                        }
                    }
                }
            }
            OnMoveItem(sender, new TomShane.Neoforce.Controls.EventArgs());
        }
        public bool CheckAdd(Slot s)
        {
            return OnCheckAdd(this, s);
        }
        public void button_MouseDown(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (((Button)sender).Parent.Top < this.Height)
            {
                OnDeSelectItem(this, new TomShane.Neoforce.Controls.EventArgs());
                if (SelectedInventory != null)
                    SelectedInventory.Color = Color.White;
                if (Manager.Skin.Name == "Red")
                    ((Button)sender).Color = Color.Red;
                if (Manager.Skin.Name == "Blue")
                    ((Button)sender).Color = Color.SkyBlue;
                if (Manager.Skin.Name == "Green")
                    ((Button)sender).Color = Color.Green;
                SelectedInventory = (Button)sender;
                Selected = ((SlotControl)(((Button)sender)).Parent).ID;
                OnSelectItem(this, new TomShane.Neoforce.Controls.EventArgs());
            }
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
             //base.DrawControl(renderer,rect,gameTime);
        }
    }
}
