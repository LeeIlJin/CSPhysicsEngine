using System;

namespace Module
{
	public sealed class TickTime : Module.TimeBase
	{
		//	Field ===============================================================================
		private TickTimer frame_timer;
		
		private int fps;
		
		private int fps_check_ms;
		private int frame_count;
		
		private int frame_time_ms;
		private float global_time_s;
		
		public TickTime(int ms)
		{
			frame_timer = new TickTimer();
			
			fps = 0;
			fps_check_ms = ms;
			frame_count = 0;
			frame_time_ms = 0;
			global_time_s = 0.0f;
		}
		
		private void Begin()
		{
			frame_timer.Begin();
		}
		
		private void End()
		{
			frame_timer.End();
			
			frame_time_ms += frame_timer.tick_ms;
			global_time_s += frame_timer.tick_s;
			frame_count++;
			
			if(frame_time_ms >= fps_check_ms)
			{
				fps = (int)((float)frame_count / frame_timer.tick_s);
				frame_time_ms = 0;
				frame_count = 0;
			}
		}
		
		//	Base's ==============================================================================
		public override void OnCreate(LoopOrder loop_order)
		{
			loop_order.Add(this.Begin,0);
			loop_order.Add(this.End,99);
		}
		public override void OnBegin(){}
		public override void OnEnd(){}
		public override void OnDispose()
		{
			frame_timer = null;
		}
		
		//	Time's ==============================================================================
		public override float Delta(){ return frame_timer.tick_s; }
		public override int DeltaMs(){ return frame_timer.tick_ms; }
		public override int Fps(){ return fps; }
		public override float Global(){ return global_time_s; }
		
	}
}