using System.Collections.Generic;
using UnityEngine;

namespace Rounds {
	[CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)] public class Cartridge : ScriptableObject {
		public enum Strengths { Subsonic, Regular, HighPower }
		public static Dictionary<Strengths, string> PowerNames => new Dictionary<Strengths, string> {{Strengths.Subsonic, "-S"}, {Strengths.Regular, ""}, {Strengths.HighPower, "+P"}};

		public GameObject CasingPrefab;
		public GameObject BulletPrefab;

		[Range(0, 50)] public int BaseDamage;
		[Range(0, 25)] public int Recoil = 0;

		public int OverPressureDamageBonus = 2;
		public int SubsonicDamageMalus     = -2;

		public float OverPressureSpeedBonus = 1.5f;
		public float SubsonicSpeedMalus     = 0.5f;

		public int OverPressureRecoilIncrease = 2;
		public int SubsonicRecoilDecrease     = -2;
	}
}