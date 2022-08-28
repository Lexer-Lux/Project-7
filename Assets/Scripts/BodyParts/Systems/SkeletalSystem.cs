using UnityEngine;
using static GMST;

namespace BodyParts.Systems {
	public class SkeletalSystem : SystemWithStat {
		public override Color Color    => Color.white;
		public override int   MaxValue => Level * OxygenPerSKL;
	}
}