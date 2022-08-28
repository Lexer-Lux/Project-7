using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GunParts;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Action = GunParts.Actions.Action;

#nullable enable

namespace Weapons.Ranged.Magazines {
	[ExecuteAlways, DisallowMultipleComponent] public abstract class Magazine : GunPart {
		[ShowInInspector, TabGroup("1", "Stats"), ProgressBar(0, "Capacity", Segmented = true, ColorGetter = "AmmoColor", DrawValueLabel = true)] public int Ammo => Contents?.Count ?? 0;

		private const                               string       MAGFEED_NAME = "Mag Feed";
		public abstract                             int          Capacity{ get; }
		public                                      bool         Empty   => Ammo < 1;
		[ShowInInspector, SerializeField] protected bool         infiniteAmmo; 
		protected abstract                          List<Round>? Contents{ get; }

		/// <summary> Guns with internal mags will put their rounds in here. Guns with external mags will attach their mag here.  </summary>
		protected GameObject magFeed => gameObject.transform.Find(MAGFEED_NAME).gameObject;
		// ReSharper disable once UnusedMember.Local
		private Color AmmoColor => Contents?.FirstOrDefault()?.Color ?? Color.gray;

		[ShowInInspector, OdinSerialize, TabGroup("1", "Speed Upgrade", Order = 2)] private Upgrade reloadSpeedUpgrade;
		[ShowInInspector, ProgressBar(0, 1), TabGroup("1", "Stats")]                public  float   ReloadSpeedModifier => 1 - reloadSpeedUpgrade.StatModifier;

		protected Magazine() => reloadSpeedUpgrade = new Upgrade(this);

		public void Awake() {
			if (gameObject.transform.Find(MAGFEED_NAME) != null) return;
			var newMagFeed = new GameObject(MAGFEED_NAME);
			newMagFeed.transform.SetParent(transform);
		}

		public abstract Round Feed();
	}
}