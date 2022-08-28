using System;
using AIStates;
using Entities;
using JetBrains.Annotations;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using static PlayerCharacter;
using Vector3 = UnityEngine.Vector3;

#nullable enable

namespace BodyParts.Mob {
	/// <summary> The eyes of an enemy. </summary>
	[DisallowMultipleComponent, RequireComponent(typeof(Entity))] public sealed class Eyes : MonoBehaviour {
		/// <summary> Whose eyes are these? </summary>
		private Entity Owner => gameObject.GetComponent<Entity>();
		[NotNull] private PlayerCharacter target => PC;

		#region Stats
		/// <summary> The distance from which I can see you, in m. </summary>
		[SuffixLabel("m"), Range(1, 25), PropertyTooltip("The distance from which I can see you, in m.")] public ushort Distance = 10;
		/// <summary> The base rate of how quickly my line of sight will move towards you, in m/s. </summary>
		[SuffixLabel("m/s"), Range(0, 10), PropertyTooltip("How quickly I'll notice you. The base rate of how quickly my line of sight will move towards you, in m/s.")] public float Acuity = 5;
		/// <summary> Horizontal width of my field of view, in degrees. </summary>
		[SuffixLabel("°"), Range(1, 360)] public ushort FOV = 180;
		/// <summary> How quickly my line of sight retreats when I can't see you, in m/s. </summary>
		[SuffixLabel("m/s"), Range(0, 10), PropertyTooltip("How quickly my line of sight retreats when I can't see you, in m/s.")] public float ForgettingRate = 1;
		#endregion

		#region Explanatory
		/// <summary> * Can I see you? </summary>
		public bool seeingPlayer => isPlayerWithinVisualRange && isPlayerWithinFieldOfView && !isPlayerOccluded;
		/// <summary> * Have I seen you long enough to recognize it's -- hey, get her! </summary>
		public bool playerIdentified => LOSlength >= relativePositionOfPlayer.magnitude;
		/// <summary> The physical line travelling from me to you, my enemy, growing the longer I can see you. </summary>
		private  Vector3         lineOfSightVector;
		private Vector3         positionToCheck => target.headTransform.position;
		private bool isPlayerOccluded{
			get {
				int layerMask = LayerMask.GetMask("Default");
				return Physics.Raycast(Owner.transform.position, positionToCheck, out RaycastHit _, relativePositionOfPlayer.magnitude, layerMask);
			}
		}
		private bool  isPlayerWithinVisualRange => Distance >= relativePositionOfPlayer.magnitude;
		/// <summary> The rate at which my LOS grows, adjusted for my awareness. </summary>
		private float ModifiedAcuity => Acuity * Brai * PC.Visibility + PC.RigidBody.velocity.magnitude;
		/// <summary> * Is the angle of the player relative to where I'm facing within my cone of vision? </summary>
		private bool isPlayerWithinFieldOfView => (leftBound <= angleToPlayer) && (angleToPlayer <= rightBound);
		/// <summary> *  How close am I to identifying you? This is the length of my sight indicator. </summary>
		private float _LOSlength;
		public float LOSlength{
			get => _LOSlength;
			// ! For some fucking reason, if I don't add this tiny bit, (LOSlength >= relativePositionOfPlayer.magnitude) will forever be false.
			set => _LOSlength = Mathf.Clamp(value, 0, relativePositionOfPlayer.magnitude + 0.01f); 
		}
		private Vector3 lastSeenPosition;
		// ? Why do I have to use relativePostitionOfPlayer? Shouldn't it be this.transform.position instead?
		private float angleToPlayer => Vector3.Angle(relativePositionOfPlayer, Owner.transform.forward);
		/// <summary> * The left and right bounds of my FOV cone, respectively. </summary>
		private float leftBound => (FOV / 2f) * -1;
		private float   rightBound               => FOV / 2f;
		public  Vector3 relativePositionOfPlayer => positionToCheck - Owner.transform.position;
		#endregion

		#region Indicator
		private Line line => gameObject.GetComponent<Line>();
		private Color newColor{
			get {
				Color color = state.Color;
				color.a = target.Visibility;
				return color;
			}
		}
		#endregion

		public void Start() {
			gameObject.AddComponent<Line>();
			line.Dashed   = true;
			line.DashSnap = DashSnapping.Off;
		}

		public void Update() {
			if (seeingPlayer) {
				LOSlength         += ModifiedAcuity * Time.deltaTime; // * Don't worry if it goes above the actual distance to the player; that's what setters are for.
				lineOfSightVector =  Owner.transform.position + Vector3.ClampMagnitude(relativePositionOfPlayer, LOSlength);
				lastSeenPosition  =  relativePositionOfPlayer;
			}
			else {
				//if (lineOfSightVector == null) return;
				LOSlength         -= ForgettingRate * Time.deltaTime; // * Don't worry if it goes below, either.
				lineOfSightVector =  Owner.transform.position + Vector3.ClampMagnitude(lastSeenPosition, LOSlength);
			}

			line.Dashed = !seeingPlayer;
			line.Color  = newColor;
			line.Start  = transform.InverseTransformPoint(gameObject.transform.position);
			line.End    = transform.InverseTransformPoint(lineOfSightVector);
		}
	}
}