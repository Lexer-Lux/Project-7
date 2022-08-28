using System.Collections.Generic;
using UnityEngine;

namespace BodyParts {
	[RequireComponent(typeof(Animator))] public abstract class Legs : MonoBehaviour {
		protected Animator animator => GetComponent<Animator>();
		public enum MovementModes { Crouch, Walk, Run, Airborne };
		public static readonly Dictionary<MovementModes, Color> MovementModeColors = new Dictionary<MovementModes, Color>() {{MovementModes.Crouch, Color.blue}, {MovementModes.Walk, new Color(0f, 0.4f, 1f)}, {MovementModes.Run, new Color(0f, 0.85f, 1f)}, {MovementModes.Airborne, new Color(0f, 1f, 0.69f)}};

		protected virtual void setSneakPlaybackSpeed() => animator.SetFloat("Sneak Playback Speed", 1);
		protected virtual void setWalkPlaybackSpeed()  => animator.SetFloat("Walk Playback Speed", 1);
		protected virtual void setRunPlaybackSpeed()   => animator.SetFloat("Run Playback Speed", 1);

		public abstract bool    DownHeld{ set; }
		public abstract bool    UpHeld  { set; }
		public abstract Vector2 Movement{ set; }
		
		public virtual  MovementModes MovementMode{ get; set; }
		float                         MaxSpeed    { get; }
		float                         SneakSpeed  { get; }
		float                         WalkSpeed   { get; }
		float                         RunSpeed    { get; }

		public virtual void Start() {
			setSneakPlaybackSpeed();
			setWalkPlaybackSpeed();
			setRunPlaybackSpeed();
		}
	}
}