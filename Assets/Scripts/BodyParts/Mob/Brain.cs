using System;
using System.Collections.Generic;
using System.ComponentModel;
using AIStates;
using Entities;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

#nullable enable

namespace BodyParts.Mob {
	[HideMonoScript, RequireComponent(typeof(Entity), typeof(Eyes))] public class Brain : MonoBehaviour {
		private Eyes  eyes      => gameObject.GetComponent<Eyes>();
		public  float Awareness => AIState.AttentionModifier;
		[ShowInInspector, Sirenix.OdinInspector.ReadOnly]
		public AIState AIState{
			get => _AIState;
			set {
				if (value == _AIState) return;
				if (AIState == value || value.enabled) Debug.LogWarning("AI script is already enabled or flagged as active!");
				else if (value != DefaultState && value != AlertedState && value != HostileState) Debug.LogWarning("Not a valid AI state!");
				else {
					AIState.enabled = false;
					value.enabled = true;
					AIState       = value;
					Mood          = AIState.Color;
				}
			}
		}
		private AIState _AIState;
		public  Color   Mood;

		private AIState DefaultState => gameObject.GetComponent<Patrolling>();
		private AIState AlertedState => gameObject.GetComponent<Searching>();
		private AIState HostileState => gameObject.GetComponent<Attacking>();

		private bool timeToDecreaseHostility => !eyes.seeingPlayer && eyes.LOSlength == 0;
		private bool timeToIncreaseHostility => eyes.seeingPlayer && eyes.playerIdentified;

		public void Start() {
			if (AIState != null || DefaultState == null || AlertedState == null || HostileState == null) throw new Exception("Brain is not properly configured");
			AIState = DefaultState;
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