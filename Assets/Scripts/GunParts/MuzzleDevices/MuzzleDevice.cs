using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

namespace GunParts.MuzzleDevices {
	[HideMonoScript, DisallowMultipleComponent] public abstract class MuzzleDevice : MonoBehaviour {
		public                                                         bool InfiniteUses;
		[Range(1, 100), HideIf("@InfiniteUses")]               public  int  MaxHealth = 5;
		[OdinSerialize]                                       private int  _health;
		[ShowInInspector, ProgressBar(0, "MaxHealth"), HideIf("@InfiniteUses")] public  int  Health => _health;

		private new Collider  collider  => GetComponent<Collider>();
		private Rigidbody rigidBody => gameObject.GetComponent<Rigidbody>();

		protected MuzzleDevice() => _health = MaxHealth;

		[HideInInspector] public UnityEvent eBroke = new UnityEvent();

		[Button] public void EnablePhysics() {
			collider.enabled      = true;
			rigidBody.isKinematic = false;
		}
		[Button] public void DisablePhysics() {
			collider.enabled      = false;
			rigidBody.isKinematic = true;
		}

		[Button] public void PassBullet() {
			_health--;
			if (_health == 0) { die(); }
		}
		[Button] private void die() {
			EnablePhysics();
			Destroy(this);
		}
	}
}