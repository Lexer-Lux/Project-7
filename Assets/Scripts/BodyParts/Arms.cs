using Sirenix.OdinInspector;
using UnityEngine;

namespace BodyParts {
	[HideMonoScript, DisallowMultipleComponent] public abstract class Arms : SerializedMonoBehaviour {
		public abstract bool LeftHandPressed { set; }
		public abstract bool LeftArmPressed  { set; }
		public abstract bool RightHandPressed{ set; }
		public abstract bool RightArmPressed { set; }
	}
}