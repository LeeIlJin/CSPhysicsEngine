using System;

public delegate void VoidMethod();

internal class EntryPoint
{
	[STAThread]
	private static void Main()
	{
		SystemHull hull = new SystemHull
		(
			new Module.WindowForm( Module.WindowForm.Desc.Default ),
			new Module.TickTime( 1000 ),
			new Module.GDIRender(),
			new Module.WorldManager
			(
				new Game.TestWorld()
			)
		);
		hull.Run();
		
		Console.ReadKey();
	}
}

