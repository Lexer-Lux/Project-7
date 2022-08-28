using System;
using Sirenix.OdinInspector;
using UnityEngine;

#nullable enable

public class Holster : MonoBehaviour {
	private ConfigurableJoint joint => GetComponent<ConfigurableJoint>() ?? throw new Exception();
	[ShowInInspector, SceneObjectsOnly]
	public Weapon? Item{
		get => _item;
		set {
			if (_item == value) return;
			else if (value != null && _item != null) throw new Exception("Can't fit two weapons in a holster!");
			else if (value != null && _item == null) {
				value.RigidBody.isKinematic = false;
				value.RigidBody.useGravity  = true;
				joint.connectedBody         = value.RigidBody;
				_item                       = value;
			}
			else if (value == null && _item != null) {
				joint.connectedBody = null;
				_item               = value;
			}
			else throw new Exception("Whoops. Didn't anticipate this one.");
		}
	}
	[SerializeField, HideInInspector] private Weapon? _item;
}