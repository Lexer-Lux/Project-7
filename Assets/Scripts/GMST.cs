using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMST : MonoBehaviour {
    //* Original walkSpeedPerNRV = 0.75, sneakSpeedPerCTV = 0.5


    // FIXME: A momentum of 50 should acccelerate our 50kg character by 1m/s/s, but instead it takes 6.21sec to accelerate from 0-1.6m/s. What's going on? Original values were 50 + 10*CTV.

    public static float velocityCutoff     = 0.25f;
    public static int   BloodPerSKL        = 4;
    public static int   OxygenPerSKL       = 4;
    public static float StaminaRegenPerNRV = 1;

    public static float DebuffResistanceModifier    = 0.5f;
    public static float DebuffVulnerabilityModifier = 2.0f;

}
