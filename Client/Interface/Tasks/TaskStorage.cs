using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Controls = TomShane.Neoforce.Controls;
using System.IO;
using ZarknorthClient;
using Cyral.Extensions;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient.Interface
{
    public class TaskStorage : Dialog
    {
        public SlotContainer slotContainer;
        private ImageBox imageCaption;

        public TaskStorage(Manager manager, Slot[] itemSlots, BlockItem storageItem, int slotsX, int slotsY)
            : base(manager)
        {
            imageCaption = new ImageBox(manager);
            imageCaption.Init();
            if (storageItem.RenderMode == BlockRenderMode.Single)
                imageCaption.Image = ContentPack.Textures["spritesheets\\" + storageItem.Name];
            else
            imageCaption.Image = ContentPack.Textures["items\\" + storageItem.Name];
            imageCaption.Left = 8;
            imageCaption.Width = imageCaption.Image.Width;
            imageCaption.Height = imageCaption.Image.Height;
            imageCaption.Top = 4;
            Add(imageCaption);
            Caption.Left = Description.Left = imageCaption.Left + imageCaption.Width + 8;
            TopPanel.Height = imageCaption.Image.Height + 12;
            Resizable = false;

            Text = storageItem.Name;
            TopPanel.Visible = true;
            Remove(BottomPanel);
            Caption.Text = storageItem.Name;
            Description.Text = storageItem.Description + " - " + storageItem.StorageSlots.X * storageItem.StorageSlots.Y + " Slots";
            Description.TextColor = Color.Gray;
            Caption.TextColor = Color.LightGray;

            slotContainer = new SlotContainer(Manager, slotsX, slotsY);
            slotContainer.Init();
            slotContainer.ItemSlots = itemSlots;
            slotContainer.Left = 8;
            slotContainer.Top = TopPanel.Height + TopPanel.Top + 8;
            slotContainer.ShiftClickItems += slotContainer_ShiftClickItems;
            Add(slotContainer);

            ClientWidth = slotContainer.Left + slotContainer.ClientWidth;
            ClientHeight = slotContainer.Top + slotContainer.ClientHeight;
            Center();

            Top = Interface.MainWindow.inventory.Top + Interface.MainWindow.InventoryOpenHeight + 48;
            Closed += TaskStorage_Closed;
            SendToBack();

            //For some reason Center() changes with width, this resets it to the correct amount.
            ClientWidth = slotContainer.Left + slotContainer.ClientWidth;

            //Open inventory
            Interface.MainWindow.hiding = false;
            Interface.MainWindow.expanding = true;
   
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

        void TaskStorage_Closed(object sender, WindowClosedEventArgs e)
        {
            this.Dispose();
            //Close the window, if no more chests are open, close inventory
            bool noclose = false;
            foreach (Control c in Manager.Controls)
                if (c != this && c is TaskStorage)
                    noclose = true;
            if (!noclose && Interface.MainWindow.mouseSlot.Equals(Slot.Empty))
                if (Interface.MainWindow.expanded == true)
                    Interface.MainWindow.hiding = true;
        }
    }
}
