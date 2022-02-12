using GunParts.Actions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Weapons.Ranged.Receivers {
	public class Manual : Action {
		[SuffixLabel("RPM"), Range(3, 60)] public float FireRate;
		// ReSharper disable once UnusedMember.Local
		private                                   float SecondsBetweenShots => 1 / (FireRate / 60);
		[ShowInInspector, ProgressBar(0, "SecondsBetweenShots")] private  float Cycle;
	}
}