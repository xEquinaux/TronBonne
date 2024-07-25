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
			foreach (var item in Plugin.Interface)
			{
				try
				{ 
					if ((bool)item?.Load())
					{ 
						item.active = true;
						Console.WriteLine(item.UponLoadSuccessMessage);
					}
				}
				catch
				{
					Console.WriteLine(item.LoadNotSuccessMessage);
				}
			}
			new Game1().Run();
		}
	}
}