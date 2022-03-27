using System;

public delegate void VoidMethod();

internal class EntryPoint
{
	[STAThread]
	private static void Main()
	{
		URandom.CreateSeed();
		
		SystemHull hull = new SystemHull
		(
			Module.Window.Desc.Default,
			new Module.TickTime( 1000 ),
			new Module.WorldManager
			(
				new Game.TestWorld()
			)
		);
		hull.Run();
		
		Console.ReadKey();
	}
}

