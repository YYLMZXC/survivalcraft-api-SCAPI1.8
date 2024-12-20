using Engine;
using Engine.Serialization;
using GameEntitySystem;
using System;
using TemplatesDatabase;

namespace Game
{
	public class ComponentCreature : Component
	{
		public SubsystemPlayerStats m_subsystemPlayerStats;

		public string[] m_killVerbs;

		public ComponentBody ComponentBody
		{
			get;
			set;
		}

		public ComponentHealth ComponentHealth
		{
			get;
			set;
		}

		public ComponentSpawn ComponentSpawn
		{
			get;
			set;
		}

		public ComponentCreatureModel ComponentCreatureModel
		{
			get;
			set;
		}

		public ComponentCreatureSounds ComponentCreatureSounds
		{
			get;
			set;
		}

		public ComponentLocomotion ComponentLocomotion
		{
			get;
			set;
		}

		public PlayerStats PlayerStats
		{
			get
			{
				var componentPlayer = this as ComponentPlayer;
				if (componentPlayer != null)
				{
					return m_subsystemPlayerStats.GetPlayerStats(componentPlayer.PlayerData.PlayerIndex);
				}
				return null;
			}
		}

		public bool ConstantSpawn
		{
			get;
			set;
		}

		public CreatureCategory Category
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public ReadOnlyList<string> KillVerbs => new(m_killVerbs);

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			ComponentBody = Entity.FindComponent<ComponentBody>(throwOnError: true);
			ComponentHealth = Entity.FindComponent<ComponentHealth>(throwOnError: true);
			ComponentSpawn = Entity.FindComponent<ComponentSpawn>(throwOnError: true);
			ComponentCreatureSounds = Entity.FindComponent<ComponentCreatureSounds>(throwOnError: true);
			ComponentCreatureModel = Entity.FindComponent<ComponentCreatureModel>(throwOnError: true);
			ComponentLocomotion = Entity.FindComponent<ComponentLocomotion>(throwOnError: true);
			m_subsystemPlayerStats = Project.FindSubsystem<SubsystemPlayerStats>(throwOnError: true);
			ConstantSpawn = valuesDictionary.GetValue<bool>("ConstantSpawn");
			Category = valuesDictionary.GetValue<CreatureCategory>("Category");
			DisplayName = valuesDictionary.GetValue<string>("DisplayName");
			if (DisplayName.StartsWith("[") && DisplayName.EndsWith("]"))
			{
				string[] lp = DisplayName.Substring(1, DisplayName.Length - 2).Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
				DisplayName = LanguageControl.GetDatabase("DisplayName", lp[1]);
			}
			m_killVerbs = HumanReadableConverter.ValuesListFromString<string>(',', valuesDictionary.GetValue<string>("KillVerbs"));
			if (m_killVerbs.Length == 0)
			{
				throw new InvalidOperationException("Must have at least one KillVerb");
			}
			if (!MathUtils.IsPowerOf2((long)Category))
			{
				throw new InvalidOperationException("A single category must be assigned for creature.");
			}
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			valuesDictionary.SetValue("ConstantSpawn", ConstantSpawn);
		}
	}
}
