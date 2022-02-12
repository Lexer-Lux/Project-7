using GunParts.Actions;

namespace Weapons.Ranged.Receivers {
	public class SemiAuto : Action {
		protected override void StrikeHammer() {
			base.StrikeHammer();
			Eject();
			ChamberRound();
		}
	}
}