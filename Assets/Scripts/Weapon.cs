using Sirenix.OdinInspector;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public Rigidbody RigidBody => GetComponent<Rigidbody>();
	[Required] public Rigidbody     Grip; // The joint on the hand will be attached to this rigibody, so you won't be holding your gun by its registration point but rather its actual grip.
}