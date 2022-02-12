using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static GMST;

namespace Body_Parts.Systems {
	public class CirculatorySystem : SystemWithStat {
		public override Color Color    => Color.red;
		public override int   MaxValue => Level * BloodPerSKL;
	}
	
	
}