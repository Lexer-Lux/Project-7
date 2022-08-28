using System;
using BodyParts;
using BodyParts.Player;
using BodyParts.Systems;
using Entities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using static HUD;
using static BodyParts.Legs;

#nullable enable

[RequireComponent(typeof(BodyParts.Player.Legs))] [RequireComponent(typeof(CirculatorySystem), typeof(SkeletalSystem), typeof(NervousSystem))] public class PlayerCharacter : Entity {
	public static                             PlayerCharacter?      PC;

	public                   Transform  headTransform; //TODO: Set in code, then hide in inspector.
	public                   Animator   animator => gameObject.GetComponent<Animator>();

	// * MISC.
	[ShowInInspector]
	public                   float      Visibility => (Legs.MovementMode == MovementModes.Crouch) ? 0.5f : 1f;
	protected                bool       canTurn    => Legs.MovementMode != MovementModes.Airborne;

	protected override void Start() {
		PC = this;
		base.Start();
	}

	protected void roll() {
		//TODO: Face forward while rolling and walking.
		Vector3 rollVelocity = RigidBody.velocity.normalized * 10;
		RigidBody.velocity = rollVelocity;
	}
	protected void jump() {
		print("jumping");
		RigidBody.AddForce(new Vector3(0, 25, 0), ForceMode.VelocityChange);
	}
}