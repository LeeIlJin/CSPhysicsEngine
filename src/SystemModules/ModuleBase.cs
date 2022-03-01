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
	
	public abstract class WindowBase : Module.Base
	{
		public abstract int Width();
		public abstract int Height();
		public abstract void Show();
		public abstract bool IsCreated();
		public abstract void Exit();
	}
	
	public abstract class TimeBase : Module.Base
	{
		public abstract float Delta();
		public abstract int DeltaMs();
		public abstract int Fps();
		public abstract float Global();
	}
	
	public abstract class RenderBase : Module.Base
	{
		public abstract Resource.IColorPolygon CreateColorPolygon(byte a, byte r, byte g, byte b, params Vector2[] args);
		public abstract Resource.IColorCircle CreateColorCircle(byte a, byte r, byte g, byte b, float radius);
	}
}