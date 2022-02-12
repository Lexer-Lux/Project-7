using UnityEngine;

namespace Rounds {
	[RequireComponent(typeof(HammerspaceToggler))] public class Casing : MonoBehaviour {
		public HammerspaceToggler HammerspaceToggler => GetComponent<HammerspaceToggler>();
		public Cartridge          Cartridge;
		public Transform CrimpPoint;
	}
}