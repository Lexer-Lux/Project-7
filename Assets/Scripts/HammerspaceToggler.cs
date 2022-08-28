using Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody)), HideMonoScript, DisallowMultipleComponent] public class HammerspaceToggler : MonoBehaviour, IHammerspaceable {
	private Collider  Collider        => GetComponent<Collider>();
	private Rigidbody RigidBody       => GetComponent<Rigidbody>();
	private Renderer  Renderer        => GetComponent<Renderer>();
	public  bool      HammerspaceState{ get; private set; }

	public void ToggleHammerspace(bool State) {
		HammerspaceState      = State;
		Renderer.enabled      = !State;
		Collider.enabled      = !State;
		RigidBody.isKinematic = State;
	}
		
}