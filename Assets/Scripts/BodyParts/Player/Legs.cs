using System;
using BodyParts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace BodyParts.Player {
	[RequireComponent(typeof(PlayerCharacter), typeof(Animator), typeof(Rigidbody))] [RequireComponent(typeof(CirculatorySystem), typeof(NervousSystem), typeof(IntegumentarySystem))] public sealed class Legs : BodyParts.Legs {
		[ShowInInspector, HideInEditorMode, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")] public float Velocity => gameObject.GetComponent<Rigidbody>().velocity.magnitude;

		public override MovementModes MovementMode{
			get {
				if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Crouch")) { return MovementModes.Crouch; }
				else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Run")) { return MovementModes.Run; }
				else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk")) { return MovementModes.Walk; }
				else if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Airborne")) { return MovementModes.Airborne; }
				else throw new Exception(); 
			}
		}
		public override bool      DownHeld  { set => animator.SetBool("Down Held", value); }
		public override bool      UpHeld    { set => animator.SetBool("Up Held", value); }
		private         Rigidbody rigidBody => GetComponent<Rigidbody>();
		public override Vector2 Movement{
			set {
				_movement = new Vector3(value.x, 0, value.y);
				animator.SetFloat("Movingness", value.magnitude);
			}
		}
		private Vector3 _movement = Vector3.zero;
		public  bool    CanMove   = true;

		// ReSharper disable once UnusedMember.Local
		private Color moveColor() => MovementModeColors[MovementMode];

		// * Male walk speed: 1.45m/s (age 40) - 1.00m/s (age 70), Usain Bolt peak speed: 12.42m/s
		[FoldoutGroup("Speeds")]

		// TODO: I changed my mind. Move all these GMSTs out into their own thing again, and make a nice Odin editor window for them.
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]
		public float SneakSpeed => GetComponent<IntegumentarySystem>().Level * sneakSpeedPerINT + baseSneakSpeed;
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]                                 public  float WalkSpeed => GetComponent<NervousSystem>().Level * walkSpeedPerNRV + baseWalkSpeed;
		[ShowInInspector, ReadOnly, FoldoutGroup("Speeds")]                                 public  float RunSpeed  => GetComponent<CirculatorySystem>().Level * runSpeedPerCRC + baseRunSpeed;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float baseSneakSpeed   = 0.5f;
		[FormerlySerializedAs("sneakSpeedPerCTV")] [SerializeField, FoldoutGroup("Speeds")] private float sneakSpeedPerINT = 0.1f;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float baseWalkSpeed    = 1.0f;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float walkSpeedPerNRV  = 0.1f;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float baseRunSpeed     = 3f;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float runSpeedPerCRC   = 0.5f;
		[SerializeField, FoldoutGroup("Speeds")]                                            private float AirborneMaxSpeed = 10.0f;

		public float MaxSpeed{
			get {
				return MovementMode switch {
					MovementModes.Crouch => SneakSpeed, MovementModes.Walk => WalkSpeed, MovementModes.Run => RunSpeed, MovementModes.Airborne => AirborneMaxSpeed, _ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		[ShowInInspector, FoldoutGroup("Momentum")] private float baseMomentum            = 50f;
		[ShowInInspector, FoldoutGroup("Momentum")] private float momentumPerINT          = 10f;
		[ShowInInspector, FoldoutGroup("Momentum")] private float sneakMomentumMultiplier = 0.5f;
		[ShowInInspector, FoldoutGroup("Momentum")] private float runMomentumMultiplier   = 1.5f;
		[ShowInInspector, FoldoutGroup("Momentum")] private float adjustedMomentum => baseMomentum + GetComponent<IntegumentarySystem>().Level * momentumPerINT;
		[ShowInInspector, FoldoutGroup("Momentum")] private float sneakMomentum    => adjustedMomentum * sneakMomentumMultiplier;
		[ShowInInspector, FoldoutGroup("Momentum")] private float walkMomentum     => adjustedMomentum;
		[ShowInInspector, FoldoutGroup("Momentum")] private float runMomentum      => adjustedMomentum * runMomentumMultiplier;
		[ShowInInspector]
		public float momentum{
			get {
				return MovementMode switch {
					(int) MovementModes.Crouch => sneakMomentum, MovementModes.Walk => walkMomentum, MovementModes.Run => runMomentum, MovementModes.Airborne => 0, _ => throw new Exception("That's not a movement mode that exists (yet).")
				};
			}
		}

		protected float velocity => GetComponent<Rigidbody>().velocity.magnitude;

		[ShowInInspector] private static float playbackSpeedModifier = 0.15f;
		protected override               void  setSneakPlaybackSpeed() => animator.SetFloat("Sneak Playback Speed", 1 + ((GetComponent<IntegumentarySystem>().Level - 6) * playbackSpeedModifier));
		protected override               void  setWalkPlaybackSpeed()  => animator.SetFloat("Walk Playback Speed", 1 + ((GetComponent<NervousSystem>().Level - 6) * playbackSpeedModifier));
		protected override               void  setRunPlaybackSpeed()   => animator.SetFloat("Run Playback Speed", 1 + ((GetComponent<CirculatorySystem>().Level - 6) * playbackSpeedModifier));

		public override void Start() {
			base.Start();
			GetComponent<IntegumentarySystem>().eLevelChanged.AddListener(setSneakPlaybackSpeed);
			GetComponent<NervousSystem>().eLevelChanged.AddListener(setWalkPlaybackSpeed);
			GetComponent<CirculatorySystem>().eLevelChanged.AddListener(setRunPlaybackSpeed);
		}

		private void FixedUpdate() {
			if (!CanMove) return;
			// * If you've only pressed the stick 0.5 of the way, then you should only move 50% of max speed.
			float desiredSpeed = _movement.magnitude * MaxSpeed;
			// * How much momentum is being applied this physics tick?
			Vector3 movementImpulse = _movement * (momentum * Time.deltaTime);

			animator.SetBool("Inputting Movement", _movement.magnitude > 0);

			float currentSpeed = rigidBody.velocity.magnitude;

			if (currentSpeed < desiredSpeed) {
				rigidBody.AddForce(movementImpulse, ForceMode.Impulse);
				// * In case we overshot it 
				if (currentSpeed > desiredSpeed) rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, MaxSpeed);
			}
			else if (currentSpeed > desiredSpeed) {
				decelerate();
				if (currentSpeed < desiredSpeed) rigidBody.velocity = rigidBody.velocity.normalized * desiredSpeed;
			}

			rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, MaxSpeed);
			animator.SetFloat("Velocity", rigidBody.velocity.magnitude);

			void decelerate() {
				// ! This took me ages to get right and I'm still not sure how it works so I'll just leave it here, untouched.
				float frictionMomentum = momentum / rigidBody.mass; // ! WHAT THE FUCK. I SHOULD HAVE TO MULTIPLY THIS BY FIXEDDELTATIME FOR IT TO WORK RIGHT, BUT NO. NO WONDER IT TOOK ME AGES TO MAKE THIS WORK RIGHT. WTF UNITY? I AM SO ANGRY RN.
				float currentMomentum  = rigidBody.velocity.magnitude * rigidBody.mass;
				if (frictionMomentum < currentMomentum) {
					Vector3 friction = -rigidBody.velocity.normalized * frictionMomentum;
					rigidBody.AddForce(friction, ForceMode.Impulse);
				}
			}
		}
	}
}