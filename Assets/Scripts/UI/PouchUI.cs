using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PouchUI : MonoBehaviour {
    public Pouch myPouch;
    [SerializeField] protected Text CurrentAmountText;
    [SerializeField] protected Text MaxAmountText;
    void Start() {
    }
    void Update() {
        CurrentAmountText.text = "" + myPouch?.AmountHeld + "";
        MaxAmountText.text = "" + myPouch?.MaxAmountHeld + "";
    }
}
