using GunParts;
using GunParts.Actions;
using Sirenix.OdinInspector;
using UnityEngine;
using Weapons.Ranged;
using Weapons.Ranged.Magazines;
using Weapons.Ranged.Receivers;

namespace Weapons {
	[HideMonoScript, DisallowMultipleComponent] public class Gun : MonoBehaviour {
		public Magazine Magazine => GetComponent<Magazine>();
		public Action Action => GetComponent<Action>();
		public Barrel Barrel => GetComponent<Barrel>();
	}
}