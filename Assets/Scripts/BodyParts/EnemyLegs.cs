using System;
using System.Collections.Generic;
using Body_Parts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using static Body_Parts.ILegs;
#pragma warning disable CS0414

namespace Body_Parts {
	public class EnemyLegs : MonoBehaviour, ILegs {
		private                                              Entity Owner => gameObject.GetComponent<Entity>();
		[ShowInInspector, ReadOnly, HideInEditorMode] public MovementModes MovementMode => MovementModes.Walk;
		// ReSharper disable once UnusedMember.Local
		private                                              Color moveColor() => MovementModeColors[MovementMode];

		[ShowInInspector, ProgressBar(0, "MaxSpeed", ColorGetter = "moveColor")]
		public float Velocity => Owner.RigidBody.velocity.magnitude;
		[ShowInInspector, ReadOnly] public float MaxSpeed => MoveModeVelocitiesDictionary[MovementMode];

		private                                                                         int                              nominalMaxSpeed = 50;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] private float                            _SneakSpeed     = 3;
		public                                                                          float                            SneakSpeed => _SneakSpeed;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] public  float                            _WalkSpeed = 6;
		public                                                                          float                            WalkSpeed => _WalkSpeed;
		[SerializeField, ProgressBar(0, "nominalMaxSpeed"), SuffixLabel("m/s")] public  float                            _RunSpeed = 9;
		public                                                                          float                            RunSpeed => _RunSpeed;
		private readonly                                                                Dictionary<MovementModes, float> MoveModeVelocitiesDictionary;

		public EnemyLegs() {
			MoveModeVelocitiesDictionary = new Dictionary<MovementModes, float>() {
				{MovementModes.Crouch, SneakSpeed}, {MovementModes.Walk, WalkSpeed}, {MovementModes.Run, RunSpeed}
			};
		}
	}
}