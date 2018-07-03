using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Controls = TomShane.Neoforce.Controls;
using System.IO;
using System.Linq;
using ZarknorthClient;
using Cyral.Extensions;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient.Interface
{
    public class TaskItemBox: Dialog
    {
        public SlotContainer slotContainer;
        public Slot[] itemSlots;
        public TextBox searchBox;

        public TaskItemBox(Manager manager)
            : base(manager)
        {

            Resizable = false;

            Text = "Item Box";
            TopPanel.Visible = true;
            Remove(BottomPanel);
            Caption.Text = "Find and use any item available in Sandbox Mode.";
            Description.Text = "";
            Description.TextColor = Color.Gray;
            Caption.TextColor = Color.LightGray;

            ClientWidth = ((32 + 4) * 10) + 26;
            ClientHeight = 584 - 64;
            Left = 16;
            Top = Interface.MainWindow.sidebar.Height + 16 + Interface.MainWindow.sidebar.Top - 8;

            Closed += TaskItemBox_Closed;

            searchBox = new TextBox(manager);
            searchBox.Init();
            searchBox.Bottom = TopPanel.Bottom - 10;
            searchBox.Left = 8;
            searchBox.TextChanged += searchBox_TextChanged;
            searchBox.FocusGained += searchBox_FocusGained;
            searchBox.FocusLost += searchBox_FocusLost;
            searchBox.Text = "Search...";
            Add(searchBox);

            //For some reason Center() changes with width, this resets it to the correct amount.
            ClientWidth = ((32+ 4) * 10) + 26;
            FillBox();
            
        }

        void searchBox_FocusLost(object sender, Controls.EventArgs e)
        {
            if (string.IsNullOrEmpty(searchBox.Text))
                searchBox.Text = "Search...";
        }

        void searchBox_FocusGained(object sender, Controls.EventArgs e)
        {
            if (searchBox.Text == "Search...")
                searchBox.Text = "";
        }

        void searchBox_TextChanged(object sender, Controls.EventArgs e)
        {
            FillBox();
        }
        public override void Show()
        {
            Interface.MainWindow.hiding = false;
            Interface.MainWindow.expanding = true;
            base.Show();
        }
        void FillBox()
        {

            List<Item> Items = Item.ItemList.ToList();
            Items = Items.Where(x => !((x is BackgroundBlockItem) && (x as BackgroundBlockItem).ForegroundEquivelent != null)).ToList<Item>();
            //Reset
            if (slotContainer == null)
            itemSlots = new Slot[10 * ((Items.Count / 10) + (Items.Count % 10) - 1)];
            for (int i = 0; i < Items.Count; i++)
                itemSlots[i] = new Slot(Item.Blank);
            if (!string.IsNullOrEmpty(searchBox.Text) && searchBox.Text != "Search...")
                Items = Items.Where(x => x.Name.ToLowerFast().Contains(searchBox.Text.Trim().ToLowerFast())).ToList<Item>();
  
            for (int i = 0; i < Items.Count; i++)
            {
                itemSlots[i] = new Slot(Items[i], Items[i].MaxStack);
            }
            if (slotContainer == null)
            {
                slotContainer = new SlotContainer(Manager, 10, (Items.Count / 10) + (Items.Count % 10) - 1, true);
                slotContainer.Init();
                slotContainer.ItemSlots = itemSlots;
                slotContainer.Left = 8;
                slotContainer.Top = TopPanel.Height + TopPanel.Top + 8;
                slotContainer.ShiftClickItems += slotContainer_ShiftClickItems;
                slotContainer.CanAdd = false;
                slotContainer.MoveItems += slotContainer_MoveItems;
                Add(slotContainer);
            }
        }

        void slotContainer_MoveItems(object sender, Controls.EventArgs e)
        {
            FillBox();
        }
        void slotContainer_ShiftClickItems(Slot slot)
        {
            for (int i = 0; i < Interface.MainWindow.inventory.ItemSlots.Length; i++)
            {
                if (Interface.MainWindow.inventory.ItemSlots[i].Equals(Slot.Empty))
                {
                    Slot s = Interface.MainWindow.inventory.ItemSlots[i];
                    Interface.MainWindow.inventory.ItemSlots[i] = (Slot)slot.Clone();
                    slotContainer.ItemSlots[slotContainer.Selected] = (Slot)s.Clone();
                    Interface.MainWindow.inventory.Refresh();

                    if (Interface.MainWindow.CraftingWindow != null && Interface.MainWindow.CraftingWindow.Visible)
                    {
                        Interface.MainWindow.CraftingWindow.UpdateItemPanel(this);
                        Interface.MainWindow.CraftingWindow.UpdateItemList(this);
                    }
                    return;
                }
            }
        }
        public override void Hide()
        {
            MainWindow.SandboxInventory.Enabled = true;
            bool noclose = false;
            foreach (Control c in Manager.Controls)
                if (c != this && c is TaskStorage)
                    noclose = true;
            if (!noclose && Interface.MainWindow.mouseSlot.Equals(Slot.Empty))
                if (Interface.MainWindow.expanded == true)
                    Interface.MainWindow.hiding = true;
            base.Hide();
        }
        void TaskItemBox_Closed(object sender, WindowClosedEventArgs e)
        {
            this.Hide();
        }
    }
}
