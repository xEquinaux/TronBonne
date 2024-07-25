namespace TronBonne
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string directory = "";
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
			if (Directory.Exists(directory))
			{
				Plugin.LoadPlugins(directory);
			}
			else goto BEGIN;
			CONTINUE:
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