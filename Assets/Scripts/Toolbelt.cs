using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class Toolbelt : MonoBehaviour {
    public Pouch[] pouches = { null, null, null, null };

}
public sealed class Pouch : MonoBehaviour {
    public enum sizes { S = 1, M = 2, L = 3 };
    public uint            size;
    public IFitsInToolbelt item;
    public uint            AmountHeld;
    public uint            MaxAmountHeld => item.maxHeld * size;
    public void Start() {
        size = (uint)sizes.S;
    }
}
