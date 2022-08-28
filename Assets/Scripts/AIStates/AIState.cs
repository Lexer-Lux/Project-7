using System;
using BodyParts.Mob;
using Entities;
using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

namespace AIStates {
	[HideMonoScript, RequireComponent(typeof(Entity))] public abstract class AIState : MonoBehaviour {
		private         Eyes  eyes             => gameObject.GetComponent<Eyes>();
		public abstract Color Color            { get; }
		public abstract float AttentionModifier{ get; }
	}

	[DisallowMultipleComponent] public abstract class Patrolling : AIState {
		public override Color Color             => new Color(0.51f, 0.09f, 1f);
		public override float AttentionModifier => 1f;
	}

	[DisallowMultipleComponent] public abstract class Searching : AIState {
		public override Color Color             => Color.blue;
		public override float AttentionModifier => 2f;
	}

	[DisallowMultipleComponent] public abstract class Attacking : AIState {
		public override Color Color             => Color.red;
		public override float AttentionModifier => 3f;
	}
}