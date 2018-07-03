using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public class ItemCategory
    {
        public static List<ItemCategory> ItemCategories = new List<ItemCategory>();

        public string Name;
        public string Description;
        public bool TopLevel;
        public ItemCategory Parent;

        public static ItemCategory All, Blocks, Weapons, Armor, Tools, Materials;

        public static ItemCategory BlocksAll, BlocksBasic, BlocksAdvanced, BlocksFurniture, BlocksDecoration, BlocksStorage, BlocksLightSource, BlocksElectronic, BlocksBackground;

        public static ItemCategory WeaponsAll, WeaponsMelee, WeaponsRanged, WeaponsExplosive, WeaponsConsumable;

        public static ItemCategory ArmorAll, ArmorHead, ArmorChest, ArmorLeg, ArmorFoot;

        public static ItemCategory ToolAll, ToolPicaxe, ToolAxe, ToolsFarming;

        public static ItemCategory MaterialsAll, MaterialsBasic, MaterialsAdvanced, MaterialsOre;

        /// <summary>
        /// Creates a new sub item category (child)
        /// </summary>
        /// <param name="parent">The top-level (parent) category this is in</param>
        public ItemCategory(string name, string description, ItemCategory parent)
        {
            Name = name;
            Description = description;
            TopLevel = false;
            Parent = parent;
            ItemCategories.Add(this);
        }
        /// <summary>
        /// Creates a new top-level (Parent) item category
        /// </summary>
        public ItemCategory(string name, string description)
        {
            Name = name;
            Description = description;
            TopLevel = true;
            ItemCategories.Add(this);
        }

        static ItemCategory()
        {
            All = new ItemCategory("All", "Every craftable item in Zarknorth.");
            Blocks = new ItemCategory("Blocks", "The core, placeable objects of the world.");
            Weapons = new ItemCategory("Weapons", "Used to kill entities and destroy buildings.");
            Armor = new ItemCategory("Armor", "Defensive items to wear.");
            Tools = new ItemCategory("Tools", "Used for mining, cutting wood, and tilling land.");
            Materials = new ItemCategory("Materials", "Basic and advanced parts to make items with.");

            BlocksAll = new ItemCategory("All Blocks", "The core, placeable objects of the world.", Blocks);
            BlocksBasic = new ItemCategory("Basic Blocks", "Simple blocks such as wood and cobblestone.", Blocks);
            BlocksAdvanced = new ItemCategory("Advanced Blocks", "Complex blocks such as Sci-Fi blocks or Corrogated Metal.", Blocks);
            BlocksFurniture = new ItemCategory("Furniture", "Household and lounging objects.", Blocks);
            BlocksDecoration = new ItemCategory("Decorations", "Decor such as plants, vases and paintings.", Blocks);
            BlocksStorage = new ItemCategory("Storage", "Items to keep items safe and organized.", Blocks);
            BlocksLightSource = new ItemCategory("Light Sources", "Blocks to provide light such as torches and lamps.", Blocks);
            BlocksLightSource = new ItemCategory("Electronics", "Blocks to interact with the wiring system.", Blocks);
            BlocksBackground = new ItemCategory("Background Blocks", "Blocks on the background layer.", Blocks);

            WeaponsAll = new ItemCategory("All Weapons", "Used to kill entities and destroy buildings.", Weapons);
            WeaponsMelee = new ItemCategory("Melee Weapons", "Swords and other slicing weapons.", Weapons);
            WeaponsRanged = new ItemCategory("Ranged Weapons", "Bows and guns.", Weapons);
            WeaponsExplosive = new ItemCategory("Explosives", "Explosive items that can cause harm and break blocks.", Weapons);
            WeaponsConsumable = new ItemCategory("Consumable Weapons", "One time use throwable items.", Weapons);

            ToolAll = new ItemCategory("All Tools", "Used for mining, chopping wood, and tilling land.", Tools);
            ToolPicaxe = new ItemCategory("Picaxes", "Used for mining.", Tools);
            ToolAxe = new ItemCategory("Axes", "Used for chopping.", Tools);
            ToolsFarming = new ItemCategory("Farming Tools", "Tools for tending to land.", Tools);

            MaterialsAll = new ItemCategory("All Materials", "Basic and advanced parts to make items with.", Materials);
            MaterialsBasic = new ItemCategory("Basic Materials", "Basic parts like sticks for making simple items.", Materials);
            MaterialsAdvanced = new ItemCategory("Advanced Materials", "Advanced parts to build more compex systems.", Materials);
            MaterialsOre = new ItemCategory("Ore Materials", "Refined ores.", Materials);
        }
    }
}
