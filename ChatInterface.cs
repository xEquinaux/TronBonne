using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronBonne
{
	public abstract class ChatInterface
	{
		internal bool active = false;
		public virtual Version Version => new Version(0, 1);
		public virtual int Priority => 0;
		public virtual string Name => "";
		public virtual char CommandChar => '!';
		public virtual string UponLoadSuccessMessage => $"Succesfully loaded {Name} {Version}";
		public virtual string LoadNotSuccessMessage => $"{Name} plugin {Version} did not load.";
		/// <summary>
		/// Each button that is in this array will be added to the side of the main drawing 
		/// window in a list.
		/// </summary>
		public virtual UI.Button[] Button { get; set; }
		/// <summary>
		/// If this passes inspection, the plugin will be set to active and beginning 
		/// running. It is the first method that runs even before the program begins the 
		/// drawing and update loops.
		/// </summary>
		/// <returns>Whether or not to set the plugin as active.</returns>
		public abstract bool Load();
		/// <summary>
		/// Resources are loaded here after the built-in font and MagicPixel value.
		/// </summary>
		/// <returns>
		/// If this returns 'true', then the API will work on loading the buttons from the 
		/// Button array.
		/// </returns>
		public abstract bool LoadContent();
		/// <summary>
		/// This is for added a once-only run in the program's Update loop. Useful for 
		/// setting UI element resizing, element children, and other things.
		/// </summary>
		public abstract void Initialize();
		public abstract void Dispose();
		public abstract void Update();
		public abstract void Draw(SpriteBatch sb);
	}
}
