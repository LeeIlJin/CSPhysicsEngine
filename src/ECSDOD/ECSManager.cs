using System;
using System.Collections.Generic;

namespace ECS
{
	//	============================================================================
	//	Archetype
	//	============================================================================
	public class Archetype
	{
		public Archetype(params Type[] args)
		{
			Types = args;
		}
		public Type[] Types;
	}
	
	public class SharedArchetype
	{
		public SharedArchetype(params Type[] args)
		{
			Types = args;
			Indices = null;
		}
		public void Reset()
		{
			Indices = null;
		}
		
		public int[] Indices;
		public Type[] Types;
	}

	//	============================================================================
	//	ECSManager
	//	============================================================================
	public class Manager : IDispose
	{
		private SystemBase[] systems;
		private ComponentDataArrayGroup cdaGroup;
		
		public ComponentDataArrayGroup CDAGroup { get{ return cdaGroup; } }
		
		public Manager(Factory factory)
		{
			systems = factory.systems;
			cdaGroup = new ComponentDataArrayGroup();
			
			foreach(KeyValuePair<Type,IComponentDataArraySource> kvp in factory.cdaSources)
			{
				cdaGroup.AddArray(kvp.Value);
			}
			
			int i=0;
			foreach(SystemBase system in systems)
			{
				system.Reset(cdaGroup,factory.eSources[i]);
				system.ArraySet();
				system.OnCreate();
				i++;
			}
		}
		
		public void Begin()
		{
			foreach(SystemBase system in systems)
			{
				system.OnBegin();
			}
		}
		
		public void Activate()
		{
			foreach(SystemBase system in systems)
			{
				system.OnEnable();
			}
		}
		
		public void Sleep()
		{
			foreach(SystemBase system in systems)
			{
				system.OnDisable();
			}
		}
		
		public void End()
		{
			foreach(SystemBase system in systems)
			{
				system.OnEnd();
			}
		}
		
		public void Dispose()
		{
			foreach(SystemBase system in systems)
			{
				system.OnDispose();
				system.ArrayDispose();
			}
			cdaGroup.Dispose();
		}
	}
	
	//	============================================================================
	//	ECSFactory
	//	============================================================================
	public class Factory : IDispose
	{
		public SystemBase[] systems;
		public IDictionary<Type,IComponentDataArraySource> cdaSources;
		public IDictionary<Type,IComponentData> cModels;
		public EntitiesSource[] eSources;
		
		public IDictionary<Type,Type[]> togetherComponents;
		public IDictionary<Type,Type[]> notifyComponents;
		
		public Factory()
		{
			cdaSources = new Dictionary<Type,IComponentDataArraySource>();
			cModels = new Dictionary<Type,IComponentData>();
			togetherComponents = new Dictionary<Type,Type[]>();
			notifyComponents = new Dictionary<Type,Type[]>();
		}
		
		public void Dispose()
		{
			systems = null;
			if(cdaSources != null) {
				foreach(KeyValuePair<Type,IComponentDataArraySource> kvp in cdaSources)
				{
					kvp.Value.Dispose();
				}
				cdaSources.Clear();
				cdaSources = null;
			}
			
			if(cModels != null)
			{
				cModels.Clear();
				cModels = null;
			}
			
			if(togetherComponents != null)
			{
				togetherComponents.Clear();
				togetherComponents = null;
			}
			
			if(notifyComponents != null)
			{
				notifyComponents.Clear();
				notifyComponents = null;
			}
			
			if(eSources != null)
			{
				foreach(EntitiesSource es in eSources)
				{
					if(es != null) es.Dispose();
				}
				eSources = null;
			}
		}
		
		public void First_AddSystems(params SystemBase[] args)
		{
			if(systems == null)
			{
				systems = args;
				eSources = new EntitiesSource[systems.Length];
				
				for(int i=0; i<systems.Length; i++)
				{
					eSources[i] = new EntitiesSource(systems[i].GetTypeCount());
					systems[i].RegistComponentTypeToFactory(this);
				}
			}
			else
			{
				List<EntitiesSource> elist = new List<EntitiesSource>();
				elist.AddRange(eSources);
				
				for(int i=0; i<args.Length; i++)
				{
					elist.Add(new EntitiesSource(args[i].GetTypeCount()));
					args[i].RegistComponentTypeToFactory(this);
				}
				
				List<SystemBase> slist = new List<SystemBase>();
				slist.AddRange(systems);
				slist.AddRange(args);
				
				systems = slist.ToArray();
				eSources = elist.ToArray();
				
				slist = null;
				elist = null;
			}
		}
		
		public void Second_RegistComponentType<T>() where T : struct, IComponentData
		{
			if(cdaSources.ContainsKey(typeof(T)) == false)
				cdaSources.Add(typeof(T), new ComponentDataArraySource<T>());
		}
		
		public void SetComponentFriends(Type own, params Type[] friends)
		{
			if(togetherComponents.ContainsKey(own))
				togetherComponents[own] = friends;
			else
				togetherComponents.Add(own,friends);
		}
		
		public void SetNotifyComponent(Type own, params Type[] reporters)
		{
			if(notifyComponents.ContainsKey(own))
				notifyComponents[own] = reporters;
			else
				notifyComponents.Add(own,reporters);
		}
		
		public void SetComponentModel(Type type, IComponentData data)
		{
			if(cModels.ContainsKey(type))
				cModels[type] = data;
			else
				cModels.Add(type,data);
		}
		
		public void SetComponentModels(Archetype at, params IComponentData[] datas)
		{
			for(int i=0; i<datas.Length; i++) 
			{
				SetComponentModel(at.Types[i],datas[i]);
			}
		}
		
		public void SetComponentModels(SharedArchetype sat, params IComponentData[] datas)
		{
			for(int i=0; i<datas.Length; i++)
			{
				SetComponentModel(sat.Types[i],datas[i]);
			}
		}
		
		public Entity CreateEntity(Archetype at)
		{
			return CreateComponentData_AddToSys(at, null);
		}
		
		public Entity CreateEntity(SharedArchetype sat)
		{
			return CreateComponentData_AddToSys(null, sat);
		}
		
		public Entity CreateEntity(Archetype at, SharedArchetype sat)
		{
			return CreateComponentData_AddToSys(at, sat);
		}
		
		private Entity CreateComponentData_AddToSys(Archetype at, SharedArchetype sat)
		{
			int totalCount = 0;
			if(at != null)
			{
				for(int i=0; i<at.Types.Length; i++)
				{
					if(togetherComponents.ContainsKey(at.Types[i]) == true)
					{
						List<Type> no_exist = new List<Type>();
						for(int j=0; j<togetherComponents[at.Types[i]].Length; j++)
						{
							bool is_exist = false;
							for(int k=0; k<at.Types.Length; k++)
							{
								if(at.Types[k] == togetherComponents[at.Types[i]][j])
								{
									is_exist = true;
								}
							}
							if(is_exist == false)
								no_exist.Add(togetherComponents[at.Types[i]][j]);
						}
						
						if(no_exist.Count == 0)
							continue;
						
						List<Type> temp = new List<Type>();
						temp.AddRange(at.Types);
						temp.AddRange(no_exist.ToArray());
						at.Types = temp.ToArray();
						temp = null;
						no_exist = null;
					}
				}
				
				totalCount += at.Types.Length;
			}
				
			if(sat != null)
			{
				for(int i=0; i<sat.Types.Length; i++)
				{
					if(togetherComponents.ContainsKey(sat.Types[i]) == true)
					{
						List<Type> no_exist = new List<Type>();
						for(int j=0; j<togetherComponents[sat.Types[i]].Length; j++)
						{
							bool is_exist = false;
							for(int k=0; k<sat.Types.Length; k++)
							{
								if(sat.Types[k] == togetherComponents[sat.Types[i]][j])
								{
									is_exist = true;
								}
							}
							if(is_exist == false)
								no_exist.Add(togetherComponents[sat.Types[i]][j]);
						}
						
						if(no_exist.Count == 0)
							continue;
						
						List<Type> temp = new List<Type>();
						temp.AddRange(sat.Types);
						temp.AddRange(no_exist.ToArray());
						sat.Types = temp.ToArray();
						temp = null;
						no_exist = null;
					}
				}
				
				totalCount += sat.Types.Length;
			}
			
			if(totalCount == 0)
				return null;
			
			
			
			
			Type[] totalTypes = new Type[totalCount];
			int[] totalIndex = new int[totalCount];
			
			int counter = 0;
			
			if(at != null)
			{
				foreach(Type type in at.Types)
				{
					if(cModels.ContainsKey(type))
					{
						totalIndex[counter] = cdaSources[type].CreateData(cModels[type]);
					}
					else
						totalIndex[counter] = cdaSources[type].CreateData(null);
					
					if(notifyComponents.ContainsKey(type))
					{
						for(int i=0; i<notifyComponents[type].Length; i++)
						{
							for(int j=0; j<at.Types.Length; j++)
							{
								if(notifyComponents[type][i] == at.Types[j])
								{
									cdaSources[type].NotifyToComponent(totalIndex[counter]);
								}
							}
						}
					}
					
					
					totalTypes[counter] = type;
					counter++;
				}
			}
			
			if(sat != null)
			{
				bool isFirst = false;
				if(sat.Indices == null)
				{
					sat.Indices = new int[sat.Types.Length];
					isFirst = true;
				}
				int t = 0;
				foreach(Type type in sat.Types)
				{
					if(isFirst == true)
					{
						if(cModels.ContainsKey(type))
						{
							sat.Indices[t] = cdaSources[type].CreateData(cModels[type]);
						}
						else
							sat.Indices[t] = cdaSources[type].CreateData(null);
					}
					
					totalIndex[counter] = sat.Indices[t];
					
					if(notifyComponents.ContainsKey(type))
					{
						for(int i=0; i<notifyComponents[type].Length; i++)
						{
							for(int j=0; j<sat.Types.Length; j++)
							{
								if(notifyComponents[type][i] == sat.Types[j])
								{
									cdaSources[type].NotifyToComponent(totalIndex[counter]);
								}
							}
						}
					}
					
					totalTypes[counter] = type;
					counter++;
					t++;
				}
			}
			
			counter = 0;
			foreach(SystemBase system in systems)
			{
				int[] gotIndex = system.GetTypeIndices(totalTypes);
				if(gotIndex == null)
				{
					counter++;
					continue;
				}
				
				for(int i=0; i<gotIndex.Length; i++)
					eSources[counter].Add(i,totalIndex[gotIndex[i]]);
				
				counter++;
			}
			
			Entity result = new Entity(totalIndex,totalTypes);
			
			return result;
		}
	}
}