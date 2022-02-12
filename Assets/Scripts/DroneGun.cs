using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class DroneGun : RangedWeapon {
    public static string Resource = "Drone Gun";
    new private uint[] damageUpgradeValues = { 0, 2, 4, 6, 8 }; // TODO: Unity does noto like this.
    protected sealed override uint[] clipSizes => new uint[] { 5, 10 };
    protected override uint reloadMode => (uint)reloadModes.Round;
    protected override uint firingType => (uint)FiringTypes.Automatic;
    protected override uint[] speedValues => new uint[] { 60, 120 };
    public override Type AmmoType => typeof(P303);
    public override string Icon => "Drone Gun Sprite";
    protected override void Start() {
        base.Start();
        infiniteAmmo = true;
    }
}
public class P303 { }