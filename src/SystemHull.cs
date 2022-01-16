using System;
using System.Collections.Generic;

public abstract class SystemModule
{
	public SystemHull Hull{ get; internal set; }
	public uint ModuleID{ get; internal set; }
	
	public abstract void OnCreate(LoopOrder loop_order);
	public abstract void OnBegin();
	public abstract void OnEnd();
	public abstract void OnDispose();
}

public abstract class SystemModuleForWindow : SystemModule
{
	public abstract int Width();
	public abstract int Height();
	public abstract void Show();
	public abstract bool IsCreated();
	public abstract void Exit();
}

public abstract class SystemModuleForTime : SystemModule
{
	public abstract float Delta();
	public abstract int DeltaMs();
	public abstract int Fps();
	public abstract float Global();
	
	public abstract string StrDelta();
	public abstract string StrDeltaMs();
	public abstract string StrFps();
	public abstract string StrGlobal();
}

public abstract class SystemModuleForRender : SystemModule
{
	public abstract Resource.IPolygon CreatePolygon(params Vector2[] args);
	public abstract Resource.ICircle CraeteCircle(float radius);
	public abstract Resource.IColor CreateColor(byte a, byte r, byte g, byte b);
	
	public abstract void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale);
	public abstract void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale, float angle);
	public abstract void RenderColorCircle(Resource.ICircle c, Resource.IColor color, Vector2 pos, Vector2 scale);
}

public class SystemHull
{
	public readonly SystemModuleForWindow Window;
	public readonly SystemModuleForTime Time;
	public readonly SystemModuleForRender Render;
	
	public readonly List<SystemModule> Modules;
	
	public event VoidMethod Loop;
	
	public SystemHull(SystemModuleForWindow window, SystemModuleForTime time, SystemModuleForRender render, params SystemModule[] args)
	{
		uint give_id = 0;
		LoopOrder loop_order = new LoopOrder();
		Modules = new List<SystemModule>();
		
		Window = window;
		Time = time;
		Render = render;
		
		Modules.Add(Window);
		Modules.Add(Time);
		Modules.Add(Render);
		Modules.AddRange(args);
		
		//	Link & Set ID
		foreach(SystemModule m in Modules)
		{
			m.Hull = this;
			m.ModuleID = give_id++;
		}
		
		//	OnCreate
		for(int i=0; i<Modules.Count; i++)
			Modules[i].OnCreate(loop_order);
		
		//	Set Loop
		loop_order.InputTo(ref this.Loop);
		loop_order = null;
	}
	
	public void Exit()
	{
		this.Loop = null;
	}
	
	public void Run()
	{
		//	Begin
		for(int i=0; i<Modules.Count; i++)
			Modules[i].OnBegin();
		
		//	Run Start <<===
		Window.Show();
		while(Window.IsCreated() && this.Loop != null)
			this.Loop();
		//	Run Finish <<===
		
		//	End
		for(int i=Modules.Count-1; i>=0; i--)
			Modules[i].OnEnd();
		
		//	Dispose
		for(int i=Modules.Count-1; i>=0; i--)
			Modules[i].OnDispose();
		
		Window.Exit();
		Modules.Clear();
		
		System.Diagnostics.Process.GetCurrentProcess().Kill();
	}
}

public class LoopOrder
{
	private class Node : IComparable<Node>
	{
		public VoidMethod method;
		public int order;
		
		public Node(VoidMethod m, int o){ method = m; order = o; }
		
		public int CompareTo(Node other)
		{
			if (other == null)
				return 1;
			else
				return this.order.CompareTo(other.order);
		}
	}
	
	private List<Node> nodes;
	
	public LoopOrder()
	{
		nodes = new List<Node>();
	}
	
	public void Add(VoidMethod method, int order)
	{
		nodes.Add(new Node(method,order));
	}
	
	public void InputTo(ref VoidMethod method)
	{
		nodes.Sort();
		for(int i=0; i<nodes.Count; i++)
		{
			method += nodes[i].method;
		}
	}
}
