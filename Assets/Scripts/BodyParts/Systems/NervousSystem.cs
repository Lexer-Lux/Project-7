using Sirenix.OdinInspector;
using UnityEngine;

namespace BodyParts.Systems {
	public class NervousSystem : BodyParts.Systems.System {
		public override Color Color        => new Color(0.87f, 1f, 0f);
		[ShowInInspector, SuffixLabel("/s")] public          float StaminaRegen => Level * GMST.StaminaRegenPerNRV;

	}
}