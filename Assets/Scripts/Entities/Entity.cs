using Body_Parts;
using Body_Parts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using static GMST;
using UnityEngine.Events;
#nullable enable

public abstract class Entity : MonoBehaviour {
	public int    AttributeMax = 12;

	public                                 int  SKL => SkeletalSystem.Level;
	[ShowIf("CirculatorySystem")]   public int? CRC => CirculatorySystem.Level;
	[ShowIf("NervousSystem")]       public int? NRV => NervousSystem.Level;
	[ShowIf("MuscularSystem")]      public int? MSC => MuscularSystem.Level;
	[ShowIf("IntegumentarySystem")] public int? INT => IntegumentarySystem.Level;

	public CirculatorySystem   CirculatorySystem   => gameObject.GetComponent<CirculatorySystem>();
	public SkeletalSystem      SkeletalSystem      => gameObject.GetComponent<SkeletalSystem>();
	public NervousSystem       NervousSystem       => gameObject.GetComponent<NervousSystem>();
	public MuscularSystem      MuscularSystem      => gameObject.GetComponent<MuscularSystem>();
	public IntegumentarySystem IntegumentarySystem => gameObject.GetComponent<IntegumentarySystem>();
	
	public Rigidbody RigidBody => gameObject.GetComponent<Rigidbody>();
	public ILegs     Legs      => gameObject.GetComponent<ILegs>();
	public Collider  Collider  => gameObject.GetComponent<Collider>();
	public bool      WeakToSilver;


	[HideInInspector] public UnityEvent eDie = new UnityEvent();
	protected virtual void Start() {
	}
	protected virtual void Update() {
		InvoluntaryActions();
	}
	protected virtual void VoluntaryActions()   { }
	protected virtual void InvoluntaryActions() { }
	protected virtual void Die()                { }
}