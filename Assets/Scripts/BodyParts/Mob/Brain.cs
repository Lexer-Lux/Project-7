using System;
using System.Collections.Generic;
using System.ComponentModel;
using AIStates;
using Entities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.Collections;
using UnityEngine;

#nullable enable

namespace BodyParts.Mob {
	[HideMonoScript, RequireComponent(typeof(Entity), typeof(Eyes))] public class Brain : MonoBehaviour {
		private Eyes   eyes      => gameObject.GetComponent<Eyes>();
		public  float? Alertness => AIState?.AttentionModifier;
		public  Color?  Mood      => AIState?.Color;

		[ShowInInspector]
		public AIState? AIState{
			get => _AIState;
			set {
				if (AIState == value || value.enabled) Debug.LogWarning("AI script is already enabled or flagged as active!");
				if (value != DefaultState && value != AlertedState && value != HostileState) Debug.LogWarning("Not a valid AI state!");

				if (AIState != null) AIState.enabled = false;
				value.enabled = true;
				_AIState       = value;
			}
		}
		[SerializeField, HideInInspector] private AIState? _AIState;

		private AIState DefaultState => gameObject.GetComponent<Patrolling>();
		private AIState AlertedState => gameObject.GetComponent<Searching>();
		private AIState HostileState => gameObject.GetComponent<Attacking>();

		private bool timeToDecreaseHostility => !eyes.seeingPlayer && eyes.LOSlength == 0;
		private bool timeToIncreaseHostility => eyes.seeingPlayer && eyes.playerIdentified;

		public void Start() {
			if (AIState == null && DefaultState != null) AIState = DefaultState;
		}

		public void Update() {
			if (AIState == DefaultState && timeToIncreaseHostility) {
				AIState        = AlertedState;
				eyes.LOSlength = 0;
			}
			else if (AIState == AlertedState && timeToDecreaseHostility) {
				AIState        = DefaultState;
				eyes.LOSlength = eyes.relativePositionOfPlayer.magnitude;
			}
			else if (AIState == AlertedState && timeToIncreaseHostility) {
				AIState        = HostileState;
				eyes.LOSlength = 0;
			}
			else if (AIState == HostileState && timeToDecreaseHostility) {
				AIState        = AlertedState;
				eyes.LOSlength = eyes.relativePositionOfPlayer.magnitude;
			}
		}
	}
}