using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.UI;
using UnityEngine.Events;

// TODO: Implement some interface or class that grants weight so we can have a stamina cost.
// TODO: Implement DPS and DPS + R;
public abstract class Weapon : MonoBehaviour {
    public abstract string Icon { get; }
    protected readonly Entity owner;
    protected abstract uint damage { get; }
    protected uint damageUpgrades = 0;
    protected uint[] damageUpgradeValues = { 0 };
    protected uint damageUpgradeBonus => damageUpgradeValues[damageUpgrades];
    public Transform connectionPoint; //TODO
    public Transform gripPoint;

    public abstract void tap();
    public abstract void doubleTap();
    public abstract void hold();
}
public abstract class RangedWeapon : Weapon {
    // TODO: I had this cool idea where you would take up to as many shells as you can hold (dual and quad loading techs for higher gun skill) as required to totally fill your mag, and they would be removed from your bag. If you get staggered while in the reloading animation, they drop to the ground, or maybe even go flying, so you have to pick them back up. That would be cool!
    // * Misc.
    public          Transform  barrelPosition;
    public          Rigidbody  rigidBody;
    public abstract Type       AmmoType{ get; } //TODO: Can I cast/limit this to type Ammo right here?
    protected       Type       CasingType;
    protected       bool       roundChambered => timeUntilNextShot == 0 && (infiniteAmmo || AmmoInClip > 0);
    public          UnityEvent eAmmoChange;

    // * Mods

    // * Reloading
    protected enum reloadModes { Round, Clip };
    protected abstract uint reloadMode { get; }

    // * Damage
    override protected sealed uint damage { get => (uint)AmmoType.GetField("Damage").GetValue(null) + damageUpgradeBonus; }

    // * UsesClips
    protected uint clipUpgradeLevel;
    protected abstract uint[] clipSizes { get; }
    public uint ClipSize { get => clipSizes[clipUpgradeLevel]; }
    [SerializeField] protected uint _AmmoInClip;
    public uint AmmoInClip {
        get => _AmmoInClip;
        set {
            if (infiniteAmmo && value < AmmoInClip) return; // * Ammo should not go down if you have infinite ammo.
            _AmmoInClip = value;
            eAmmoChange?.Invoke();
        }
    }
    protected bool infiniteAmmo = false;

    // * StrikeHammer rate
    public enum FiringTypes { Manual, SemiAutomatic, BurstFire, Automatic }
    protected abstract uint firingType { get; }
    protected uint speedUpgradelevel;
    protected abstract uint[] speedValues { get; } // * In rounds per second.
    protected float timeBetweenShots {
        get {
            switch (firingType) {
                case (uint)FiringTypes.Manual: // ! If only we had C# 9 in unity, I could simply say "case this or that". Many such cases!
                    throw new ArgumentException("Manual action firearms have not been implemented yet due to our groundbreaking new assault rifles ban. Please hand in your prohibited-class firearm immediately. ~ J.T."); //TODO: Implement manuals with firerate based on gun skill.
                case (uint)FiringTypes.Automatic:
                    return 1 / speedValues[speedUpgradelevel];
                case (uint)FiringTypes.SemiAutomatic:
                    return 0;
                case (uint)FiringTypes.BurstFire:
                    throw new ArgumentException("Burst-fire weapons are not implemented yet. You shouldn't even be able to get one so please hand it in. We're not mad at you or anything hahaha it's ok  ~ ATF"); //TODO: Implement burst-fire.
                default:
                    throw new ArgumentException("Your gun seems to have an illegal firing mode, wow! Can we come check it out? It sounds really cool haha ~ Not ATF");
            }
        }
    }

    protected float _timeUntilNextShot;
    protected float timeUntilNextShot { get => _timeUntilNextShot; set => _timeUntilNextShot = Mathf.Clamp(value, 0, timeBetweenShots); } //TODO: Try without helper var.
    protected virtual void Start() {
        rigidBody = GetComponent<Rigidbody>();
        eAmmoChange = new UnityEvent();
    }

    protected void Update() {
        timeUntilNextShot -= Time.deltaTime;
    }
    public sealed override void tap() {
        if (roundChambered) {
            timeUntilNextShot = timeBetweenShots;
            if (!infiniteAmmo) AmmoInClip --;
            // TODO: Projectile generation code.
            // TODO: Noise creation code.
            // TODO: Casing creation code.
        }
    }
    public sealed override void doubleTap() {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }
    public sealed override void hold() {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }
}

public abstract class MeleeWeapon: Weapon {
    protected sealed override uint damage       => (uint) (damageFormula + damageUpgradeBonus);
    protected abstract        int  damageFormula{ get; }
    public sealed override void doubleTap() {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }
    public sealed override void hold() {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }
    public sealed override void tap() {
        Debug.Log(MethodBase.GetCurrentMethod().Name);
    }
}

public sealed class DroneBrassKnuckles : MeleeWeapon {
    public override string Icon { get => "Drone Gun Sprite"; }
    //protected sealed override int damageFormula => owner.Stats.MSC + 3; TODO
    protected sealed override int damageFormula => 3;
    new private uint[] damageUpgradeValues = { 0, 2, 4, 6 };
}



