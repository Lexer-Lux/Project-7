using System;
using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

namespace BodyParts.Player {
	[DisallowMultipleComponent, HideMonoScript] public sealed class Hand : MonoBehaviour{
		[ShowInInspector]
		public Weapon? Weapon{
			get => _weapon;
			set {
				if (_weapon == value) return;
				else if (value != null && _weapon != null) throw new InvalidOperationException("You'll need at least 6 fingers to do that. Being a cartoon character, you only have 4.");
				else if (value != null && _weapon == null) {
					value.RigidBody.isKinematic = false;
					value.RigidBody.useGravity  = true;
					value.transform.rotation    = gameObject.transform.rotation;
					value.transform.Rotate(180, 0, 0);
					//value.transform.rotation    = Quaternion.Euler(0, 90, 0);
					grip.connectedBody          = value.Grip;
					_weapon                     = value;
				} else if (value == null && _weapon != null) {
					grip.connectedBody = null;
					_weapon            = value;
				}
				else throw new Exception("I didn't anticipate this one.");
			}
		}
		[SerializeField, HideInInspector] private Weapon? _weapon;
		
		private                                             ConfigurableJoint grip => gameObject.GetComponent<ConfigurableJoint>();

		public void Start() {
			if (grip == null) throw new InvalidOperationException("Your grip strength has failed the vibe check.");
		}
	}
}