using Sirenix.OdinInspector;
using UnityEngine;

namespace DefaultNamespace {
	public interface IHammerspaceable {
		GameObject gameObject   { get; }
		bool       InHammerspace{ get; }
		void       EnterHammerspace();
		void       ExitHammerspace();
	}
}