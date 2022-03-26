using System;

namespace Module
{
	public abstract class Base
	{
		public SystemHull Hull{ get; internal set; }
		public uint ModuleID{ get; internal set; }
		
		public abstract void OnCreate(LoopOrder loop_order);
		public abstract void OnBegin();
		public abstract void OnEnd();
		public abstract void OnDispose();
	}
	
	public abstract class TimeBase : Module.Base
	{
		public abstract float Delta();
		public abstract int DeltaMs();
		public abstract int Fps();
		public abstract float Global();
	}
}