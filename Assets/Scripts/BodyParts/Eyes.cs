using System;
using System.Numerics;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using static PlayerCharacter;
using Vector3 = UnityEngine.Vector3;

namespace Body_Parts {
	/// <summary> * The eyes of an enemy. </summary>
	// TODO: Make some farsighted enemies.
	[RequireComponent(typeof(Enemy))] public class Eyes : MonoBehaviour {
		/// <summary> The one whose eyes these are. </summary>
		private Enemy Owner => gameObject.GetComponent<Enemy>();

		#region Stats
		/// <summary> The distance from which I can see you, in m. </summary>
		[SuffixLabel("m")] public uint Distance = 10;
		// TODO: Make this into a Vector2 so it can be a range. Farsighted and nearsighted enemies. Edit it easily in Odin.
		/// <summary> The base rate at which my LOS grows, in m/s. </summary>
		[SuffixLabel("m/s")] public float Acuity = 5;
		/// <summary> Horizontal with of my field of view, in degrees. </summary>
		[SuffixLabel("°"), Range(1, 360)] public ushort FOV = 180;
		/// <summary> How much my line of sight retreats when I can't see you, in m/s. </summary>
		[SuffixLabel("m/s")] public float ForgettingRate = 1;
		#endregion

		#region Explanatory
		/// <summary> * Can I see you? </summary>
		public bool seeingPlayer => isPlayerWithinVisualRange && isPlayerWithinFieldOfView && !isPlayerOccluded;
		/// <summary> * Have I seen you long enough to recognize it's -- hey, get her! </summary>
		public bool playerIdentified => LOSlength >= relativePositionOfPlayer.magnitude;
		public  Vector3         lineOfSightVector;
		private PlayerCharacter target          => PC;
		private Vector3         positionToCheck => target.headTransform.position;
		private bool isPlayerOccluded{
			get {
				int        layerMask = LayerMask.GetMask("Default");
				return Physics.Raycast(Owner.transform.position, positionToCheck, out RaycastHit _,
					relativePositionOfPlayer.magnitude, layerMask);
			}
		}
		private bool isPlayerWithinVisualRange => Distance >= relativePositionOfPlayer.magnitude;
		/// <summary> The rate at which my LOS grows, adjusted for my awareness. </summary>
		private float ModifiedAcuity{
			get {
				int   attentionModifier;
				if (Owner.state is Patrolling) { attentionModifier     = 1; }
				else if (Owner.state is Searching) { attentionModifier = 2; }
				else if (Owner.state is Attacking) { attentionModifier = 3; }
				else {
					throw new ArgumentException(
						"What am I doing? Patrolling? Searching? Attacking? I can't see you any harder than 3!");
				}

				return Acuity * attentionModifier * PC.Visibility + PC.RigidBody.velocity.magnitude;
			}
		}

		/// <summary> * Is the angle of the player relative to where I'm facing within my cone of vision? </summary>
		private bool isPlayerWithinFieldOfView => (leftBound <= angleToPlayer) && (angleToPlayer <= rightBound);
		/// <summary> *  How close am I to identifying you? This is the length of my sight indicator. </summary>
		private float _LOSlength;
		public float LOSlength{
			get => _LOSlength;
			set {
				_LOSlength = Mathf.Clamp(value, 0, relativePositionOfPlayer.magnitude + 0.01f);
			} // ! For some fucking reason, if I don't add this tiny bit, (LOSlength >= relativePositionOfPlayer.magnitude) will forever be false.
		}

		private Vector3 lastSeenPosition;
		// ? Why do I have to use relativePostitionOfPlayer? Shouldn't it be this.transform.position instead?
		private float angleToPlayer => Vector3.Angle(relativePositionOfPlayer, Owner.transform.forward);
		/// <summary> * The left and right bounds of my FOV cone, respectively. </summary>
		private float leftBound => (FOV / 2f) * -1;
		private float rightBound => FOV / 2f;
		public Vector3 relativePositionOfPlayer =>
			positionToCheck - Owner.transform.position; // * Your relative position to me.
		#endregion

		#region Indicator
		private Line line => gameObject.GetComponent<Line>();
		private Color newColor{
			get {
				Color color = Owner.state.color;
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
				LOSlength +=
					ModifiedAcuity * Time
						.deltaTime; // * Don't worry if it goes above the actual distance to the player; that's what setters are for.
				lineOfSightVector = Owner.transform.position +
					Vector3.ClampMagnitude(relativePositionOfPlayer, LOSlength);
				lastSeenPosition = relativePositionOfPlayer;
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