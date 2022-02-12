using UnityEngine;

namespace DefaultNamespace {
	[RequireComponent(typeof(HammerspaceToggler))] public class Bullet : UnityEngine.MonoBehaviour {
		public HammerspaceToggler HammerspaceToggler => GetComponent<HammerspaceToggler>();
		public Color              Color;
	}
}