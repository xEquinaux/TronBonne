using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Text;
using ColorDialog = System.Windows.Forms.ColorDialog;
using TronBonne.UI;
using System.Windows.Media.TextFormatting;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Xps;

namespace TronBonne
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private static SpriteBatch _spriteBatch;
		public static SpriteBatch spriteBatch => _spriteBatch;


		/// <summary>
		/// Instance of the main functions class.
		/// </summary>
		public static Game1 Instance;
		public static Texture2D MagicPixel;
		public static SpriteFont Consolas;
		/// <summary>
		/// To link a plugin user interface button to add special effects and uses. Adding 
		/// a Button to this is required for some features.
		/// </summary>
		public static IList<Button> ApiButton = new List<Button>();
		bool init = false;
		Rectangle sideBar;
		int ticks = 0;
		ListBox sideBarList;
		Scroll sideBarScroll;

		public Game1()
		{
			Instance = this;
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			sideBar = new Rectangle(Point.Zero, new Point(200, Window.ClientBounds.Height));
			sideBarList = new ListBox(sideBar, sideBarScroll, ApiButton.ToArray());
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			MagicPixel = new Texture2D(this.GraphicsDevice, 1, 1);
			MagicPixel.SetData(new byte[] { 255, 255, 255, 255 });
			Consolas = Content.Load<SpriteFont>("Consolas");

			foreach (var item in Plugin.Interface)
			{
				if ((bool)item?.LoadContent())
				{ 
					if (item.Button.Length > 0)
					{
                        foreach (var item1 in item.Button)
                        {
							if (item1 != default && item1.active)
							{ 
								ApiButton.Add(item1);
							}
                        }
                    }
				}
			}
		}

		protected override bool BeginDraw()
		{
			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			return true;
		}

		protected override void EndDraw()
		{
			_spriteBatch.End();
			GraphicsDevice.Present();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (!init)
			{
				init = true;
				sideBarScroll = new Scroll(sideBar);
				sideBarList.scroll = sideBarScroll;
				foreach (var item in Plugin.Interface)
				{
					item?.Initialize();
				}
			}

			sideBarList.item = ApiButton.ToArray();
			sideBarList.Update(false);

			

			foreach (var item in Plugin.Interface)
			{
				item?.Update();
			}
			Instance = this;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			if (!init) return;

			foreach (var item in Plugin.Interface)
			{
				item?.Draw(spriteBatch);
			}

			if (sideBar.Contains(Mouse.GetState().Position))
			{
				spriteBatch.Draw(MagicPixel, sideBar, Color.Black * 0.2f);
				sideBarList.Draw(spriteBatch, Consolas, default, Color.White, Color.Black, true, 2, 4, 18);
				sideBarScroll.Draw(spriteBatch, Color.Gray);
			}

			base.Draw(gameTime);
		}
	}
}
