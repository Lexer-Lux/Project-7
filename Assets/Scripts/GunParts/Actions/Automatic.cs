using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Weapons.Ranged;
using Weapons.Ranged.Receivers;

namespace GunParts.Actions {
	public class Automatic : Action {
		[ShowInInspector, TabGroup("1", "Stats"), SuffixLabel("RPM"), ProgressBar(0, 600)]             public  int FireRate => (int) (baseFireRate + fireRateUpgrade.StatModifier);
		[TabGroup("1", "Stats"), SuffixLabel("RPM"), Range(100, 600), ShowInInspector, SerializeField] private int baseFireRate;
		// ReSharper disable once UnusedMember.Local
		private                                                                                                                                          float   SecondsBetweenShots => 1 / (FireRate / 60f);
		[ShowInInspector, ProgressBar(0, "SecondsBetweenShots", ColorGetter = "AmmoColor"), SuffixLabel("s"), HideIf("@CyclingCompleteIn <= 0")] private float   CyclingCompleteIn;
		[OdinSerialize, ShowInInspector, TabGroup("1", "Upgrade")]                                                                           private Upgrade fireRateUpgrade;
		public Automatic() { fireRateUpgrade = new Upgrade(this); }

		protected override void StrikeHammer() {
			base.StrikeHammer();
			CyclingCompleteIn = SecondsBetweenShots;
		}
		public void Update() {
			if (CyclingCompleteIn > 0) {
				CyclingCompleteIn -= Time.deltaTime;
				if (CyclingCompleteIn <= 0) {
					Eject();
					ChamberRound();
				}
			}
		}
	}
}