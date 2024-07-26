using Microsoft.Xna.Framework.Content;

namespace TronBonne
{
	internal class Program
	{
		public static string directory = string.Empty;
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				directory = args[0];
			}
			if (Directory.Exists(directory))
			{
				goto CONTINUE;
			}
			BEGIN:
			Console.Write("What directory contains the plugins: ");
			directory = Console.ReadLine();
			CONTINUE:
			if (Directory.Exists(directory))
			{
				Plugin.LoadPlugins(directory);
			}
			else goto BEGIN;
			Plugin.Interface = Plugin.Interface.OrderBy(t => t.Priority).ToList();
			int num = Plugin.Interface.Count;
			var array = (ChatInterface[])Plugin.Interface.ToArray().Clone();
			Plugin.Interface.Clear();
			for (int i = 0; i < num; i++)
			{
				var item = array[i];
				try
				{ 
					if ((bool)item?.Load())
					{ 
						item.active = true;
						Plugin.Interface.Add(item);
						Plugin.ConsoleLog(item.UponLoadSuccessMessage);
					}
				}
				catch (Exception e)
				{
					Plugin.ConsoleLog(item.LoadNotSuccessMessage, ConsoleColor.Yellow);
					Plugin.ConsoleLog(e.ToString(), ConsoleColor.Red, TimeSpan.FromSeconds(10));
				}
			}
			array = null;
			new Game1().Run();
		}
	}
}