using System;
using BodyParts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable

namespace BodyParts.Player {
	[RequireComponent(typeof(PlayerCharacter), typeof(Animator), typeof(Rigidbody))] [RequireComponent(typeof(CirculatorySystem), typeof(NervousSystem), typeof(IntegumentarySystem))] public sealed class Legs : BodyParts.Legs {
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

		[TitleGroup("Velocity")]
		[ShowInInspector, HideInEditorMode, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")]                   public  float Velocity => gameObject.GetComponent<Rigidbody>().velocity.magnitude;

		[SerializeField, HorizontalGroup("Velocity/Sneak", 0.4f), LabelText("Sneak ="), Range(0, 10), LabelWidth(50)] private float baseSneakSpeed   = 0.5f;
		[SerializeField, HorizontalGroup("Velocity/Sneak", 0.4f), LabelText("+ INT x"), LabelWidth(50), Range(0, 1)]                          private float sneakSpeedPerINT = 0.1f;
		[ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Sneak", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)]              public  float SneakSpeed   => GetComponent<IntegumentarySystem>().Level * sneakSpeedPerINT + baseSneakSpeed;
		private                                                                                                                                       Color sneakColor() => MovementModeColors[MovementModes.Crouch];

		[SerializeField, HorizontalGroup("Velocity/Walk", 0.4f), LabelText("Walk ="), Range(0, 10), LabelWidth(50)]             private float baseWalkSpeed   = 1.0f;
		[SerializeField, HorizontalGroup("Velocity/Walk", 0.4f), LabelText("+ NRV x"), LabelWidth(50), Range(0, 1)]             private float walkSpeedPerNRV = 0.1f;
		[ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Walk", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)] public  float WalkSpeed => GetComponent<NervousSystem>().Level * walkSpeedPerNRV + baseWalkSpeed;

		[SerializeField, HorizontalGroup("Velocity/Run", 0.4f), LabelText("Run ="), Range(0, 10), LabelWidth(50)]              private float baseRunSpeed   = 3f;
		[SerializeField, HorizontalGroup("Velocity/Run", 0.4f), LabelText("+ CRC x"), LabelWidth(50), Range(0, 1)]             private float runSpeedPerCRC = 0.5f;
		[ShowInInspector, ReadOnly, HorizontalGroup("Velocity/Run", 0.2f), LabelText("="), SuffixLabel("m/s"), LabelWidth(25)] public  float RunSpeed => GetComponent<CirculatorySystem>().Level * runSpeedPerCRC + baseRunSpeed;
		[SerializeField, TitleGroup("Velocity"), Range(0, 100), SuffixLabel("m/s")]                                            private float AirborneMaxSpeed = 10.0f;

		public float MaxSpeed{
			get {
				return MovementMode switch {
					MovementModes.Crouch => SneakSpeed, MovementModes.Walk => WalkSpeed, MovementModes.Run => RunSpeed, MovementModes.Airborne => AirborneMaxSpeed, _ => throw new ArgumentOutOfRangeException()
				};
			}
		}
		[TitleGroup("Momentum")] [ShowInInspector, HorizontalGroup("Momentum/Sneak", 0.25f), LabelText("Sneak ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)] private float sneakMomentum => adjustedMomentum * sneakMomentumMultiplier;
		[ShowInInspector, HorizontalGroup("Momentum/Walk", 0.25f), LabelText("Walk ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)]                             private float walkMomentum  => adjustedMomentum;
		[ShowInInspector, HorizontalGroup("Momentum/Run", 0.25f), LabelText("Run ="), SuffixLabel("N・s"), LabelWidth(50), PropertyOrder(0)]                              private float runMomentum   => adjustedMomentum * runMomentumMultiplier;

		[SerializeField, LabelText("="), LabelWidth(20), HorizontalGroup("Momentum/Sneak", 0.25f), HorizontalGroup("Momentum/Walk", 0.25f), HorizontalGroup("Momentum/Run", 0.25f), PropertyOrder(1), Range(0, 100)]                        private float baseMomentum            = 50f;
		[SerializeField, ShowInInspector, LabelText("+ INT x"), LabelWidth(35), HorizontalGroup("Momentum/Sneak", 0.25f), HorizontalGroup("Momentum/Walk", 0.25f), HorizontalGroup("Momentum/Run", 0.25f), PropertyOrder(2), Range(0, 100)] private float momentumPerINT          = 10f;
		[SerializeField, ShowInInspector, HorizontalGroup("Momentum/Sneak", 0.25f), LabelText("x"), LabelWidth(15), PropertyOrder(3), Range (0, 1)]                                                                                      private float sneakMomentumMultiplier = 0.5f;
		[SerializeField, ShowInInspector, HorizontalGroup("Momentum/Run", 0.25f), LabelText("x"), LabelWidth(15), PropertyOrder(3), Range(1, 3)]                                                                                        private float runMomentumMultiplier   = 1.5f;

		private float adjustedMomentum => baseMomentum + GetComponent<IntegumentarySystem>().Level * momentumPerINT;

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