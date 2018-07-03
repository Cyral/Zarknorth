using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
     /// <summary>
    /// A Slot for inventory, OR Ingredient for recipies
    /// </summary>
    public class Slot
    {
        public Item Item;
        public int Stack;

        public static Slot Empty;

        public Slot(Item item,int stack_ = 0)
        {
            this.Item = item;
            Stack = stack_;
        }
        public void Add()
        {
            if (Stack <= Item.MaxStack)
            Stack++;
           
        }
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        public override bool Equals(object obj)
        {
            return Stack == ((Slot)obj).Stack && Item.ID == ((Slot)obj).Item.ID;
        }
        public void Sub(int amount = 1)
        {
            Stack-= amount;
            if (Stack <= 0)
            {
                //Automaticly set the item and stack to blank and 0 and reset the mouse and dragging variables
                Item = Item.Blank;
                Interface.MainWindow.mouseSlot = new Slot(Item.Blank, 0);
                Interface.MainWindow.isDragging = false;
                Stack = 0;
            }
        }
        /// <summary>
        /// Finds and removes the specified items from the Slot array
        /// </summary>
        /// <param name="slot">The item and amount to remove</param>
        /// <param name="inventory">Slot array to look for</param>
        public static void RemoveAmount(Slot slot, Slot[] inventory)
        {
             int amountLeftToRemove = slot.Stack;
             foreach (Slot s in inventory)
             {
                 if (s.Item.ID == slot.Item.ID)
                 {
                     int amountRemoved = Math.Min(amountLeftToRemove, s.Stack);
                     s.Sub(amountRemoved);
                     amountLeftToRemove -= amountRemoved;

                     if (amountLeftToRemove == 0)
                         return;
                 }
             }
        }
        /// <summary>
        /// Check to see how much of the specified item is in the inventory
        /// </summary>
        /// <param name="check">Item to check for</param>
        /// <param name="inventory">Inventory slots to check in</param>
        /// <returns>How many items the inventory has that match</returns>
        public static int HowMany(Item check, Slot[] inventory)
        {
            int howMany = 0;

            foreach (Slot s in inventory)
                if (s.Item.ID == check.ID)
                    howMany += s.Stack;

            return howMany;
        }
        /// <summary>
        /// Checks if there are enough of the specified item and stack in the inventory
        /// </summary>
        /// <param name="check">Item and amount to check for</param>
        /// <param name="inventory">Slots to search through</param>
        /// <returns>If there is enough</returns>
        public static bool HasEnough(Slot check, Slot[] inventory)
        {
            return HowMany(check.Item, inventory) >= check.Stack;
        }
    }
}
