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

		public static Game1 Instance;
		public static Texture2D MagicPixel;
		public static SpriteFont Font;
		public static IList<string> messages = new List<string>() { "Beginning of chat:" };
		public static IList<Color> msgColor = new List<Color>() { Color.Black };
		public static IList<Button> apiButtons = new List<Button>();
		bool init = false;
		IrcChat chat;
		Rectangle chatBounds = new Rectangle(0, 0, 200, 300);
		ListBox chatBox;
		Scroll chatScroll;
		Button color; 
		Button textColor;
		Button borderColor;
		Button noBgButton;
		bool noBackground = false;
		Color chatBgColor = Color.Gray;
		Color chatTextColor = Color.White;
		Color chatBorderColor = Color.Black;
		Rectangle sideBar;
		int ticks = 0;
		ListBox sideBarList;
		Scroll sideBarScroll;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			//new IrcChat().ConnectAsync("", "", "");
			chatScroll = new Scroll(chatBounds);
			chatBox = new ListBox(chatBounds, chatScroll, messages.ToArray(), textColor: msgColor.ToArray());
			sideBar = new Rectangle(Point.Zero, new Point(200, Window.ClientBounds.Height));
			sideBarList = new ListBox(sideBar, sideBarScroll, apiButtons.ToArray());
			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			MagicPixel = new Texture2D(this.GraphicsDevice, 1, 1);
			MagicPixel.SetData(new byte[] { 255, 255, 255, 255 });
			Font = Content.Load<SpriteFont>("Consolas");

			var v2 = Font.MeasureString("Change background");
			color = new Button("Change background", new Rectangle(0, 0, (int)v2.X, (int)v2.Y), Color.Gray) { active = true, drawMagicPixel = true, innactiveDrawText = true };
			v2 = Font.MeasureString("Text color");
			textColor = new Button("Text color", new Rectangle(0, (int)v2.Y * 1, (int)v2.X, (int)v2.Y), Color.LightGray) { active = true, drawMagicPixel = true, innactiveDrawText = true };
			v2 = Font.MeasureString("Border color");
			borderColor = new Button("Border color", new Rectangle(0, (int)v2.Y * 2, (int)v2.X, (int)v2.Y), Color.LightGray) { active = true, drawMagicPixel = true, innactiveDrawText = true };
			v2 = Font.MeasureString("Draw background");
			noBgButton = new Button("Draw background", new Rectangle(0, (int)v2.Y * 3, (int)v2.X, (int)v2.Y), Color.LightGray) { active = true, drawMagicPixel = true, innactiveDrawText = true };
			
			Instance = this;
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

			chatBox.content = messages.ToArray();
			chatBox.textColor = msgColor.ToArray();
			chatBounds = chatBox.hitbox;
			if (!chatScroll.clicked)
			{ 
				if (!Element.Resize(ref chatBox.hitbox))
				{
					Mouse.SetCursor(MouseCursor.Arrow);
					chatBox.Update(true);
				}
				else if (chatBounds != chatBox.hitbox)
				{
					ResizeChat();
				}
			}
			chatBox.Update(false);
			var v2 = Font.MeasureString("Change background");
			color.box = new Rectangle(0, 0, (int)v2.X, (int)v2.Y);

			if (!init)
			{
				init = true;
				chatBox.hitbox.X = (int)v2.X;
				sideBarScroll = new Scroll(sideBar);
				sideBarList.scroll = sideBarScroll;
				apiButtons.Add(color);
				apiButtons.Add(textColor);
				apiButtons.Add(borderColor);
				apiButtons.Add(noBgButton);
				sideBarList.item = apiButtons.ToArray();
			}

			sideBarList.Update(false);

			if (color.LeftClick())
			{
				var item = new ColorDialog();
				item.ShowDialog();
				var c = item.Color;
				chatBgColor = new Color(c.R, c.G, c.B);
			}
			if (textColor.LeftClick())
			{
				var item = new ColorDialog();
				item.ShowDialog();
				var c = item.Color;
				chatTextColor = new Color(c.R, c.G, c.B);
			}
			if (borderColor.LeftClick())
			{
				var item = new ColorDialog();
				item.ShowDialog();
				var c = item.Color;
				chatBorderColor = new Color(c.R, c.G, c.B);
			}
			if (noBgButton.LeftClick() && ticks == 0)
			{
				noBackground = !noBackground;
				ticks++;
			}
			else if (!noBgButton.LeftClick()) ticks = 0;

			foreach (var item in Plugin.interfaces)
			{
				item.Update();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			if (!init) return;

			foreach (var item in Plugin.interfaces)
			{
				item.Draw(spriteBatch);
			}

			if (!noBackground)
			{ 
				spriteBatch.Draw(MagicPixel, chatBox.hitbox, chatBgColor);
			}
			chatBox.Draw(spriteBatch, Font, default, chatTextColor, chatBorderColor, 4, 0, 18);
			if (chatBox.hitbox.Contains(Mouse.GetState().Position))
			{
				chatScroll.Draw(spriteBatch, Color.Gray);
			}
			if (sideBar.Contains(Mouse.GetState().Position))
			{
				spriteBatch.Draw(MagicPixel, sideBar, Color.Black * 0.2f);
				sideBarList.Draw(spriteBatch, Font, default, Color.White, Color.Black, true, 2, 4, 18);
				sideBarScroll.Draw(spriteBatch, Color.Gray);
			}

			base.Draw(gameTime);
		}

		public void AddMessage(string text, Color color)
		{
			var list = TextWrapper.WrapText(Font, text, chatBox.hitbox.Width);
			foreach (var item in list)
			{
				messages.Add(item);
				msgColor.Add(color);
			}
			chatScroll.ScrollToCaret(messages.Count, (int)(chatBox.hitbox.Height / Font.MeasureString("|").Y));
		}

		public void ResizeChat()
		{
		}
	}
	public class Container
	{
		public string text;
		int ticks = 0;
		public Color color;
		public Rectangle bounds;
		public Container child;
		void Update()
		{
			ticks = 0;
		}
		public bool Click(Point mouse)
		{
			bool flag = Hover(mouse) && Mouse.GetState().LeftButton == ButtonState.Pressed && ticks++ == 0;
			Update();
			return flag;
		}
		bool Hover(Point mouse)
		{
			return bounds.Contains(mouse);
		}
		public void Draw(SpriteBatch sb)
		{
			sb.Draw(Game1.MagicPixel, bounds, color);
			if (Hover(Mouse.GetState().Position))
			{
				sb.DrawString(Game1.Font, text, new Vector2(bounds.Left, bounds.Bottom + 2), Color.White);
			}
		}
	}

	public static class TextWrapper
	{
		public static List<string> WrapText(SpriteFont spriteFont, string text, float maxLineWidth)
		{
			List<string> lines = new List<string>();
			string[] words = text.Split(' ');

			StringBuilder currentLine = new StringBuilder();
			float currentLineWidth = 0f;

			foreach (string word in words)
			{
				Vector2 size = spriteFont.MeasureString(word + " ");
				if (currentLineWidth + size.X > maxLineWidth)
				{
					if (size.X > maxLineWidth)
					{
						// Split the long word into smaller segments
						string remainingWord = word;
						while (spriteFont.MeasureString(remainingWord).X > maxLineWidth)
						{
							int splitIndex = FindSplitIndex(spriteFont, remainingWord, maxLineWidth);
							lines.Add(remainingWord.Substring(0, splitIndex));
							remainingWord = remainingWord.Substring(splitIndex);
						}
						currentLine.Append(remainingWord + " ");
						currentLineWidth = spriteFont.MeasureString(remainingWord + " ").X;
					}
					else
					{
						lines.Add(currentLine.ToString());
						currentLine.Clear();
						currentLineWidth = 0f;
						currentLine.Append(word + " ");
						currentLineWidth += size.X;
					}
				}
				else
				{
					currentLine.Append(word + " ");
					currentLineWidth += size.X;
				}
			}

			if (currentLine.Length > 0)
			{
				lines.Add(currentLine.ToString());
			}

			return lines;
		}

		private static int FindSplitIndex(SpriteFont spriteFont, string word, float maxLineWidth)
		{
			for (int i = 1; i < word.Length; i++)
			{
				if (spriteFont.MeasureString(word.Substring(0, i)).X > maxLineWidth)
				{
					return i - 1;
				}
			}
			return word.Length;
		}
	}
}
