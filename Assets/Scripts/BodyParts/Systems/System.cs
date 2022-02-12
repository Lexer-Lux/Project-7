using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

namespace Body_Parts.Systems {
	[RequireComponent(typeof(Entity)), HideMonoScript] public abstract class System : MonoBehaviour {
		private Entity Owner => gameObject.GetComponent<Entity>();

		// ReSharper disable once UnusedMember.Local
		private string Suffix => $"{Level}/{maxLevel}";
		[SuffixLabel("$Suffix", Overlay = false), SerializeField,
		 ProgressBar(0, "maxLevel", Segmented = true, ColorGetter = "Color"), OnValueChanged("updateLevel"), HideLabel]
		protected int _Level = 6;
		public int Level{
			get => _Level;
			set {
				_Level = Math.Clamp(value, 0, Owner.AttributeMax);
				eLevelChanged.Invoke();
			}
		}
		public abstract          Color      Color{ get; }
		[HideInInspector] public UnityEvent eLevelChanged;
		// ReSharper disable once MemberCanBePrivate.Global
		protected                                          int           maxLevel => Owner.AttributeMax;
		[HideLabel, FoldoutGroup("Debuff Manager")] public DebuffManager DebuffManager;

		protected void Update() { DebuffManager.Update(); }

		protected System() {
			DebuffManager = new DebuffManager();
			eLevelChanged = new(); 
		}

		public virtual void Start() { }
		protected void updateLevel() =>
			eLevelChanged.Invoke(); // * This is just for when we change the value in the Odin inspector.
	}

	[Serializable] public class DebuffManager {

		#region Vulnerability
		[OnValueChanged("resetDebuffsIfNeeded"), TabGroup("Vulnerability"), DictionaryDrawerSettings(IsReadOnly = true)]
		public DebuffVulnerabilityDictionary Debuffability;
		public void resetDebuffsIfNeeded() {
			if (Debuffability.Count == DebuffType.DebuffTypes.Count) return;
			Debug.Log("Resetting debuff vulnerabilites.");
			Debuffability = blankDebuffVulnerabilityDictionary;
		}

		// ReSharper disable once MemberCanBePrivate.Global
		protected static DebuffVulnerabilityDictionary blankDebuffVulnerabilityDictionary{
			get {
				DebuffVulnerabilityDictionary r = new();
				foreach (DebuffType debuff in DebuffType.DebuffTypes) {
					r.Add(debuff, DebuffType.VulnerabilityValues.Immune);
				}

				return r;
			}
		}
		#endregion

		#region Stacks
		[ShowInInspector, TabGroup("Stacks"), HideLabel] private List<DebuffType> debuffStacks;
		internal void onStackGained(DebuffType type) => debuffStacks.Insert(0, type);
		// * if you don't understand why we add them to the beginning, think about how you want debuffs to look on hearts on the UI. it's hard to explain in text.
		public int StacksOf(DebuffType Type) {
			int value = 0;
			foreach (DebuffType stack in debuffStacks) {
				if (stack == Type) value++;
			}

			return value;
		}
		internal void onStackCleared(DebuffType type) => debuffStacks.Remove(type);
		#endregion

		#region Buildup
		[ShowInInspector, TabGroup("Buildup"), PropertyOrder(1), ListDrawerSettings(DraggableItems = false, HideAddButton = true)]
		private List<debuffBuildupManager> debuffBuildupManagers;
		// * We're not using a dictionary because Odin won't let us visualize them very well.
		
		[Button, TabGroup("Buildup"), ShowIf("@testType != null")] public void test() => GainOrLoseDebuffBuildup(testType, debuffAmount);
		[AssetSelector, TabGroup("Buildup"), HideLabel] public DebuffType testType;
		[TabGroup("Buildup"), HideLabel] public float debuffAmount;

		private debuffBuildupManager buildupManagerOfType(DebuffType type) {
			if (type == null) throw new Exception("Need a value.");
			foreach (debuffBuildupManager manager in debuffBuildupManagers) {
				if (manager.Type.name == type.name) return manager;
			}

			return null;
		}

		public void GainOrLoseDebuffBuildup(DebuffType Type, float Amount) {
			if (Amount == 0) throw new Exception("Invalid value.");
			if (buildupManagerOfType(Type) == null)
				debuffBuildupManagers.Add(new debuffBuildupManager(Type, this));
			buildupManagerOfType(Type).Amount += Amount;
		}

		internal void clearBuildup(DebuffType type) => debuffBuildupManagers.Remove(buildupManagerOfType(type));
		#endregion

		public void Update() {
			foreach (debuffBuildupManager VARIABLE in debuffBuildupManagers) { VARIABLE.Update(); }
		}

		internal DebuffManager() {
			debuffBuildupManagers = new List<debuffBuildupManager>();
			debuffStacks          = new List<DebuffType>();
		}
	}

	[HideReferenceObjectPicker] internal class debuffBuildupManager {
		
		
		[HorizontalGroup("A", 50), VerticalGroup("A/A"), HideLabel, ShowInInspector,
		 PreviewField(50, ObjectFieldAlignment.Left)]
		private Sprite sprite => Type.Sprite;

		[VerticalGroup("A/B"), HideLabel] public readonly DebuffType    Type;
		private readonly                                   DebuffManager debuffManager;
		private float BuildupPerStack =>
			Type.buildupPerStack / DebuffType.VulnerabilityModifiers[debuffManager.Debuffability[Type]];
		private float recoveryRate =>
			Type.RecoveryRate / DebuffType.VulnerabilityModifiers[debuffManager.Debuffability[Type]];

		[VerticalGroup("A/B"), HideLabel, ShowInInspector, ProgressBar(0, "BuildupPerStack", ColorGetter = "@Type.Color")]
		private float _amount;
		public float Amount{
			get => _amount;
			set {
				_amount = value;

				while (_amount > BuildupPerStack) {
					debuffManager.onStackGained(Type);
					_amount -= BuildupPerStack;
				}

				while (_amount < 0) {
					if (debuffManager.StacksOf(Type) > 0) {
						debuffManager.onStackCleared(Type);
						_amount += BuildupPerStack;
					}
					else {
						debuffManager.clearBuildup(Type);
						_amount = 0;
					}
				}
			}
		}

		internal debuffBuildupManager(DebuffType Type, DebuffManager debuffManager) {
			this.debuffManager = debuffManager;
			this.Type          = Type;
		}

		internal void Update() => Amount -= recoveryRate * Time.deltaTime;
	}

	[Serializable]
	public class
		DebuffVulnerabilityDictionary : UnitySerializedDictionary<DebuffType, DebuffType.VulnerabilityValues> { }
}