using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

#nullable enable

namespace GunParts {
	[HideMonoScript, ExecuteAlways] public abstract class GunPart : SerializedMonoBehaviour {
		[HideInInspector] public UnityEvent eStart = new UnityEvent();
		protected                Receiver   Receiver => GetComponent<Receiver>();

		public void Start() => eStart.Invoke();
	}
	[Serializable] sealed class Upgrade {
		[ShowInInspector, ProgressBar(0, "UpgradeLevels", Segmented = true), HideIf("@UpgradeLevels == 0")]
		public int UpgradeLevel{
			get => _upgradeLevel;
			set {
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

		[OdinSerialize, HideInInspector] private                                                                     int                                             _upgradeLevel;
		                public                                                                                       float                                           StatModifier => _statModifier;
		[OdinSerialize, Sirenix.OdinInspector.ReadOnly] private                                                      float                                           _statModifier;
		public                                                                                                       int                                             UpgradeLevels => UpgradesList?.Count ?? 0;
		[OdinSerialize, ShowInInspector, ValidateInput("validateUpgradesList", null, InfoMessageType.Error)] private List<(float Modifier, GameObject? Attachment)>? UpgradesList;
		private bool validateUpgradesList(ref string errorMessage) {
			if (UpgradesList != null) {
				for (int i = 0; i < UpgradeLevels; i++) {
					(float modifier, GameObject? attachment) = UpgradesList[i];
					if (modifier <= 0) {
						errorMessage = $"Modifier at index {i} is less than or equal to 0. All gun upgrade modifiers are positive by convention.";
						return false;
					}
				}
			}

			return true;
		}

		public Upgrade(GunPart part) { part.eStart.AddListener(recalculate); }

		public void recalculate() => UpgradeLevel = UpgradeLevel;
		// * Since we don't cache the upgrade level, we need to refresh it upon load.
	}
}