using System;
using System.Collections.Generic;

public class SystemHull
{
	public readonly Module.Window Window;
	public readonly Module.Draw Draw;
	public readonly Module.Input Input;
	
	public readonly Module.TimeBase Time;
	
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
		Module.Window.Desc window_desc,
		Module.TimeBase time,
		params Module.Base[] args
	)
	{
		uint give_id = 0;
		LoopOrder loop_order = new LoopOrder();
		Modules = new List<Module.Base>();
		
		this.Window = new Module.Window(window_desc);
		this.Draw = new Module.Draw();
		this.Input = new Module.Input();
		
		this.Time = time;
		
		this.Window.Initialize(loop_order);
		this.Draw.Initialize(loop_order, this.Window.Form);
		this.Input.Initialize(loop_order, this.Window.Form, this.Time);
		
		Modules.Add(Time);
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
		
		Input.Dispose();
		Draw.Dispose();
		
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
