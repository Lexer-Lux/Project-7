using Sirenix.OdinInspector;
using UnityEngine;

namespace Body_Parts.Systems {
	public class NervousSystem : System {
		public override Color Color        => new Color(0.87f, 1f, 0f);
		[ShowInInspector, SuffixLabel("/s")] public          float StaminaRegen => Level * GMST.StaminaRegenPerNRV;

	}
}