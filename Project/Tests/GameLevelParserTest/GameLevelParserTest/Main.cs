using System;
using ClassesLibrary;
using System.Text;

namespace GameLevelParserTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			GameLevelParser parser = new GameLevelParser ();
			parser.LoadFromFile ("1.xpm", Encoding.UTF8);

			Console.WriteLine ( String.Format( "Try to load game level from file '{0}'... {1}", parser.FileName, parser.LastError ) );
			if (parser.IsLoaded ()) {
				Console.WriteLine (String.Format("Item at position [3,6] is '{0}'", parser.Item(3,6) ) );
			}
		}
	}
}
