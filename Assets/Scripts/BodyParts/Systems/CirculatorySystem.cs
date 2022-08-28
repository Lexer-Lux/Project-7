using UnityEngine;
using static GMST;

namespace BodyParts.Systems {
	public class CirculatorySystem : SystemWithStat {
		public override Color Color    => Color.red;
		public override int   MaxValue => Level * BloodPerSKL;
	}
	
	
}