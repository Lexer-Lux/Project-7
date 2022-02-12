using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Check that Yanderedev video that talks about how to do this properly.
public abstract class InteractiveObject : MonoBehaviour {
    const ushort INTERACTIONDISTANCE = 1;
    public bool Enabled;
    // * If hasUI: open. If hasUI and is open and player is not close enough: Close.
    protected virtual void Interact() {}
    protected virtual void Update() {}

}