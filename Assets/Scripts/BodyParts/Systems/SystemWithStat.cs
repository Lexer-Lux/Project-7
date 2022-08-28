using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BodyParts.Systems {
	public abstract class SystemWithStat : BodyParts.Systems.System {
		[HorizontalGroup("Colors"), HideLabel] public Color ValueColor;
		[HorizontalGroup("Colors"), HideLabel] public Color MaxValueColor;

		[SerializeField] private int _Stat;
		[ShowInInspector, ProgressBar(0, "MaxValue", Segmented = true, ColorGetter = "ValueColor", BackgroundColorGetter = "MaxValueColor"), HideLabel]
		public int Stat{
			get => _Stat;
			set {
				_Stat = Math.Clamp(value, int.MinValue, MaxValue);
				eValueChanged.Invoke();
			}
		}
		public abstract          int        MaxValue{ get; }
		[HideInInspector] public UnityEvent eValueChanged, eMaxValueChanged;

		[OnValueChanged("ResetVulnerabilitiesIfNeeded"), DictionaryDrawerSettings(IsReadOnly = true)] public DamageVulnerabilityDictionary Vulnerability;
		// ReSharper disable once UnusedMember.Local
		private void ResetVulnerabilitiesIfNeeded() {
			if (Vulnerability.Count == DamageType.DamageTypes.Count) return;
			print("Resetting damage vulnerabilites.");
			Vulnerability = blankDamageVulnerabilityDictionary;
		}
		private DamageVulnerabilityDictionary blankDamageVulnerabilityDictionary{
			get {
				DamageVulnerabilityDictionary r = new();
				foreach (DamageType damageType in DamageType.DamageTypes) { r.Add(damageType, DamageType.VulnerabilityValues.Immune); }

				return r;
			}
		}

		public SystemWithStat() { eLevelChanged.AddListener(onMaxValueChanged); }

		public override void Start() {
			base.Start();
			eValueChanged    = new();
			eMaxValueChanged = new();
		}
		private void onMaxValueChanged() {
			if (Stat > MaxValue) Stat = MaxValue;
			eMaxValueChanged.Invoke();
		}
	}
	[Serializable] public class DamageVulnerabilityDictionary : UnitySerializedDictionary<DamageType, DamageType.VulnerabilityValues> { }
}