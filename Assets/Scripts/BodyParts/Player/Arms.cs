using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

#nullable enable

namespace BodyParts.Player {
	[HideMonoScript, DisallowMultipleComponent, RequireComponent(typeof(Animator))] public sealed class Arms : BodyParts.Arms {
		[BoxGroup("Left Hand")] [OdinSerialize, ShowInInspector, Required, ChildGameObjectsOnly]  public Hand     LeftHand    { get; private set; } = null!;
		[BoxGroup("Right Hand")] [OdinSerialize, ShowInInspector, Required, ChildGameObjectsOnly] public Hand     RightHand   { get; private set; } = null!;
		[BoxGroup("Left Hand")] [OdinSerialize, ShowInInspector, Required, ChildGameObjectsOnly]  public Holster  LeftHolster { get; private set; } = null!;
		[BoxGroup("Right Hand")] [OdinSerialize, ShowInInspector, Required, ChildGameObjectsOnly] public Holster  RightHolster{ get; private set; } = null!;
		private                                                                                          Animator animator    => GetComponent<Animator>();

		public override bool LeftHandPressed { set => animator.SetBool("Left Hand Button", value); }
		public override bool LeftArmPressed  { set => animator.SetBool("Left Arm Button", value); }
		public override bool RightHandPressed{ set => animator.SetBool("Right Hand Button", value); }
		public override bool RightArmPressed { set => animator.SetBool("Right Arm Button", value); }

		[BoxGroup("Left Hand"), Button("Unholster")] public void UnholsterLeftHandWeapon() {
			if (LeftHolster.Item == null || LeftHand.Weapon != null) {
				Debug.Log("No item in L holster to unholster or you already have an item in your L hand.");
				return;
			}
			Weapon toUnholster = LeftHolster.Item;
			LeftHolster.Item = null;
			LeftHand.Weapon  = toUnholster;
		}
		[BoxGroup("Left Hand"), Button("Holster")] public void HolsterLeftHandWeapon() {
			if (LeftHand.Weapon == null || LeftHolster.Item != null) {
				Debug.Log("Your L holster is full or you have no L hand item to holster.");
				return;
			}
			Weapon toHolster = LeftHand.Weapon;
			LeftHand.Weapon  = null;
			LeftHolster.Item = toHolster;
		}

		[BoxGroup("Right Hand"), Button("Unholster")] public void UnholsterRightHandWeapon() {
			if (RightHolster.Item == null || RightHand.Weapon != null) {
				Debug.Log("No R holster item to unholster or you alreadhy have an item in your R hand.");
				return;
			}
			Weapon toUnholster = RightHolster.Item;
			RightHolster.Item = null;
			RightHand.Weapon  = toUnholster;
		}

		[BoxGroup("Right Hand"), Button("Holster")] public void HolsterRightHandWeapon() {
			if (RightHand.Weapon == null || RightHolster.Item != null) {
				Debug.Log("You either have no R hand item to holster or your R holster is already full.");
				return;
			}
			Weapon toHolster = RightHand.Weapon;
			RightHand.Weapon  = null;
			RightHolster.Item = toHolster;
		}

		public void Start() {
			if (LeftHolster == null || RightHolster == null || LeftHand == null || RightHand == null) throw new InvalidOperationException();
		}
	}
}