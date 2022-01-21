using System;
using System.Collections.Generic;

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
		public abstract Resource.IPolygon CreatePolygon(params Vector2[] args);
		public abstract Resource.ICircle CraeteCircle(float radius);
		public abstract Resource.IColor CreateColor(byte a, byte r, byte g, byte b);
		
		public abstract void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale);
		public abstract void RenderColorPolygon(Resource.IPolygon p, Resource.IColor color, Vector2 pos, Vector2 scale, float angle);
		public abstract void RenderColorCircle(Resource.ICircle c, Resource.IColor color, Vector2 pos, Vector2 scale);
	}
}

public class SystemHull
{
	public readonly Module.WindowBase Window;
	public readonly Module.TimeBase Time;
	public readonly Module.RenderBase Render;
	
	public readonly List<Module.Base> Modules;
	
	public event VoidMethod Loop;
	/*
	0 = Time.Begin
	5 => Do Frame Begin
	15 => Do Update Before Input
	20 (temporary) = Window.Application.DoEvents
	30 => Do Input Update
	40 => Do Update After Input
	50 = Render.BeforeRender
	65 => Do Render
	80 = Render.AfterRender
	85 => Do Update After Render
	90 => Do Frame End
	99 = Time.End
	*/
	
	public SystemHull
	(
		Module.WindowBase window,
		Module.TimeBase time,
		Module.RenderBase render,
		params Module.Base[] args
	)
	{
		uint give_id = 0;
		LoopOrder loop_order = new LoopOrder();
		Modules = new List<Module.Base>();
		
		Window = window;
		Time = time;
		Render = render;
		
		Modules.Add(Window);
		Modules.Add(Time);
		Modules.Add(Render);
		Modules.AddRange(args);
		
		//	Link & Set ID
		foreach(Module.Base m in Modules)
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
