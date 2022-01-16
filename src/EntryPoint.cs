using System;

public delegate void VoidMethod();

internal class EntryPoint
{
	[STAThread]
	private static void Main()
	{
		SystemHull hull = new SystemHull
		(
			new Module.FormModule( Module.FormModule.Desc.Default ),
			new Module.TimeModule( 1000 ),
			new Module.GDIRenderModule()
		);
		hull.Run();
		
		Console.ReadKey();
	}
}

