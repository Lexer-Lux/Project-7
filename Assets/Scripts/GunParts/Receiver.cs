using GunParts.Actions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

namespace GunParts {
	[HideMonoScript, DisallowMultipleComponent] public sealed class Receiver : Weapon {
		public  bool      HasRecoil = true;
		public  AudioClip FireSound;
		private float     _accuracy;
		[ShowInInspector]
		public float Accuracy{
			get => _accuracy;
			private set {
				if (value > _accuracy) eAccuracyWorsened.Invoke();
				if (value > maxSpread) maxSpreadExceeded();
				_accuracy = value;
			}
		}
		[ShowInInspector, SerializeField] private int   maxSpread = 25;
		public  float Precision;
		[ShowInInspector]                public  float Spread => Precision + Accuracy;
		
		public                                  Action     Action => GetComponent<Action>();
		public                                  Barrel     Barrel => GetComponent<Barrel>();
		[HideInInspector] public                                  UnityEvent eAccuracyWorsened;
		public Receiver() => eAccuracyWorsened = new UnityEvent();
		public void ApplyRecoil(Round round) {
			if (!HasRecoil) return;
			Rigidbody bulletRigidBody = round.Bullet.GetComponent<Rigidbody>();
			GetComponent<Rigidbody>().AddForce(bulletRigidBody.velocity * -1 * bulletRigidBody.mass, ForceMode.Impulse);
			Accuracy += round.Recoil;
		}
		private void maxSpreadExceeded() {
			Debug.Log("Gun should fly out of your hand or someth now.");
		}
	}
}