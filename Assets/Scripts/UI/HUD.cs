using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerCharacter;

public class HUD : MonoBehaviour {
    public static HUD activeHUD;
    public Bar HealthBar;
    public Bar MaxHealthBar;
    //public WeaponInfoL weaponInfoL;
    public PlayerCharacter Subject = PC;
    //TODO: Scale HUD to screen size...somehow.
    public HUD() => activeHUD = this;
    void Start() {
        //Subject.eDamageTaken.AddListener(HealthBar.Render);
        //Subject.eMaxHealthChange.AddListener(MaxHealthBar.Render); TODO;
    }
    void Update() {
        transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y - 3, transform.position.z);
    }
}
