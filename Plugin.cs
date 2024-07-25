using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using twitchbot.api;
using twitchbot;

namespace TronBonne
{
	public class Plugin
	{
		public static IList<ChatInterface> Interface = new List<ChatInterface>();
		public static void LoadPlugins(string directory, string ext = "*.dll", bool reload = false)
		{
			try
			{
				foreach (string item in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, directory), ext, SearchOption.TopDirectoryOnly))
				{
					Type[] array = Assembly.LoadFile(item)?.GetExportedTypes();
					foreach (Type type in array)
					{
						if (!type.IsSubclassOf(typeof(ChatInterface)) || !type.IsPublic || type.IsAbstract)
						{
							continue;
						}
						object[] customAttributes = type.GetCustomAttributes(typeof(ApiVersion), inherit: true);
						if (customAttributes.Length != 0 && ((ApiVersion)customAttributes[0]).Match(new ApiVersion(0, 1)))
						{
							ChatInterface pluginInstance = (ChatInterface)Activator.CreateInstance(type);
							if (reload)
							{
								pluginInstance.Initialize();
							}
							Register(pluginInstance);
							//Console.WriteLine("Loaded: " + pluginInstance.Name + " " + pluginInstance.Version.ToString());
						}
					}
				}
			}
			catch (Exception e)
			{
				LogErrors(e, TimeSpan.FromSeconds(5.0));
			}
		}

		public static void UnloadPlugins(string directory, List<ChatInterface> list)
		{
			foreach (ChatInterface bot in list)
			{
				try
				{
					bot?.Dispose();
					Console.WriteLine("Unloaded: " + bot.Name + " " + bot.Version.ToString());
				}
				catch (Exception e)
				{
					LogErrors(e, TimeSpan.FromSeconds(5.0));
				}
			}
			list.Clear();
		}

		protected static void Register(ChatInterface bot)
		{
			if (!Interface.Contains(bot))
			{
				Interface.Add(bot);
			}
		}

		public static void ConsoleLog(string message, ConsoleColor color = ConsoleColor.Green, TimeSpan timeout = default(TimeSpan))
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			if (timeout != default(TimeSpan))
			{
				Thread.Sleep(timeout);
			}
		}

		public static void LogErrors(string message, ConsoleColor color, TimeSpan timeout)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Thread.Sleep(timeout);
		}

		public static void LogErrors(Exception e, TimeSpan timeout)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ForegroundColor = ConsoleColor.Gray;
			Thread.Sleep(timeout);
		}

		public static void LogErrors(Exception e, bool wait)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ForegroundColor = ConsoleColor.Gray;
			if (wait)
			{
				Console.WriteLine("Press any key to continue . . .");
				Console.ReadKey();
			}
		}

		public static void LogErrors(string message, bool wait)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			if (wait)
			{
				Console.WriteLine("Press any key to continue . . .");
				Console.ReadKey();
			}
		}
	}
}
