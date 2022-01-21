using System;
using System.Collections.Generic;

namespace Module
{
	public sealed class WorldManager : Module.Base
	{
		private List<Module.WorldNode> world_list;
		private Module.WorldNode current_world;
		private int next_world;
		
		public WorldManager(Module.WorldNode start, params Module.WorldNode[] others)
		{
			current_world = start;
			next_world = -1;
			
			world_list = new List<Module.WorldNode>();
			world_list.Add(start);
			world_list.AddRange(others);
			
			uint give_id = 0;
			foreach(Module.WorldNode w in world_list)
			{
				w.WorldID = give_id++;
			}
		}
		
		private void SwitchWorld(int world_number)
		{
			current_world.OnDisable();
			current_world = world_list[world_number];
			current_world.OnEnable();
			next_world = -1;
		}
		
		public bool ChangeWorld(int world_number)
		{
			if(world_number < 0)
				return false;
			
			if(next_world == world_number)
				return true;
			else if(next_world >= 0)
				return false;
				
			if(world_number == current_world.WorldID)
				return false;
			else if(world_number >= world_list.Count)
				return false;
			
			next_world = world_number;
			return true;
		}
		
		
		private void FrameBegin(){ if(next_world >= 0){ SwitchWorld(next_world); } current_world.OnFrameBegin(); }
		private void UpdateBeforeInput(){ current_world.OnUpdateBeforeInput(); }
		private void UpdateAfterInput(){ current_world.OnUpdateAfterInput(); }
		private void Render(){ current_world.OnRender(); }
		private void UpdateAfterRender(){ current_world.OnUpdateAfterRender(); }
		private void FrameEnd(){ current_world.OnFrameEnd(); }
		
		//	Base's ==============================================================================
		public override void OnCreate(LoopOrder loop_order)
		{
			foreach(Module.WorldNode w in world_list)
			{
				w.WorldManager = this;
				w.Window = Hull.Window;
				w.Time = Hull.Time;
				w.Render = Hull.Render;
			}
			
			foreach(Module.WorldNode w in world_list)
				w.OnCreate();
			
			loop_order.Add(this.FrameBegin,5);
			loop_order.Add(this.UpdateBeforeInput,15);
			loop_order.Add(this.UpdateAfterInput,40);
			loop_order.Add(this.Render,65);
			loop_order.Add(this.UpdateAfterRender,85);
			loop_order.Add(this.FrameEnd,90);
			
		}
		public override void OnBegin()
		{
			foreach(Module.WorldNode w in world_list)
				w.OnBegin();
			
			current_world.OnEnable();
		}
		public override void OnEnd()
		{
			current_world.OnDisable();
			
			foreach(Module.WorldNode w in world_list)
				w.OnEnd();
		}
		public override void OnDispose()
		{
			foreach(Module.WorldNode w in world_list)
				w.OnDispose();
			
			world_list.Clear();
			world_list = null;
			current_world = null;
		}
	}
	
	public abstract class WorldNode
	{
		public uint WorldID{ get; internal set; }
		public Module.WorldManager WorldManager{ get; internal set; }
		public Module.WindowBase Window{ get; internal set; }
		public Module.TimeBase Time{ get; internal set; }
		public Module.RenderBase Render{ get; internal set; }
		
		public abstract void OnEnable();
		public abstract void OnDisable();
		
		public abstract void OnCreate();
		public abstract void OnBegin();
		public abstract void OnEnd();
		public abstract void OnDispose();
		
		
		//	Loop Events
		public virtual void OnFrameBegin(){}
		
		public virtual void OnUpdateBeforeInput(){}
		public virtual void OnUpdateAfterInput(){}
		
		public virtual void OnRender(){}
		public virtual void OnUpdateAfterRender(){}
		
		public virtual void OnFrameEnd(){}
	}
}