using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

#nullable enable

namespace GunParts {
	[HideMonoScript, ExecuteAlways] public abstract class GunPart : SerializedMonoBehaviour {
		[HideInInspector] public UnityEvent eStart = new UnityEvent();
		protected                Gun        gun => GetComponent<Gun>();

		public void Start() => eStart.Invoke();
	}
	[Serializable] sealed class Upgrade {
		[ShowInInspector, ProgressBar(0, "UpgradeLevels", Segmented = true)]
		public int UpgradeLevel{
			get => _upgradeLevel;
			set {
				Debug.Log(value);
				if (value < 0 || value > UpgradeLevels) {
					var up = new Exception($"$Value {{value}} is out of range 0 -{UpgradeLevels}.");
					Debug.LogException(up);
					throw up;
				}

				float newModifier = 0;
				if (UpgradesList != null) {
					for (int i = 0; i < UpgradeLevels; i++) {
						(float modifier, GameObject? attachment) = UpgradesList[i];
						if (i < value) {
							newModifier += modifier;
							if (attachment != null) attachment.SetActive(true);
						}
						else {
							if (attachment != null) attachment.SetActive(false);
						}
					}
				}

				_statModifier = newModifier;
				_upgradeLevel = value;
			}
		}

		[OdinSerialize, HideInInspector] private int                                             _upgradeLevel;
		[ShowInInspector]                public  float                                           StatModifier => _statModifier;
		[OdinSerialize]                  private float                                           _statModifier;
		public                                   int                                             UpgradeLevels => UpgradesList?.Count ?? 0;
		[OdinSerialize, ShowInInspector] private List<(float Modifier, GameObject? Attachment)>? UpgradesList;
		
		public Upgrade(GunPart part) { part.eStart.AddListener(recalculate); }

		public void recalculate() => UpgradeLevel = UpgradeLevel;
		// * Since we don't cache the upgrade level, we need to refresh it upon load.
	}
}