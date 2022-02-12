using System.Collections.Generic;
using UnityEngine;

namespace Body_Parts {
	public interface ILegs {
		public enum MovementModes { Crouch, Walk, Run, Airborne };
		public static readonly Dictionary<MovementModes, Color> MovementModeColors = new Dictionary<MovementModes, Color>() {
			{MovementModes.Crouch, Color.blue},
			{MovementModes.Walk, new Color(0f, 0.4f, 1f)},
			{MovementModes.Run, new Color(0f, 0.85f, 1f)},
			{MovementModes.Airborne, new Color(0f, 1f, 0.69f)}
		};

		MovementModes MovementMode{ get; }
		float         MaxSpeed    { get; }
		float         SneakSpeed  { get; }
		float         WalkSpeed   { get; }
		float         RunSpeed    { get; }
	}
}