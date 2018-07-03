
#region  Using
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient
{

    #region  Classes 
    public class ItemToolTip : ToolTip
    {
        #region  Fields 
        private Texture2D image = null; // Image we show in tooltip   
        #endregion

        #region  Properties 
        // Visible property should be always overriden in this manner.
        // You can set width and height of the tooltip according to it's content.    
        public override bool Visible
        {
            set
            {
                base.Visible = value;
                if (image == null)
                {
                    Vector2 size = Skin.Layers[0].Text.Font.Resource.MeasureString(Text);
                    Width = (int)size.X + Skin.Layers[0].ContentMargins.Horizontal + 16;
                    Height = (int)size.Y + Skin.Layers[0].ContentMargins.Vertical;

                    if (Height <  Skin.Layers[0].ContentMargins.Vertical + 8) Height = Skin.Layers[0].ContentMargins.Vertical + 8;
                }
                else
                {

                    Vector2 descSize = Manager.Skin.Fonts["Default9"].Resource.MeasureString(Description);
                    Vector2 textSize = Manager.Skin.Fonts["Default12"].Resource.MeasureString(Text);
                    Vector2 statsSize = Manager.Skin.Fonts["Default8"].Resource.MeasureString(Stats);
                    Vector2 size = descSize.X > textSize.X ? descSize : textSize;
                    size = size.X > statsSize.X ? size : statsSize;
                    Width = (int)size.X + Skin.Layers[0].ContentMargins.Horizontal + image.Width + 16;
                    Height = Skin.Layers[0].ContentMargins.Vertical;
                    Height += (int)(Manager.Skin.Fonts["Default12"].Resource.MeasureString(Text).Y + Manager.Skin.Fonts["Default9"].Resource.MeasureString(Description).Y + Manager.Skin.Fonts["Default8"].Resource.MeasureString(Stats).Y) + 4;

                    if (Height < image.Height + Skin.Layers[0].ContentMargins.Vertical + 8) Height = image.Height + Skin.Layers[0].ContentMargins.Vertical + 8;
                }
            }
        }
       
        public Texture2D Image
        {
            get { return image; }
            set { image = value; }
        }
        public string Description { get; set; }
        public string Stats { get; set; }
        #endregion

        #region  Construstors
               
        // Standard constructor
        public ItemToolTip(Manager manager)
            : base(manager)
        {
        }
        #endregion

        #region  Methods 
        public override void Init()
        {
            base.Init();
        }

        protected override void InitSkin()
        {
            base.InitSkin();

            // We specify what skin this control uses. We use standard tooltip background here.
            Skin = new SkinControl(Manager.Skin.Controls["ToolTip"]);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            SkinLayer l = Skin.Layers[0];

            // We render background of the tooltip
            renderer.DrawLayer(this, l, rect);

            Rectangle rc1 = Rectangle.Empty;
            if (image != null)
            {
                // Now we draw image in the left top corner of the tooltip
                rc1 = new Rectangle(l.ContentMargins.Left, l.ContentMargins.Top + 4, image.Width, image.Height);
                renderer.Draw(image, rc1, Color.White);
            }

            // Text is rendered next to the image
            rect = new Rectangle(rc1.Right + 10, rect.Top + 4, rect.Width, rect.Height);
            int startY = rect.Y;
            
            renderer.DrawString(Manager.Skin.Fonts["Default12"].Resource, Text, rect, Color.Black, Alignment.TopLeft, true);
            rect.Y += (int)Manager.Skin.Fonts["Default12"].Resource.MeasureString(Text).Y;
            renderer.DrawString(Manager.Skin.Fonts["Default9"].Resource, Description, rect, new Color(40,40,40), Alignment.TopLeft, true);
            rect.Y += (int)Manager.Skin.Fonts["Default9"].Resource.MeasureString(Description).Y;
            renderer.DrawString(Manager.Skin.Fonts["Default8"].Resource, Stats, rect, new Color(70,70,70), Alignment.TopLeft, true);
			rect.Y += (int)Manager.Skin.Fonts["Default8"].Resource.MeasureString(Stats).Y;
			renderer.SpriteBatch.DrawLine(new Vector2(rc1.Right + 6, startY + 2), new Vector2(rc1.Right + 6, rect.Y), Color.Black);
        }
        #endregion
    }
    #endregion
}
