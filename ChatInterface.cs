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
		internal bool active = true;
		public virtual Version Version => new Version(0, 1);
		public virtual int Priority => 0;
		public virtual string Name => "";
		public virtual char CommandChar => '!';
		public abstract void Initialize();
		public abstract void Dispose();
		public abstract void Update();
		public abstract void Draw(SpriteBatch sb);
	}
}
