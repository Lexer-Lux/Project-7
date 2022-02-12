using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static Body_Parts.ILegs;

namespace Body_Parts {
	[RequireComponent(typeof(PlayerCharacter))] public class PlayerLegs : MonoBehaviour, ILegs {
		protected PlayerCharacter Owner    => gameObject.GetComponent<PlayerCharacter>();

		[ShowInInspector, HideInEditorMode, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")]
		public float Velocity => gameObject.GetComponent<Rigidbody>().velocity.magnitude;
		// ReSharper disable once UnusedMember.Local
		private Color moveColor() => MovementModeColors[MovementMode];
		[ShowInInspector, ReadOnly, HideInEditorMode]
		public MovementModes MovementMode{
			get {
				if (Owner.animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling") ||
				    Owner.animator.GetCurrentAnimatorStateInfo(0).IsName("Jumping")) { return MovementModes.Airborne; }
				else if (Owner.crouch) { return MovementModes.Crouch; }
				else if (Owner.upHold) { return MovementModes.Run; }
				else { return MovementModes.Walk; }
			}
		}

		// * Male walk speed: 1.45m/s (age 40) - 1.00m/s (age 70), Usain Bolt peak speed: 12.42m/s
		[FoldoutGroup("Speeds")]

		// TODO: I changed my mind. Move all these GMSTs out into their own thing again, and make a nice Odin editor window for them.
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]
		public float SneakSpeed => Owner.INT * sneakSpeedPerINT + baseSneakSpeed;
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]
		public float WalkSpeed => Owner.NRV * walkSpeedPerNRV + baseWalkSpeed;
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]
		public float RunSpeed => Owner.CRC * runSpeedPerCRC + baseRunSpeed;
		[SerializeField, FoldoutGroup("Speeds")]     private float baseSneakSpeed   = 0.5f;
		[FormerlySerializedAs("sneakSpeedPerCTV")] [SerializeField, FoldoutGroup("Speeds")] private float sneakSpeedPerINT = 0.1f;
		[SerializeField, FoldoutGroup("Speeds")]     private float baseWalkSpeed    = 1.0f;
		[SerializeField, FoldoutGroup("Speeds")]     private float walkSpeedPerNRV  = 0.1f;
		[SerializeField, FoldoutGroup("Speeds")]     private float baseRunSpeed     = 3f;
		[SerializeField, FoldoutGroup("Speeds")]     private float runSpeedPerCRC   = 0.5f;
		[SerializeField, FoldoutGroup("Speeds")]     private float AirborneMaxSpeed = 10.0f;

		public float MaxSpeed{
			get {
				return MovementMode switch {
					MovementModes.Crouch => SneakSpeed,
					MovementModes.Walk => WalkSpeed,
					MovementModes.Run => RunSpeed,
					MovementModes.Airborne => AirborneMaxSpeed,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		[ShowInInspector, FoldoutGroup("Momentum")] protected float baseMomentum            = 50f;
		[ShowInInspector, FoldoutGroup("Momentum")] protected float momentumPerINT          = 10f;
		[ShowInInspector, FoldoutGroup("Momentum")] protected float sneakMomentumMultiplier = 0.5f;
		[ShowInInspector, FoldoutGroup("Momentum")] protected float runMomentumMultiplier   = 1.5f;
		[ShowInInspector, FoldoutGroup("Momentum")]
		protected float adjustedMomentum => baseMomentum + Owner.INT * momentumPerINT;
		[ShowInInspector, FoldoutGroup("Momentum")]
		protected float sneakMomentum => adjustedMomentum * sneakMomentumMultiplier;
		[ShowInInspector, FoldoutGroup("Momentum")] protected float walkMomentum => adjustedMomentum;
		[ShowInInspector, FoldoutGroup("Momentum")]
		protected float runMomentum => adjustedMomentum * runMomentumMultiplier;
		[ShowInInspector]
		public float momentum{
			get {
				return MovementMode switch {
					(int) MovementModes.Crouch => sneakMomentum,
					MovementModes.Walk => walkMomentum,
					MovementModes.Run => runMomentum,
					MovementModes.Airborne => 0,
					_ => throw new Exception("That's not a movement mode that exists (yet).")
				};
			}
		}
	}
}