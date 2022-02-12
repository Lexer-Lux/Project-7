using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HUD;
using static PlayerCharacter;

public class ToolbeltUI : MonoBehaviour {
    public PouchUI Pouch1;
    private PlayerCharacter subject;
    public void Start() {
        subject = activeHUD.Subject;
        subject.controls.Player.Toolbelt.performed += context => { gameObject.SetActive(true); };
        subject.controls.Player.Toolbelt.canceled += context => { gameObject.SetActive(false); };
    }
    public void Update() {
        Pouch1.myPouch = PC.myToolbelt.pouches[0];
    }
}
