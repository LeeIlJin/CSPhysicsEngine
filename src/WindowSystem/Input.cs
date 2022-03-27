using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Module
{
	public sealed class Input
	{
		private IDictionary<Keys,KeyNode> keyNodes;
		private IDictionary<MouseButtons,KeyNode> mouseKeyNodes;
		
		private Vector2 mousePosition;
		private Vector2 mousePriviousPosition;
		private Vector2 mouseVector;
		private bool isMouseMoving;
		
		private Module.TimeBase timer;
		
		public bool IsKeyPress(Keys code) {return this.keyNodes[code]?.press ?? false;}
		public bool IsKeyDown(Keys code) {return this.keyNodes[code]?.down ?? false;}
		public bool IsKeyUp(Keys code) {return this.keyNodes[code]?.up ?? false;}
		public float KeyPressElapsed(Keys code) {return this.keyNodes[code]?.elapsed ?? 0.0f;}
		
		public bool IsMousePress(MouseButtons code) {return this.mouseKeyNodes[code]?.press ?? false;}
		public bool IsMouseDown(MouseButtons code) {return this.mouseKeyNodes[code]?.down ?? false;}
		public bool IsMouseUp(MouseButtons code) {return this.mouseKeyNodes[code]?.up ?? false;}
		public float MousePressElapsed(MouseButtons code) {return this.mouseKeyNodes[code]?.elapsed ?? 0.0f;}
		
		public Vector2 MousePosition {get{return this.mousePosition;}}
		public Vector2 MousePriviousPosition {get{return this.mousePriviousPosition;}}
		public Vector2 MouseVector {get{return this.mouseVector;}}
		public bool IsMouseMoving() {return this.isMouseMoving;}
		
		public Input()
		{
			keyNodes = new Dictionary<Keys,KeyNode>();
			mouseKeyNodes = new Dictionary<MouseButtons,KeyNode>();
			
			mouseKeyNodes.Add(MouseButtons.Left,new KeyNode());
			mouseKeyNodes.Add(MouseButtons.Right,new KeyNode());
			mouseKeyNodes.Add(MouseButtons.Middle,new KeyNode());
			
			mousePosition = Vector2.Zero;
			mousePriviousPosition = Vector2.Zero;
			mouseVector = Vector2.Zero;
			isMouseMoving = false;
		}
		
		public void Initialize(LoopOrder loop_order, Form form, Module.TimeBase timer_)
		{
			form.KeyDown += OnKeyDown;
			form.KeyUp += OnKeyUp;
			form.MouseDown += OnMouseDown;
			form.MouseUp += OnMouseUp;
			form.MouseMove += OnMouseMove;
			
			timer = timer_;
			
			loop_order.Add(this.BeforeApplicationEvent,19);
		}
		
		public void Dispose()
		{
			keyNodes.Clear();
			mouseKeyNodes.Clear();
		}
		
		public void AddKey(Keys code)
		{
			if(this.keyNodes.ContainsKey(code) == false)
				this.keyNodes.Add(code,new KeyNode());
		}
		
		public void AddKeys(params Keys[] args)
		{
			foreach(Keys code in args) {
				if(this.keyNodes.ContainsKey(code) == false)
					this.keyNodes.Add(code,new KeyNode());
			}
		}
		
		
		//	KEY
		private void OnKeyDown(object o, KeyEventArgs e)
		{
			if(keyNodes.ContainsKey(e.KeyCode) == false)
				return;
			keyNodes[e.KeyCode].elapsed = 0.0f;
			keyNodes[e.KeyCode].down = true;
			keyNodes[e.KeyCode].press = true;
		}
		
		private void OnKeyUp(object o, KeyEventArgs e)
		{
			if(keyNodes.ContainsKey(e.KeyCode) == false)
				return;
			keyNodes[e.KeyCode].up = true;
			keyNodes[e.KeyCode].press = false;
		}
		
		//	MOUSE
		private void OnMouseDown(object o, MouseEventArgs e)
		{
			if(mouseKeyNodes.ContainsKey(e.Button) == false)
				return;
			mouseKeyNodes[e.Button].elapsed = 0.0f;
			mouseKeyNodes[e.Button].down = true;
			mouseKeyNodes[e.Button].press = true;
		}
		
		private void OnMouseUp(object o, MouseEventArgs e)
		{
			if(mouseKeyNodes.ContainsKey(e.Button) == false)
				return;
			mouseKeyNodes[e.Button].up = true;
			mouseKeyNodes[e.Button].press = false;
		}
		
		private void OnMouseMove(object o, MouseEventArgs e)
		{
			mousePriviousPosition = mousePosition;
			mousePosition = new Vector2(e.X,e.Y);
			mouseVector = mousePosition - mousePriviousPosition;
			isMouseMoving = true;
		}
		
		
		//	On Frame
		private void BeforeApplicationEvent()
		{
			this.isMouseMoving = false;
			
			foreach(KeyValuePair<Keys,KeyNode> kvp in this.keyNodes)
			{
				if(kvp.Value.press)
					kvp.Value.elapsed += timer.Delta();
				if(kvp.Value.down)
					kvp.Value.down = false;
				if(kvp.Value.up)
					kvp.Value.up = false;
			}
			
			foreach(KeyValuePair<MouseButtons,KeyNode> kvp in this.mouseKeyNodes)
			{
				if(kvp.Value.press)
					kvp.Value.elapsed += timer.Delta();
				if(kvp.Value.down)
					kvp.Value.down = false;
				if(kvp.Value.up)
					kvp.Value.up = false;
			}
		}
		
		private class KeyNode
		{
			public bool press;
			public bool down;
			public bool up;
			public float elapsed;
			
			public KeyNode()
			{
				press = false;
				down = false;
				up = false;
				elapsed = 0.0f;
			}
		}
	}
}