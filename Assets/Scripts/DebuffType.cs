using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu] public class DebuffType : ScriptableObject {
	public enum VulnerabilityValues { Weak, Normal, Resistant, Immune}
	public static Dictionary<VulnerabilityValues, float> VulnerabilityModifiers => new Dictionary<VulnerabilityValues, float>() {
		{VulnerabilityValues.Weak, 2.0f}, {VulnerabilityValues.Normal, 1f}, {VulnerabilityValues.Resistant, 0.5f},
		{VulnerabilityValues.Immune, 0f}
	};

	public static List<DebuffType> DebuffTypes => Resources.LoadAll<DebuffType>("Debuff Types").ToList();
	public        int              buildupPerStack = 12;
	public        int              RecoveryRate;
	public Sprite Sprite;
	public Color  Color;
}