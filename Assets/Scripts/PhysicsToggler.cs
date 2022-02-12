using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody)), HideMonoScript, DisallowMultipleComponent] public class PhysicsToggler : MonoBehaviour {
	private Collider  Collider  => GetComponent<Collider>();
	private Rigidbody RigidBody => GetComponent<Rigidbody>();
	public  bool      PhysicsState;
		
	public void SetPhysics(bool State) {
		PhysicsState          = State;
		Collider.enabled      = State;
		RigidBody.isKinematic = !State;
	}
		
}