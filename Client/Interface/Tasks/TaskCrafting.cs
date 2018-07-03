using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class TaskCrafting : Dialog
    {
        private ImageBox imageCaption;
        private ControlList<ItemListControl> itemSelector;

        private GroupPanel grpPnlRecipie;
        private GroupPanel grpPnlItem;
        private GroupPanel grpPnlReq;
        private GroupPanel grpPnlStats;

        private ComboBox cmbTopCat;
        private ComboBox cmbSubCat;
        private Panel pnlItem;

        private Panel pnlCraft;

        private ImageBox imgItem;
        private Label lblName, lblDesc, lblIngredients, lblStation;

        private SpinBox spnCraft;
        private Button btnCraft;

        private SlotContainer sltOutput, sltInput;

        private CheckBox chkHasMaterials, chkAtStation;
        private ProgressBar prgFuel, prgOutput;
        private TextBox txtSearch;

        Slot[] itemSlotOutput = new Slot[1];
        Slot[] itemSlotInput = new Slot[1];
        private string FilterString = string.Empty;
        private List<Tuple<ImageBox, Label>> ingredientsNeeded = new List<Tuple<ImageBox, Label>>();
        private ImageBox imgStationNeeded;
        private Label lblStationNeeded;

        private float PowerLeft;
        private float OutputLeft = 0;

        private Label[] ItemStatLabels;
        private KeyValuePair<Item,Recipe> CurrentRecipe;
        private BlockItem Station;

        public TaskCrafting(Manager manager)
            : base(manager)
        {
            //Set up the window
            TopPanel.Visible = true;
            BottomPanel.Visible = false;
            Resizable = false;
            Height = 512;
            Width = 696;

            imageCaption = new ImageBox(manager);
            imageCaption.Init();
            imageCaption.Left = 8;
            imageCaption.Width = 48;
            imageCaption.Height = 48;
            imageCaption.Top = -4;
            Add(imageCaption);
            Caption.Left = Description.Left = imageCaption.Left + imageCaption.Width + 8;
            TopPanel.Height = imageCaption.Height + 8;

            grpPnlRecipie = new GroupPanel(manager);
            grpPnlRecipie.Init();
            grpPnlRecipie.Left = 4;
            grpPnlRecipie.Top = TopPanel.Height + 2;
            grpPnlRecipie.Width = (ClientWidth / 2) -4 -2;
            grpPnlRecipie.Height = ClientHeight - grpPnlRecipie.Top - 6;
            grpPnlRecipie.Text = "Recipies";
            grpPnlRecipie.Color = Color.White;
            Add(grpPnlRecipie);

            grpPnlItem = new GroupPanel(manager);
            grpPnlItem.Init();
            grpPnlItem.Left = (ClientWidth / 2) + 2;
            grpPnlItem.Top = TopPanel.Height + 2;
            grpPnlItem.Width = (ClientWidth / 2) - 6;
            grpPnlItem.Height = ClientHeight - grpPnlRecipie.Top - 6;
            grpPnlItem.Text = "Item";
            grpPnlItem.Color = Color.White;
            Add(grpPnlItem);

            pnlItem = new Panel(manager);
            pnlItem.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            pnlItem.Init();
            pnlItem.Width = grpPnlItem.ClientWidth;
            pnlItem.Height = 64;
            pnlItem.BevelBorder = BevelBorder.Bottom;
            grpPnlItem.Add(pnlItem);

            grpPnlReq = new GroupPanel(manager);
            grpPnlReq.Init();
            grpPnlReq.Left = -1;
            grpPnlReq.Top = 62;
            grpPnlReq.Width = (ClientWidth / 2) - 6;
            grpPnlReq.Height = 84;
            grpPnlReq.Text = "Requirements";
            grpPnlReq.Color = Color.White;
            grpPnlItem.Add(grpPnlReq);


            imgItem = new ImageBox(manager);
            imgItem.Init();
            imgItem.Left = 8;
            imgItem.Width = 0;
            imgItem.Height = 0;
            imgItem.Top = 8;
            pnlItem.Add(imgItem);

            lblName = new Label(manager);
            lblName.Init();
            lblName.Left = imgItem.Width + imgItem.Left + 6;
            lblName.Top = imgItem.Top;
            lblName.Width = pnlItem.Width - lblName.Left;
            lblName.Font = FontSize.Default12;
            pnlItem.Add(lblName);

            lblDesc = new Label(manager);
            lblDesc.Init();
            lblDesc.Left = imgItem.Width + imgItem.Left + 10;
            lblDesc.Top = lblName.Top+lblName.Height;
            lblDesc.Width = pnlItem.Width - lblName.Left;
            lblDesc.TextColor = Color.Gray;
            pnlItem.Add(lblDesc);

            lblIngredients = new Label(manager);
            lblIngredients.Init();
            lblIngredients.Left = 8;
            lblIngredients.Top = 4;
            lblIngredients.Height = Tile.Height;
            lblIngredients.Text = "Ingredients:";
            lblIngredients.Width = (int)manager.Skin.Fonts[0].Resource.MeasureRichString(lblIngredients.Text, manager).X;
            grpPnlReq.Add(lblIngredients);

            lblStation = new Label(manager);
            lblStation.Init();
            lblStation.Left = 8;
            lblStation.Top = lblIngredients.Top + lblIngredients.Height + 8;
            lblStation.Text = "Station:";
            lblStation.Width = (int)manager.Skin.Fonts[0].Resource.MeasureRichString(lblStation.Text, manager).X;
            grpPnlReq.Add(lblStation);

            cmbTopCat = new ComboBox(manager);
            cmbTopCat.Init();
            cmbTopCat.Left = 8;
            cmbTopCat.Top = 8;
            cmbTopCat.Width = grpPnlRecipie.Width / 2 - 11;
            cmbTopCat.Items.AddRange(ItemCategory.ItemCategories.Where(x => x.TopLevel).Select(element => element.Name).ToList());
            grpPnlRecipie.Add(cmbTopCat);

            cmbSubCat = new ComboBox(manager);
            cmbSubCat.Init();
            cmbSubCat.Left = cmbTopCat.Left + cmbTopCat.Width + 2;
            cmbSubCat.Top = cmbTopCat.Top;
            cmbSubCat.Width = grpPnlRecipie.Width / 2 - 8;
            cmbSubCat.MaxItems = 128;
           
            grpPnlRecipie.Add(cmbSubCat);
            cmbSubCat.Items.Add(ItemCategory.All.Name);


            chkHasMaterials = new CheckBox(manager);
            chkHasMaterials.Init();
            chkHasMaterials.Top = cmbTopCat.Top + cmbTopCat.Height + 8;
            chkHasMaterials.Left = cmbTopCat.Left;
            chkHasMaterials.Text = "Have Materials";
            chkHasMaterials.Width = 106;
            chkHasMaterials.ToolTip.Text = "Only display items that you have materials for.";
            chkHasMaterials.CheckedChanged += chkHasMaterials_CheckedChanged;
            grpPnlRecipie.Add(chkHasMaterials);

            chkAtStation = new CheckBox(manager);
            chkAtStation.Init();
            chkAtStation.Top = cmbTopCat.Top + cmbTopCat.Height + 8;
            chkAtStation.Left = cmbTopCat.Left + chkHasMaterials.Width;
            chkAtStation.Text = "At Station";
            chkAtStation.Width = 80;
            chkAtStation.ToolTip.Text = "Only display items who's crafting station is near.";
            chkAtStation.CheckedChanged += chkAtStation_CheckedChanged;
            grpPnlRecipie.Add(chkAtStation);

            txtSearch = new TextBox(manager);
            txtSearch.Init();
            txtSearch.Top = cmbTopCat.Top + cmbTopCat.Height + 4;
            txtSearch.Left = chkAtStation.Left + chkAtStation.Width;
            txtSearch.Text = "Search...";
            txtSearch.Width = 131;
            txtSearch.ToolTip.Text = "Only display items that contain or match the search text.";
            txtSearch.FocusGained += txtSearch_FocusGained;
            txtSearch.FocusLost += txtSearch_FocusLost;
            txtSearch.TextChanged += txtSearch_TextChanged;
            grpPnlRecipie.Add(txtSearch);


            itemSelector = new ControlList<ItemListControl>(manager);
            itemSelector.Init();
            itemSelector.Width = grpPnlRecipie.ClientWidth - 16;
            itemSelector.Height = grpPnlRecipie.ClientHeight - 16 - chkHasMaterials.Top - chkHasMaterials.Height;
            itemSelector.Left = 8;
            itemSelector.Top = chkHasMaterials.Top + chkHasMaterials.Height + 8;
            grpPnlRecipie.Add(itemSelector);

            pnlCraft = new Panel(manager);
            pnlCraft.Anchor = Anchors.Left | Anchors.Bottom | Anchors.Right;
            pnlCraft.Init();
            pnlCraft.Width = grpPnlItem.ClientWidth;
            pnlCraft.Height = 56 + 8 + 8;
            pnlCraft.Top = grpPnlItem.ClientHeight - pnlCraft.Height;
            pnlCraft.BevelBorder = BevelBorder.Top;
            grpPnlItem.Add(pnlCraft);

            grpPnlStats = new GroupPanel(manager);
            grpPnlStats.Init();
            grpPnlStats.Left = -1;
            grpPnlStats.Top = grpPnlReq.Top + grpPnlReq.Height;
            grpPnlStats.Width = grpPnlReq.Width;
            grpPnlStats.Height = (int)Math.Abs(pnlCraft.Top - (grpPnlReq.Top + grpPnlReq.Height)) + 2;
            grpPnlStats.Text = "Stats";
            grpPnlStats.Color = Color.White;
            grpPnlItem.Add(grpPnlStats);


            btnCraft = new Button(manager);
            btnCraft.Init();
            btnCraft.Enabled = false;
            btnCraft.Text = "Craft";
            btnCraft.Left = (grpPnlItem.Width / 2) - btnCraft.Width - 4;
            btnCraft.Top = grpPnlItem.ClientHeight - btnCraft.Height - 8;
            btnCraft.Click += btnCraft_Click;
            grpPnlItem.Add(btnCraft);


            itemSlotOutput[0] = new Slot(Item.Blank, 0);
            sltOutput = new SlotContainer(manager, 1, 1);
            sltOutput.ItemSlots = itemSlotOutput;
            sltOutput.Init();
            sltOutput.Left = (grpPnlItem.ClientWidth / 2) + 4;
            sltOutput.Top = grpPnlItem.ClientHeight - sltOutput.Height;
            sltOutput.MoveItems += sltOutput_MoveItems;
            sltOutput.CanAdd = false;

            spnCraft = new SpinBox(manager, SpinBoxMode.Range);
            spnCraft.Init();
            spnCraft.Width = btnCraft.Width;
            spnCraft.Left = (grpPnlItem.Width / 2) - spnCraft.Width - 4;
            spnCraft.Minimum = 0;
            spnCraft.Maximum = (int)StackSize.Max;
            spnCraft.ReadOnly = false;
            spnCraft.Rounding = 0;
            spnCraft.Top = sltOutput.Top;
            spnCraft.TextChanged += spnCraft_TextChanged;
            grpPnlItem.Add(spnCraft);
            grpPnlItem.Add(sltOutput);

            itemSlotInput[0] = new Slot(Item.Blank, 0);
            sltInput = new SlotContainer(manager, 1, 1);
            sltInput.ItemSlots = itemSlotInput;
            sltInput.Init();
            sltInput.Left = btnCraft.Left - sltInput.ClientWidth;
            sltInput.Top = grpPnlItem.ClientHeight - sltOutput.Height + 4;
            sltInput.CheckItems += sltInput_CheckItems;
            sltInput.MoveItems += sltInput_MoveItems;
            sltInput.CanAdd = true;
            grpPnlItem.Add(sltOutput);
            grpPnlItem.Add(sltInput);

            prgFuel = new ProgressBar(manager);
            prgFuel.Init();
            prgFuel.Color = Color.Orange;
            prgFuel.Text = "Fuel";
            prgFuel.ToolTip.Text = "Fuel";
            prgFuel.Top = pnlCraft.Top + 4;
            prgFuel.Left = sltInput.Left;
            prgFuel.Width = sltInput.ClientWidth - 8;
            prgFuel.Height = 15;
            grpPnlItem.Add(prgFuel);

            prgOutput = new ProgressBar(manager);
            prgOutput.Init();
            prgOutput.Color = Color.ForestGreen;
            prgOutput.Text = "Output";
            prgOutput.ToolTip.Text = "Output";
            prgOutput.Top = pnlCraft.Top + 4;
            prgOutput.Left = sltOutput.Left;
            prgOutput.Width = sltOutput.ClientWidth - 8;
            prgOutput.Height = 15;
            grpPnlItem.Add(prgOutput);

          

            imgStationNeeded = new ImageBox(Manager);
            imgStationNeeded.Init();
            grpPnlReq.Add(imgStationNeeded);

            lblStationNeeded = new Label(Manager);
            lblStationNeeded.Init();
            grpPnlReq.Add(lblStationNeeded);

            ItemStatLabels = new Label[15];
            for (int j = 0; j < 15; j++)
            {
                ItemStatLabels[j] = new Label(Manager);
                ItemStatLabels[j].Init();
                ingredientsNeeded.Add(Tuple.Create(new ImageBox(Manager), new Label(Manager)));
                ingredientsNeeded[j].Item1.Init();
            }
          
            HideAll();

            SetupCraftingControls(manager);
            Center();
         
        }
        public override void Close(ModalResult modalResult)
        {
            if (ZarknorthClient.Interface.MainWindow.mouseSlot.Item == Item.Blank)
            base.Close(modalResult);
        }
        public void Show(BlockItem craftingStation, bool Update = true)
        {
            craftingStation = UpdateCaption(craftingStation);

            Show();
            if (Update)
            {
                Interface.MainWindow.CraftingWindow.UpdateItemList(this, true);
                Interface.MainWindow.CraftingWindow.UpdateItemPanel(this);
            }

        }

        public BlockItem UpdateCaption(BlockItem craftingStation)
        {
            if (craftingStation == Item.Blank)
            {
                craftingStation = Item.CraftingTable;
                Text = "Crafting";
                Caption.Text = "Crafting";
                Description.Text = "To advance in the game, create a [color:Orange]Crafting Table[/color] with spare wood to unlock new items.";
            }
            else
            {
                Text = craftingStation.Name;
                Caption.Text = craftingStation.Name;
                Description.Text = craftingStation.Description;
            }
            Station = craftingStation;
            imageCaption.Image = craftingStation.Textures[1];
            imageCaption.Width = imageCaption.Image.Width;
            imageCaption.Height = imageCaption.Image.Height;
            Caption.Left = Description.Left = imageCaption.Left + imageCaption.Width + 8;
            return craftingStation;
        }
        private void HideAll()
        {

            btnCraft.Hide();
            sltOutput.Hide();
            spnCraft.Hide();
            btnCraft.Hide();
            sltOutput.Hide();
            sltInput.Hide();
            prgFuel.Hide();
            prgOutput.Hide();
        }
        enum StationType
        {
            Crafting,
            Smelting,
        }
        StationType curStation;
        private bool Smelting;
        private void ResetCraftingControls(Manager manager)
        {
            if (curStation == StationType.Crafting)
            {
                btnCraft.Hide();
                sltOutput.Hide();
                spnCraft.Hide();
            }
            if (curStation == StationType.Smelting)
            {
                btnCraft.Hide();
                sltOutput.Hide();
                sltInput.Hide();
                prgFuel.Hide();
                prgOutput.Hide();
            }
        }
        private void SetupCraftingControls(Manager manager)
        {
            ResetCraftingControls(manager);
            curStation = StationType.Crafting;

            btnCraft.Show();
            btnCraft.Enabled = false;
            btnCraft.Text = "Craft";
            btnCraft.Left = (grpPnlItem.Width / 2) - btnCraft.Width - 4;
            btnCraft.Top = grpPnlItem.ClientHeight - btnCraft.Height - 8;

            itemSlotOutput[0] = new Slot(Item.Blank, 0);
            sltOutput.Show();
            sltOutput.ItemSlots = itemSlotOutput;
            sltOutput.Left = (grpPnlItem.ClientWidth / 2) + 4;
            sltOutput.Top = grpPnlItem.ClientHeight - sltOutput.Height;
            sltOutput.CanAdd = false;

            spnCraft.Show();
            spnCraft.Width = btnCraft.Width;
            spnCraft.Left = (grpPnlItem.Width / 2) - spnCraft.Width - 4;
            spnCraft.ReadOnly = false;
            spnCraft.Rounding = 0;
            spnCraft.Top = sltOutput.Top;
        }

        private void SetupFurnaceControls(Manager manager)
        {
            ResetCraftingControls(manager);
            curStation = StationType.Smelting;

            btnCraft.Show();
            btnCraft.Enabled = false;
            btnCraft.Text = "Smelt";
            btnCraft.Left = (grpPnlItem.Width / 2) - (btnCraft.Width /2);
            btnCraft.Top = grpPnlItem.ClientHeight - btnCraft.Height - 16;

            sltOutput.Show();
            sltOutput.ItemSlots = itemSlotOutput;
            sltOutput.Left = btnCraft.Left + btnCraft.Width + 8;
            sltOutput.Top = grpPnlItem.ClientHeight - sltOutput.Height +4;
            sltOutput.CanAdd = false;

            sltInput.Show();
            sltInput.ItemSlots = itemSlotInput;
            sltInput.Left = btnCraft.Left - sltInput.ClientWidth;
            sltInput.Top = grpPnlItem.ClientHeight - sltOutput.Height + 4;
            sltInput.CanAdd = true;

            prgFuel.Show();
            prgFuel.Color = Color.Orange;
            prgFuel.Text = "Fuel";
            prgFuel.Top = pnlCraft.Top + 4;
            prgFuel.Left = sltInput.Left;
            prgFuel.Width = sltInput.ClientWidth - 8;
            prgFuel.Height = 15;

            prgOutput.Show();
            prgOutput.Color = Color.ForestGreen;
            prgOutput.Text = "Output";
            prgOutput.Top = pnlCraft.Top + 4;
            prgOutput.Left = sltOutput.Left;
            prgOutput.Width = sltOutput.ClientWidth - 8;
            prgOutput.Height = 15;


         
        }

        void sltInput_MoveItems(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //Giveitem
            Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;

            //Get recipie
            Recipe r = null;
            foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                if (kv.Key.ID == item.ID)
                    r = kv.Value;
            if (!Smelting && sltInput != null && (btnCraft.Text != "Refuel" && !sltInput.ItemSlots[0].Equals(Slot.Empty)))
                btnCraft.Enabled = CanCraft(r);
            if (btnCraft.Text == "Refuel" && sltInput.ItemSlots[0].Item.ID != Item.Blank.ID)
            {
                btnCraft.Text = "Smelt";
                btnCraft.TextColor = Color.White;
                btnCraft.Enabled = true;
            
            }
            if (sltInput.ItemSlots[0].Item == Item.Blank && Smelting)
            {
                StopSmelting();
            }
        }

    
        bool sltInput_CheckItems(object o, Slot s)
        {
            return s.Item.Fuel > 0;
        }

        public override void Close()
        {
            //Interface.MainWindow.CraftingWindows.Remove(this);
            //base.Close();
            Hide();
        }
        void chkAtStation_CheckedChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            UpdateItemList(sender);
            itemSelector.ScrollTo(0);
        }

        void chkHasMaterials_CheckedChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            UpdateItemList(sender);
            itemSelector.ScrollTo(0);
        }

        void txtSearch_FocusLost(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if ((sender as TextBox).Text == string.Empty)
                (sender as TextBox).Text = "Search...";
        }

        void txtSearch_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            string Text = (sender as TextBox).Text;
            FilterString = Text.Trim();
            UpdateItemList(sender);

        }

        void txtSearch_FocusGained(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if ((sender as TextBox).Text == "Search...")
                (sender as TextBox).Text = string.Empty;
        }

        void sltOutput_MoveItems(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (((sender as Button).Parent.Parent as SlotContainer).ItemSlots[0].Equals(Slot.Empty))
            {
                if (curStation == StationType.Crafting)
                    btnCraft.Text = "Craft";
                if (curStation == StationType.Smelting && !sltInput.ItemSlots[0].Equals(Slot.Empty))
                UpdateItemPanel(sender);
            }
        }

        void spnCraft_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;

            //Get recipie
            Recipe r = null;
            foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                if (kv.Key.ID == item.ID)
                    r = kv.Value;

            btnCraft.Enabled = CanCraft(r);

            DisplayNeededIngredients(itemSelector, item);

            string suffix = spnCraft.Value > 1 ? " (x" + spnCraft.Value + ")" : "";
            lblName.Text = item.Name + suffix;
        }

        private bool CanCraft(Recipe r)
        {
            //Check if we have enough of the item
            int i = 0;
            foreach (Slot s in r.Ingredients)
            {
                if (Slot.HasEnough(new Slot(s.Item, s.Stack * (int)(spnCraft.Value / r.AmountToMake)), Game.level.Players[0].Inventory))
                    i++;
            }
            return i == r.Ingredients.Count() && BlocksNearby.Contains(r.Station);

           
        }
        /// <summary>
        /// The all important crafting button click
        /// </summary>
        void btnCraft_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;

            //Get recipie
            Recipe r = null;
            foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                if (kv.Key.ID == item.ID)
                    r = kv.Value;

            
            if (curStation == StationType.Crafting)
            {
                foreach (Slot s in r.Ingredients)
                {
                    Slot.RemoveAmount(new Slot(s.Item, s.Stack * (int)(spnCraft.Value / r.AmountToMake)), Game.level.Players[0].Inventory);
                }
                sltOutput.ItemSlots[0] = new Slot(item, r.AmountToMake * (int)(spnCraft.Value / r.AmountToMake));

                btnCraft.Text = "Take Item";
            }
            else if (curStation == StationType.Smelting)
            {
                Smelting = true;
                btnCraft.Text = "Smelting...";
                grpPnlRecipie.Enabled = false;
            }
            btnCraft.Enabled = false;
        }
        protected override void Update(GameTime gameTime)
        {
         
             
   
            if (curStation == StationType.Smelting && Smelting)
            {
              
                if (PowerLeft > 0)
                {
                    PowerLeft -= (((float)gameTime.ElapsedGameTime.TotalMilliseconds * 1000)/ CurrentRecipe.Key.TimeToSmelt) / (float)sltInput.ItemSlots[0].Item.Fuel;
                    prgFuel.Value = (int)((PowerLeft / 1000) * 100);
                    prgFuel.Text = string.Format("Fuel {0:0%}", PowerLeft / 1000);
                    if (PowerLeft > 0)
                    {
                        OutputLeft += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        prgOutput.Value = (int)((OutputLeft / CurrentRecipe.Key.TimeToSmelt) * 100);
                        prgOutput.Text = string.Format("Output {0:0%}", OutputLeft / CurrentRecipe.Key.TimeToSmelt);
                    }
                }
                if (PowerLeft <= 0)
                {
                    sltInput.ItemSlots[0].Sub();
                    if (sltInput.ItemSlots[0].Stack > 0)
                    {
                        PowerLeft = 1000;
                    }
                    else
                    {
                        PowerLeft = 0;
                        StopSmelting();
                    }
                }

                if (OutputLeft >= CurrentRecipe.Key.TimeToSmelt)
                {
                    OutputLeft = 0;
                    prgOutput.Value = (int)((OutputLeft / CurrentRecipe.Key.TimeToSmelt) * 100);
                    prgOutput.Text = "Output 100%";
                    btnCraft.Enabled = false;
                    if (sltInput.ItemSlots[0].Stack <= 0)
                    Smelting = false;

                    //Giveitem
                    Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;

                    //Get recipie
                    Recipe r = null;
                    foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                        if (kv.Key.ID == item.ID)
                            r = kv.Value;

                    //Remove ingredients
                    foreach (Slot s in r.Ingredients)
                    {
                        Slot.RemoveAmount(new Slot(s.Item, s.Stack * (int)(spnCraft.Value / r.AmountToMake)), Game.level.Players[0].Inventory);
                    }
                    DisplayNeededIngredients(this,item);
                    if (sltOutput.ItemSlots[0].Item.ID == Item.Blank.ID)
                        sltOutput.ItemSlots[0] = new Slot(item, 1);
                    else if (sltOutput.ItemSlots[0].Item.ID != Item.Blank.ID && item.ID == sltOutput.ItemSlots[0].Item.ID)
                        sltOutput.ItemSlots[0].Stack++;
                    btnCraft.Text = Smelting ? "Smelting..." : "Smelt";
                }
            }
            base.Update(gameTime);
        }

        private void StopSmelting()
        {
            prgFuel.Value = (int)((PowerLeft / 1000) * 100);
            prgFuel.Text = string.Format("Fuel {0:0%}", PowerLeft / 1000);
            btnCraft.Enabled = false;
            Smelting = false;
            btnCraft.Text = "Refuel";
            grpPnlRecipie.Enabled = true;
        }
        public override void Init()
        {
            itemSelector.ItemIndex = 0;
            cmbTopCat.ItemIndex = 0;
            cmbSubCat.ItemIndex = 0;
            cmbTopCat.ItemIndexChanged += cmbTopCat_ItemIndexChanged;
            cmbSubCat.ItemIndexChanged += cmbSubCat_ItemIndexChanged;
            BlocksNearby = UpdateBlocksNearby();
            UpdateItemList(this, true);
            UpdateItemPanel(this);
            itemSelector.ItemIndexChanged += itemSelector_ItemIndexChanged;
            itemSelector.ItemIndex = 0;
            base.Init();
        }

        public void itemSelector_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (itemSelector.Items.Count() > 0 && itemSelector.ItemIndex >= 0)
            {
                Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;
                Recipe r = null;
                foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                    if (kv.Key.ID == item.ID)
                    {
                        r = kv.Value;
                        CurrentRecipe = kv;
                    }
                spnCraft.Text = r.AmountToMake.ToString();
                spnCraft.Value = r.AmountToMake;
                spnCraft_TextChanged(spnCraft, new TomShane.Neoforce.Controls.EventArgs());
                if (r.Station == Item.CraftingTable)
                {
                    SetupCraftingControls(Manager);
                }
                else if (r.Station.SubType == BlockSubType.Furnace)
                {
                    SetupFurnaceControls(Manager);
                }
                if (BlocksNearby.Contains(r.Station))
                    Show(r.Station,false);
                UpdateItemPanel(sender);
            }
        }

        public void UpdateItemPanel(object sender)
        {
            if (itemSelector.Items.Count() > 0 && itemSelector.ItemIndex > -1 && itemSelector.ItemIndex < itemSelector.Items.Count)
            {

                Item item = (itemSelector.Items[itemSelector.ItemIndex] as ItemListControl).Item;

                imgItem.Image = ContentPack.Textures["items\\" + item.Name];
                imgItem.Width = imgItem.Image.Width;
                imgItem.Height = imgItem.Image.Height;

                //Get recipie
                Recipe r = null;
                foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                    if (kv.Key.ID == item.ID)
                        r = kv.Value;
                if (curStation == StationType.Crafting)
                {
                    btnCraft.Enabled = CanCraft(r);
                }
                else if (curStation == StationType.Smelting)
                {
                    SetSmeltingButton(r);
                }
                //Set the min and max values to craft, minumum being least number able to be crafted
                //Maximum is the max stack, however possibly taken down so it is divisible by the AmountToMake
                spnCraft.Minimum = r.AmountToMake;
                spnCraft.Maximum = (int)Math.Floor((double)item.MaxStack / r.AmountToMake) * r.AmountToMake;
                spnCraft.Step = r.AmountToMake;

                string suffix = spnCraft.Value > 1 ? " (x" + spnCraft.Value + ")" : "";
                lblName.Text = item.Name + suffix;
                lblDesc.Text = lblDesc.ToolTip.Text = item.Description;

                imgItem.Top = (pnlItem.Height / 2) - (imgItem.Height / 2);
                lblName.Top = (pnlItem.Height / 2) - (lblName.Height / 2) - 6;
                lblDesc.Top = (pnlItem.Height / 2) - (lblDesc.Height / 2) + 6;
                lblName.Left = imgItem.Width + imgItem.Left + 8;
                lblDesc.Left = imgItem.Width + imgItem.Left + 8;
                lblDesc.Width = grpPnlItem.Width - lblDesc.Left - 8;

                DisplayNeededIngredients(sender, item);
            }
        }

        private void SetSmeltingButton(Recipe r)
        {
            if (!Smelting && sltInput != null && (btnCraft.Text != "Refuel" && !sltInput.ItemSlots[0].Equals(Slot.Empty)))
                btnCraft.Enabled = CanCraft(r);
            if (Smelting && !CanCraft(r))
            {
                Smelting = false;
                btnCraft.Enabled = true;
                btnCraft.Text = "Smelt";
            }
        }
    
        private void DisplayNeededIngredients(object sender, Item item)
        {

            //Get recipie
            Recipe r = null;
            foreach (KeyValuePair<Item, Recipe> kv in CraftingRecipies.Recipies)
                if (kv.Key.ID == item.ID)
                    r = kv.Value;

            //Check if we have enough of the item, and set up the images
            int i = 0;
            int left = lblIngredients.Left + 4 + lblIngredients.Width;
            for (int j = 0; j < 15; j++)
            {
                ingredientsNeeded[j].Item1.Visible = false;
                ingredientsNeeded[j].Item2.Visible = false;
            }
            foreach (Slot s in r.Ingredients)
            {
                ingredientsNeeded[i].Item1.Visible = true;
                ingredientsNeeded[i].Item2.Visible = true;
                ingredientsNeeded[i].Item1.Image = ContentPack.Textures["items\\" + s.Item.Name];
                ingredientsNeeded[i].Item2.Init();
                ingredientsNeeded[i].Item2.Text = Slot.HowMany(s.Item, Game.level.Players[0].Inventory) + "/" + s.Stack * (spnCraft.Value / r.AmountToMake);
                // ingredientsNeeded[i].Item2.ToolTip.Text = Slot.HowMany(s.Item, Game.level.Players[0].inventory) + "/" + s.Stack;
                ingredientsNeeded[i].Item2.Width = (int)(sender as Control).Manager.Skin.Fonts[0].Resource.MeasureRichString(ingredientsNeeded[i].Item2.Text, (sender as Control).Manager).X;
                ingredientsNeeded[i].Item2.Top = lblIngredients.Top + 4;
                ingredientsNeeded[i].Item1.Top = lblIngredients.Top;
                ingredientsNeeded[i].Item2.Left = left + Tile.Width + 2;
                ingredientsNeeded[i].Item1.Left = left;
                ingredientsNeeded[i].Item1.ToolTip.Text = s.Item.Name;
                left += Tile.Width + 4 + ingredientsNeeded[i].Item2.Width;
                ingredientsNeeded[i].Item1.SizeMode = SizeMode.Fit;
                ingredientsNeeded[i].Item1.Width = Tile.Width;
                ingredientsNeeded[i].Item1.Height = Tile.Height;
                if (!Slot.HasEnough(new Slot(s.Item, s.Stack * (int)(spnCraft.Value / r.AmountToMake)), Game.level.Players[0].Inventory))
                    ingredientsNeeded[i].Item2.TextColor = Color.Red;
                else
                    ingredientsNeeded[i].Item2.TextColor = Color.White;
                grpPnlReq.Add(ingredientsNeeded[i].Item1);
                grpPnlReq.Add(ingredientsNeeded[i].Item2);
                i += 1;


            }
            if (r.Station != Item.Blank)
                imgStationNeeded.Image = ContentPack.Textures["items\\" + r.Station.Name];
            else
                imgStationNeeded.Image = ContentPack.Textures["gui\\icons\\cross"];
            imgStationNeeded.Top = lblStation.Top - 4;
            imgStationNeeded.Left = lblStation.Left + 4 + lblStation.Width;
            imgStationNeeded.SizeMode = SizeMode.Fit;
            imgStationNeeded.Width = Tile.Width;
            imgStationNeeded.Height = Tile.Height;
            if (r.Station != Item.Blank)
                lblStationNeeded.Text = r.Station.Name;
            else
                lblStationNeeded.Text = "No station needed";
            lblStationNeeded.Top = lblStation.Top;
            lblStationNeeded.Left = imgStationNeeded.Left + imgStationNeeded.Width + 4;
            lblStationNeeded.Width = 200;
            if (!GetBlocksInRadius((int)(Game.level.Players[0].Position.X / Tile.Width), (int)(Game.level.Players[0].Position.Y / Tile.Height), 6).Contains(r.Station))
                lblStationNeeded.TextColor = Color.Red;
            else
                lblStationNeeded.TextColor = Color.White;

            string[] str = item.OnPrintStats(new PrintItemDataEventArgs());

            //Clear stats
            foreach (Label l in ItemStatLabels)
                l.Text = "";
            //Show item stats
            for (int j = 0; j < str.Count(); j++)
            {
                ItemStatLabels[j].Init();
                ItemStatLabels[j].Left = 8;
                ItemStatLabels[j].Top = 4 + (j * ItemStatLabels[j].Height);
                ItemStatLabels[j].Width = grpPnlStats.Width;
                ItemStatLabels[j].Text = str[j];
                grpPnlStats.Add(ItemStatLabels[j]);
            }
        }
        public List<BlockItem> GetBlocksInRadius(int X, int Y, int Radius)
        {
            List<BlockItem> Blocks = new List<BlockItem>();
            try
            {
                for (int a = -Radius; a <= Radius; a++)
                {
                    for (int b = -Radius; b <= Radius; b++)
                    {
                        Blocks.Add(Game.level.tiles[X + a, Y + b].Foreground);
                    }
                }
            }
            catch
            {
            }
            return Blocks;
        }

        void cmbSubCat_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            UpdateItemList(sender);
        }
        List<BlockItem> BlocksNearby;
        public void UpdateItemList(object sender, bool updateNearby = false)
        {
            Color redColor = new Color(255, 50, 50, 255);
            Color greenColor = new Color(70, 255, 70, 255);
            if (updateNearby)
                BlocksNearby = UpdateBlocksNearby();
            itemSelector.Items.ForEach(x => (x as Control).Dispose());
            itemSelector.Items.Clear();
            
            UpdateItem(sender, ref redColor, ref greenColor);
        }

        private void UpdateItem(object sender, ref Color redColor, ref Color greenColor)
        {
            if ((string)cmbTopCat.Items[cmbTopCat.ItemIndex] == ItemCategory.All.Name)
            {
                foreach (KeyValuePair<Item, Recipe> r in CraftingRecipies.Recipies)
                {
                    if (FilterString != string.Empty && FilterString != "Search..." && !r.Key.Name.ToLowerFast().Contains(FilterString.ToLowerFast()))
                        continue;
                    int count = 0;
                    for (int i = 0; i < r.Value.Ingredients.Count(); i++)
                    {
                        if (Slot.HasEnough(r.Value.Ingredients[i], Game.level.Players[0].Inventory))
                            count++;
                    }
                    bool AtStation = BlocksNearby.Contains(r.Value.Station);
                    bool HaveMaterials = count == r.Value.Ingredients.Count();
                    bool canCraft = AtStation && HaveMaterials;
                    if (chkHasMaterials.Checked && !HaveMaterials)
                        continue;
                    if (chkAtStation.Checked && !AtStation)
                        continue;
                    itemSelector.Items.Add(new ItemListControl((sender as Control).Manager, r.Key, canCraft ? greenColor : redColor));

                }
                cmbSubCat.ToolTip.Text = ItemCategory.All.Description;
            }
            else
            {
                foreach (KeyValuePair<Item, Recipe> r in CraftingRecipies.Recipies)
                {
                    foreach (ItemCategory c in r.Value.Categories)
                    {
                        if (cmbSubCat.Items.Count > 0 && c.Name == (string)cmbSubCat.Items[cmbSubCat.ItemIndex])
                        {
                            if (FilterString != string.Empty && FilterString != "Search..." && !r.Key.Name.ToLowerFast().Contains(FilterString.ToLowerFast()))
                                continue;
                            int count = 0;
                            for (int i = 0; i < r.Value.Ingredients.Count(); i++)
                            {
                                if (Slot.HasEnough(r.Value.Ingredients[i], Game.level.Players[0].Inventory))
                                    count++;
                            }
                            bool AtStation = BlocksNearby.Contains(r.Value.Station);
                            bool HaveMaterials = count == r.Value.Ingredients.Count();
                            bool canCraft = AtStation && HaveMaterials;
                            if (chkHasMaterials.Checked && !HaveMaterials)
                                continue;
                            if (chkAtStation.Checked && !AtStation)
                                continue;
                            itemSelector.Items.Add(new ItemListControl((sender as Control).Manager, r.Key, canCraft ? greenColor : redColor));
                        }
                    }

                }
                cmbSubCat.ToolTip.Text = ItemCategory.ItemCategories.Where(x => !x.TopLevel && x.Name == (string)cmbSubCat.Items[cmbSubCat.ItemIndex]).Select(element => element.Description).ToList()[0];
            }
        }
        private List<BlockItem> UpdateBlocksNearby()
        {
            try
            {
                List<BlockItem> BlocksNearby = GetBlocksInRadius((int)(Game.level.Players[0].Position.X / Tile.Width), (int)(Game.level.Players[0].Position.Y / Tile.Height), 6);
                if (BlocksNearby.Where(x => x.Station).Count() > 0)
                    Interface.MainWindow.CraftingWindow.UpdateCaption(BlocksNearby.Where(x => x.Station).ElementAt(0));
                else
                    Interface.MainWindow.CraftingWindow.UpdateCaption(Item.Blank);
                return BlocksNearby;
            }
            catch
            {
                return new List<BlockItem>();
            }
        }

        void cmbTopCat_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            cmbTopCat.ToolTip.Text = ItemCategory.ItemCategories.Where(x => x.TopLevel && x.Name == (string)cmbTopCat.Items[cmbTopCat.ItemIndex]).Select(element => element.Description).ToList()[0];
            cmbSubCat.Items.Clear();
            if ((string)cmbTopCat.Items[cmbTopCat.ItemIndex] == ItemCategory.All.Name)
            {
             
                cmbSubCat.Items.Add(ItemCategory.All.Name);
                cmbTopCat.ToolTip.Text = ItemCategory.All.Description;
            }
            else
                cmbSubCat.Items.AddRange(ItemCategory.ItemCategories.Where(x => !x.TopLevel && x.Parent.Name == (string)cmbTopCat.Items[cmbTopCat.ItemIndex]).Select(element => element.Name).ToList());
            cmbSubCat.ItemIndex = 0;
        }

    }
}
