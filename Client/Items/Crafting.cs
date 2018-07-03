using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public static class CraftingRecipies
    {
        public static Dictionary<Item,Recipe> Recipies = new Dictionary<Item,Recipe>();

        static CraftingRecipies()
        {
            Recipies.Add(Item.CraftingTable, new Recipe(1, new Slot(Item.Wood, 15)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic }, Station = Item.Blank});
            Recipies.Add(Item.Furnace, new Recipe(1, new Slot(Item.Stone, 15)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Anvil, new Recipe(1, new Slot(Item.IronBar, 15)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Wood, new Recipe(4, new Slot(Item.Log, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic }, Station = Item.Blank });
            Recipies.Add(Item.WoodPlank, new Recipe(1, new Slot(Item.Wood, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Stick, new Recipe(4, new Slot(Item.Log, 1)) { Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.Materials }, Station = Item.Blank });
            Recipies.Add(Item.WoodenTable, new Recipe(1, new Slot(Item.Wood, 12)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture} });
            Recipies.Add(Item.WoodenChair, new Recipe(1, new Slot(Item.Wood, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } });
            Recipies.Add(Item.WoodenBed, new Recipe(1, new Slot(Item.Wood, 20)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } });
            Recipies.Add(Item.WoodenNightstand, new Recipe(1, new Slot(Item.Wood, 5)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } });
            Recipies.Add(Item.WoodenBench, new Recipe(1, new Slot(Item.Wood, 20)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture }, Station = Item.Sawmill });
            Recipies.Add(Item.WoodenCloset, new Recipe(1, new Slot(Item.Wood, 40)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture }, Station = Item.Sawmill });
            Recipies.Add(Item.WoodenDesk, new Recipe(1, new Slot(Item.Wood, 45)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture }, Station = Item.Sawmill });
            Recipies.Add(Item.Bookshelf, new Recipe(1, new Slot(Item.Wood, 40)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture }, Station = Item.Sawmill });
            Recipies.Add(Item.CoffeeTable, new Recipe(1, new Slot(Item.Wood, 8)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } });
            Recipies.Add(Item.WoodenChest, new Recipe(1, new Slot(Item.Wood, 10), new Slot(Item.IronBar, 1), new Slot(Item.Stone, 2)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksStorage } });
            Recipies.Add(Item.WoodenCrate, new Recipe(1, new Slot(Item.Wood, 15), new Slot(Item.WoodPlank, 10)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksStorage } });
            Recipies.Add(Item.MetalCrate, new Recipe(1, new Slot(Item.SteelPlating, 10)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksStorage } });
            Recipies.Add(Item.WoodenDoor, new Recipe(1, new Slot(Item.WoodPlank, 6)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.WoodLadder, new Recipe(3, new Slot(Item.Stick, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.WoodBeam, new Recipe(1, new Slot(Item.Stick, 1), new Slot(Item.Log, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic }, Station = Item.Sawmill});
            Recipies.Add(Item.WoodSupport, new Recipe(1, new Slot(Item.Wood, 6)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced }, Station = Item.Sawmill });
            Recipies.Add(Item.WoodPlankSupport, new Recipe(1, new Slot(Item.WoodPlank, 6)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced }, Station = Item.Sawmill });
            Recipies.Add(Item.WoodPlatform, new Recipe(2, new Slot(Item.Stick, 2), new Slot(Item.Wood, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.WoodStair, new Recipe(2, new Slot(Item.Wood, 3)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.WoodPlankPlatform, new Recipe(2, new Slot(Item.Stick, 2), new Slot(Item.WoodPlank, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.WoodPlankStair, new Recipe(2, new Slot(Item.WoodPlank, 3)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.StonePlatform, new Recipe(2, new Slot(Item.Stone, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.StoneStair, new Recipe(2, new Slot(Item.Stone, 3)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.MetalLadder, new Recipe(3, new Slot(Item.Steel, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic }, Station = Item.Anvil });
            Recipies.Add(Item.MetalPole, new Recipe(1, new Slot(Item.Steel, 2)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } , Station = Item.Anvil });
            Recipies.Add(Item.MetalBeam, new Recipe(1, new Slot(Item.Steel, 2)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic }, Station = Item.Anvil });
            Recipies.Add(Item.CobbleStone, new Recipe(1, new Slot(Item.Stone, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Sign, new Recipe(2, new Slot(Item.Wood, 3), new Slot(Item.Stick,1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Torch, new Recipe(4, new Slot(Item.Coal, 1), new Slot(Item.Stick, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksLightSource }, Station = Item.Blank });
            Recipies.Add(Item.Steel, new Recipe(3, new Slot(Item.IronBar, 1)) { Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic }, Station = Item.Anvil });
            Recipies.Add(Item.SteelPlating, new Recipe(2, new Slot(Item.Steel, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced }, Station = Item.Anvil });
            Recipies.Add(Item.MetalBlock, new Recipe(3, new Slot(Item.Steel, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced }, Station = Item.Anvil });
            Recipies.Add(Item.Pillar, new Recipe(1, new Slot(Item.Stone, 2)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.CinderBlock, new Recipe(1, new Slot(Item.Stone, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced} });
            Recipies.Add(Item.Sidewalk, new Recipe(1, new Slot(Item.Stone, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced } });
            Recipies.Add(Item.StoneBrick, new Recipe(1, new Slot(Item.Stone, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.Brick, new Recipe(1, new Slot(Item.Clay, 4)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } });
            Recipies.Add(Item.FlintAndSteel, new Recipe(1, new Slot(Item.Flint, 1), new Slot(Item.Steel, 1)) { Categories = new List<ItemCategory> { ItemCategory.ToolAll } });
            Recipies.Add(Item.StonePickaxe, new Recipe(1, new Slot(Item.Stick, 2), new Slot(Item.Stone, 4)) { Station = Item.CraftingTable, Categories = new List<ItemCategory> { ItemCategory.ToolAll, ItemCategory.ToolPicaxe } });
            Recipies.Add(Item.Glass, new Recipe(1, new Slot(Item.Sand,1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic } });
            Recipies.Add(Item.WoodenWindow, new Recipe(1, new Slot(Item.Glass, 1), new Slot(Item.Wood,1)) { Station = Item.CraftingTable, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic } });
            Recipies.Add(Item.GlassBlock, new Recipe(1, new Slot(Item.Glass, 2)) { Station = Item.CraftingTable, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic } });
            Recipies.Add(Item.CopperBar, new Recipe(1, new Slot(Item.CopperOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.IronBar, new Recipe(1, new Slot(Item.IronOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.SilverBar, new Recipe(1, new Slot(Item.SilverOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.GoldBar, new Recipe(1, new Slot(Item.GoldOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.Ruby, new Recipe(1, new Slot(Item.RubyOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.Diamond, new Recipe(1, new Slot(Item.DiamondOre, 1)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.Coal, new Recipe(1, new Slot(Item.Log, 2)) { Station = Item.Furnace, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.Flint, new Recipe(1, new Slot(Item.IronOre, 1), new Slot(Item.Quartz, 1)) { Station = Item.CraftingTable, Categories = new List<ItemCategory> { ItemCategory.MaterialsAll, ItemCategory.MaterialsBasic, ItemCategory.MaterialsOre } });
            Recipies.Add(Item.MetalTable, new Recipe(1, new Slot(Item.Steel, 12)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } ,Station = Item.Anvil,});
            Recipies.Add(Item.MetalChair, new Recipe(1, new Slot(Item.Steel, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } ,Station = Item.Anvil,});
            Recipies.Add(Item.MetalPlatform, new Recipe(2, new Slot(Item.Steel, 2), new Slot(Item.Wood, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } ,Station = Item.Anvil,});
            Recipies.Add(Item.MetalStair, new Recipe(2, new Slot(Item.Steel, 3)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBasic } ,Station = Item.Anvil,});
            Recipies.Add(Item.MetalBed, new Recipe(1, new Slot(Item.Steel, 15)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksFurniture } ,Station = Item.Anvil,});
            Recipies.Add(Item.WoodFence, new Recipe(4, new Slot(Item.WoodPlank, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBackground } });
            Recipies.Add(Item.PicketFence, new Recipe(4, new Slot(Item.Wood, 4)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBackground } });
            Recipies.Add(Item.ChainLinkFence, new Recipe(4, new Slot(Item.Steel, 2), new Slot(Item.MetalBeam)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksBackground } });
            Recipies.Add(Item.Bucket, new Recipe(1, new Slot(Item.IronBar, 2)) { Categories = new List<ItemCategory> { ItemCategory.ToolAll, ItemCategory.Tools, } });
            Recipies.Add(Item.Spike, new Recipe(1, new Slot(Item.IronBar, 1)) { Categories = new List<ItemCategory> { ItemCategory.BlocksAll, ItemCategory.BlocksAdvanced}, Station = Item.Anvil});

            //GetLevels();
        }

        //private static void GetLevels()
        //{
        //    foreach (KeyValuePair<Item,Recipe> r in Recipies)
        //    {
        //        int level = -1;
        //        GetCreator(new Slot[] { new Slot(r.Key) }, r.Key, ref level);
        //    }
        //}

        //private static void GetCreator(Slot[] Items, Item origin, ref int level)
        //{
        //    foreach (Slot ing in Items)
        //    foreach (Item i in Item.ItemList)
        //    {
        //        if (i.ID == ing.Item.ID)
        //        {
        //            level++;
        //            origin.Teir = level;
        //            if (Recipies.ContainsKey(ing.Item))
        //            {
        //                GetCreator(Recipies[ing.Item].Ingredients, origin, ref level);
        //            }
        //        }
        //    }
        //}
    }
    /// <summary>
    /// Stores recipie data for the crafting of items
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// An array of all the ingredients needed to make the item
        /// </summary>
        public Slot[] Ingredients;
        /// <summary>
        /// When the proper recipie is created, how many items should we give? (Does a stick make 1 torch? or 2? etc)
        /// </summary>
        public int AmountToMake;

        /// <summary>
        /// The station needed for crafting this ingredient such as a crafting table or furnace
        /// </summary>
        public BlockItem Station;

        public List<ItemCategory> Categories = new List<ItemCategory>();

        public Recipe(int amountToMake = 1, params Slot[] ingredients)
        {
            AmountToMake = amountToMake;
            Ingredients = ingredients;
            Station = Item.CraftingTable;
        }
    }
}
