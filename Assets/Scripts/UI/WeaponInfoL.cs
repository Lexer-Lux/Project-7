using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HUD;

public class WeaponInfoL : MonoBehaviour {
    protected Object Icon;
    protected PlayerCharacter subject;
    void Start() {
        subject = activeHUD.Subject;
        subject.eWeaponEquip.AddListener(Render);
        Render();
    }
    public void Render() {
    }
}
