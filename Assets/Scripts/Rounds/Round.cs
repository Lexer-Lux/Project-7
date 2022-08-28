using System;
using System.Collections.Generic;
using DefaultNamespace;
using GunParts;
using Interfaces;
using Rounds;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

[HideMonoScript, DisallowMultipleComponent, ExecuteAlways] public class Round : MonoBehaviour, IHammerspaceable {
	public Cartridge.Strengths Strength;
	public Color               Color     => Bullet.Color;
	public Cartridge           Cartridge => Casing.Cartridge;
	public Casing              Casing    => GetComponentInChildren<Casing>();
	public Bullet              Bullet    => GetComponentInChildren<Bullet>();

	[ShowInInspector] public bool HammerspaceState{ get; private set; }

	public static Round HandLoad(Cartridge.Strengths Powder, Cartridge Cartridge, Bullet aoeu) {
		GameObject newGO  = new GameObject();
		var        casing = Instantiate(Cartridge.CasingPrefab, newGO.transform, true);
		var        bullet = Instantiate(Cartridge.BulletPrefab, newGO.transform);
		bullet.transform.position = casing.GetComponent<Rounds.Casing>().CrimpPoint.transform.position;
		newGO.AddComponent<Round>();
		newGO.GetComponent<Round>().Strength = Powder;
		newGO.name                           = $"{Cartridge.name} X {Cartridge.PowerNames[Powder]}";
		return newGO.GetComponent<Round>();
	}

	public void ToggleHammerspace(bool State) {
		Bullet.HammerspaceToggler.ToggleHammerspace(State);
		Casing.HammerspaceToggler.ToggleHammerspace(State);
		HammerspaceState = State;
	}

	private float powderVelocityModifier{
		get {
			return Strength switch {
				Cartridge.Strengths.Subsonic => Cartridge.SubsonicSpeedMalus, Cartridge.Strengths.Regular => 1, Cartridge.Strengths.HighPower => Cartridge.OverPressureSpeedBonus, _ => throw new Exception()
			};
		}
	}
	
	public int Recoil{
		get {
			switch (Strength) {
				case Rounds.Cartridge.Strengths.Regular:   return Cartridge.Recoil;
				case Rounds.Cartridge.Strengths.Subsonic:  return Cartridge.Recoil + Cartridge.SubsonicRecoilDecrease;
				case Rounds.Cartridge.Strengths.HighPower: return Cartridge.Recoil + Cartridge.OverPressureRecoilIncrease;
				default: Debug.LogException(new ArgumentException("Invalid powder load."));
					return 0;
			}
		}
	}

	[Button] public void StrikePrimer(float MuzzleVelocity) {
		if (HammerspaceState) Debug.LogError("You should have known better than to set off explosions in hammerspace.");

		Bullet.GetComponent<Rigidbody>().velocity = Bullet.transform.right * MuzzleVelocity * powderVelocityModifier;
		eOnFire.Invoke();

		Bullet.transform.SetParent(null, true);
		Casing.transform.SetParent(null, true);
		Destroy(this);
	}
	public UnityEvent eOnFire;
	public void       Start() => eOnFire = new();
}