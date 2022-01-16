using System;

public class TickTimer
{
	public int pre_tick_ms;
	public int post_tick_ms;
	
	public int tick_ms;
	public float tick_s;
	
	public TickTimer()
	{
		pre_tick_ms = 0;
		post_tick_ms = 0;
		
		tick_ms = 0;
		tick_s = 0.0f;
	}
	
	public void Begin()
	{
		pre_tick_ms = Environment.TickCount;
	}
	
	public void End()
	{
		post_tick_ms = Environment.TickCount;
		tick_ms = post_tick_ms - pre_tick_ms;
		tick_s = (float)tick_ms * 0.001f;
	}
}
