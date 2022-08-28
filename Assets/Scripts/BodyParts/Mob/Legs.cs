using System.Collections.Generic;
using Entities;
using Sirenix.OdinInspector;
using UnityEngine;

#pragma warning disable CS0414

namespace BodyParts.Mob {
	public class EnemyLegs : Legs {
		private                                              Entity        Owner       => gameObject.GetComponent<Entity>();
		[ShowInInspector, ReadOnly, HideInEditorMode] public MovementModes MovementMode{ get; set; } = MovementModes.Walk;
		// ReSharper disable once UnusedMember.Local
		private Color moveColor() => MovementModeColors[MovementMode];

		[ShowInInspector, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")] public float Velocity => Owner.RigidBody.velocity.magnitude;
		[ShowInInspector, ReadOnly]                                              public float MaxSpeed => MoveModeVelocitiesDictionary[MovementMode];

		private                                                                         int                              nominalMaxSpeed = 50;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] private float                            _SneakSpeed     = 3;
		public                                                                          float                            SneakSpeed => _SneakSpeed;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] public  float                            _WalkSpeed = 6;
		public                                                                          float                            WalkSpeed => _WalkSpeed;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] public  float                            _RunSpeed = 9;
		public                                                                          float                            RunSpeed => _RunSpeed;
		private readonly                                                                Dictionary<MovementModes, float> MoveModeVelocitiesDictionary;

		public override bool DownHeld{
			set {
				if (value == false) MovementMode = MovementModes.Walk;
				else MovementMode                = MovementModes.Crouch;
			}
		}
		public override bool UpHeld{
			set {
				if (value == false) MovementMode = MovementModes.Walk;
				else MovementMode                = MovementModes.Run;
			}
		}

		private         Vector2 movement;
		public override Vector2 Movement{ set => movement = value; }

		public EnemyLegs() { MoveModeVelocitiesDictionary = new Dictionary<MovementModes, float>() {{MovementModes.Crouch, SneakSpeed}, {MovementModes.Walk, WalkSpeed}, {MovementModes.Run, RunSpeed}}; }
	}
}