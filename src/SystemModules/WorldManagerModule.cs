using System;
using System.Collections.Generic;

namespace Module
{
	public sealed class WorldManager : Module.Base
	{
		public override void OnCreate(LoopOrder loop_order)
		{
			
		}
		public override void OnBegin()
		{
			
		}
		public override void OnEnd()
		{
			
		}
		public override void OnDispose()
		{
			
		}
		
		public abstract class World
		{
			public WorldManager WorldManager{ get; internal set; }
			
			public ECS.Manager ECSManager{get; protected set;}
			
			public abstract void OnEnable();
			public abstract void OnDisable();
			
			public abstract void OnCreate();
			public abstract void OnBegin();
			public abstract void OnEnd();
			public abstract void OnDispose();
			
			public abstract void OnFrameBegin(float delta);
			public abstract void OnUpdateBeforeInput(float delta);
			public abstract void OnUpdateAfterInput(float delta);
			// TODO: 루프중 업데이트 함수 채워 넣기
		}
	}
}