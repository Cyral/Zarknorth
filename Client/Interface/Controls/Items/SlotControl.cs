using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class SlotControl : Control
    {
        public Label label;
        public ImageBox image;
        public Button button;
        public int ID;
        public bool Mini;

        public SlotControl(Manager manager, bool mini =false)
            : base(manager)
        {
            Mini = mini;
            //CanFocus = true;
            Passive = true;
            button = new Button(manager);
            button.Init();
            button.Height = button.Width = Mini ? 32 : 48;
            button.Text = "";
            Add(button);

            image = new ImageBox(manager);
            image.Init();
            image.Height = Tile.Height + 2;
            image.Width = Tile.Width + (Mini ? 6 : 12);
            image.Left =  Mini ? 1 : 6;
            image.Top =  Mini ? 3 : 5;
            image.Passive = true;
            image.SizeMode = SizeMode.Fit;
            image.BringToFront();
            Add(image);

            if (!Mini)
            {
                label = new Label(manager);
                label.Init();
                label.Left = 12;
                label.Width = 48;
                label.Height = 10;
                label.Top = 34;
                label.Passive = true;
                label.TextColor = Microsoft.Xna.Framework.Color.LightGray;
                label.TextChanged += label_TextChanged;
                Add(label);
            }

            // Set the tooltip class we want to constuct for this control 
            button.ToolTipType = typeof(ItemToolTip);

        }

        void label_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Label l = (Label)sender;
            l.Left = (48 / 2) - (int)(Manager.Skin.Fonts[0].Resource.MeasureString(l.Text).X / 2);
        }
        Item lastItem = Item.Blank;
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            //Check for nulls
            if (((SlotContainer)Parent).ItemSlots[ID] == null)
                ((SlotContainer)Parent).ItemSlots[ID] = new Slot(Item.Blank);
            //  base.DrawControl(renderer,rect,gameTime);
            Slot s = ((SlotContainer)Parent).ItemSlots[ID];
            image.Visible = s.Item != Item.Blank;

            if (s.Stack > 0 && s.Item != Item.Blank)
            {
                image.Image = ContentPack.Textures["items\\" + s.Item.Name];
                if (!Mini)
                label.Text = s.Stack.ToString();
                if (lastItem.ID != s.Item.ID)
                ResetTooltip(s);
            }
            else if (!Mini)
                label.Text = "";
            lastItem = s.Item;
        }
  
        private void ResetTooltip(Slot s)
        {
            StringBuilder sb = new StringBuilder();
            string[] strs = s.Item.OnPrintStats(new PrintItemDataEventArgs());
            foreach (string str in strs)
            {
                sb.Append("\n");
                sb.Append(str);
            }
            (button.ToolTip as ItemToolTip).Text = s.Item.Name;
            (button.ToolTip as ItemToolTip).Description = s.Item.Description;
            (button.ToolTip as ItemToolTip).Stats = sb.ToString();
            (button.ToolTip as ItemToolTip).Image = s.Item.Textures[0];
        }
    }
}
  
   
