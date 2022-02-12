using System;
using System.Collections.Generic;
using DefaultNamespace;
using Rounds;
using Sirenix.OdinInspector;
using UnityEngine;

public class Round : MonoBehaviour, IHammerspaceable {
	public Cartridge.Strengths Strength;
	public Color               Color     => Bullet.Color;
	public Cartridge           Cartridge => Casing.Cartridge;
	public Casing              Casing    => GetComponentInChildren<Casing>();
	public Bullet              Bullet    => GetComponentInChildren<Bullet>();

	[ShowInInspector] public bool InHammerspace{ get; private set; }

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

	public void Start() {
		if (transform.parent == null) ExitHammerspace();
		if (transform.parent != null) EnterHammerspace();
	}

	[Button] public void EnterHammerspace() {
		Bullet.HammerspaceToggler.ToggleHammerspace(true);
		Casing.HammerspaceToggler.ToggleHammerspace(true);
		InHammerspace = true;
	}
	[Button] public void ExitHammerspace() {
		Bullet.HammerspaceToggler.ToggleHammerspace(false);
		Casing.HammerspaceToggler.ToggleHammerspace(false);
		InHammerspace = false;
		transform.SetParent(null, true);
	}

	private float powderVelocityModifier{
		get {
			switch (Strength) {
				case Cartridge.Strengths.Subsonic:  return Cartridge.SubsonicSpeedMalus;
				case Cartridge.Strengths.Regular:   return 1;
				case Cartridge.Strengths.HighPower: return Cartridge.OverPressureSpeedBonus;
				default:                            throw new Exception();
			}
		}
	}

	[Button] public void StrikePrimer(float BaseVelocity) {
		if (InHammerspace) Debug.LogError("You should have known better than to set off explosions in hammerspace.");

		Bullet.GetComponent<Rigidbody>().velocity = Bullet.transform.forward * BaseVelocity * powderVelocityModifier;
		Bullet.transform.SetParent(null, true);
		Casing.transform.SetParent(null, true);
		Destroy(this);
	}
}