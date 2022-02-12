using System.Security.AccessControl;
using System.Net.Http.Headers;
using System;
using Body_Parts;
using Body_Parts.Systems;
using UnityEngine;
using static GMST;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static HUD;
using static Body_Parts.ILegs;

// TODO: On game start, you make it clear you're depersonalized, feeling not in control of yourself, and have a distinct feeling of being watched.. Upon losing input: "You feel even more alone than before." DRONE: "Hello? What do I do now?" [Walk back to bed.]
public class PlayerCharacter : Entity {
    public static PlayerCharacter PC;

    #region Making attribute values explicit.
    public new int CRC => (int) base.CRC;
    public new int SKL => base.SKL;
    public new int NRV => (int) base.NRV;
    public new int MSC => (int) base.MSC;
    public new int INT => (int) base.INT;
    #endregion

    public new PlayerLegs Legs => base.Legs as PlayerLegs;


    public                   GameObject LeftHand; //TODO: Set in code, then hide in inspector.
    public                   Transform  headTransform; //TODO: Set in code, then hide in inspector.
    [HideInInspector] public GameObject LHolster;
    public Animator   animator => gameObject.GetComponent<Animator>();
    // * EVENTS
    [HideInInspector] public UnityEvent eWeaponEquip;
    // * CONTROLS
    public CustomInputAction controls;
    public bool crouch;
    public bool upTap;
    public bool upHold;
    public bool leftHandTap;
    public bool leftHandHold;
    protected InputAction movement;
    
    // * MISC.
    [HideInInspector] public Toolbelt myToolbelt;
    protected Weapon _LeftWeapon;
    public Weapon LeftWeapon {
        get => _LeftWeapon;
        set {
            _LeftWeapon = value;
            eWeaponEquip.Invoke();
        }
    }
    public    float Visibility => (Legs.MovementMode == MovementModes.Crouch) ? 0.5f : 1f;
    protected bool  canTurn   => Legs.MovementMode != MovementModes.Airborne;

    protected void Awake() {
        controls = new CustomInputAction();
    }
    protected override void Start() {
        PC = this;
        base.Start();
        LHolster = transform.Find("Drone Leg L").Find("Thigh B").Find("Calf B").Find("L Holster").gameObject;
        LeftWeapon = Loader.Load(typeof(DroneGun)) as Weapon;
        LeftWeapon.transform.SetParent(LeftHand.transform, false);
        holsterGun();
        // Cursor.lockState = CursorLockMode.Confined;
        // * Test toolbelt stuff.
        myToolbelt = gameObject.AddComponent<Toolbelt>();
        myToolbelt.pouches[0] = myToolbelt.gameObject.AddComponent<Pouch>();
        // myToolbelt.pouches[0].item = new P303BritishFMJRound(); // FIXME: Can't instantiate a Mono.
        myToolbelt.pouches[0].AmountHeld = 10;
        // * Enable events.
        eWeaponEquip = new UnityEvent();

        activeHUD.gameObject.SetActive(true);
    }
    protected void OnEnable() {
        movement = controls.Player.Move;
        movement.Enable();
        controls.Player.Down.started += context => { crouch = true; };
        controls.Player.Down.canceled += context => { crouch = false; };
        controls.Player.Down.Enable();
        controls.Player.UpHold.Enable();
        controls.Player.UpTap.Enable();
        controls.Player.LeftHandTap.Enable();
        controls.Player.LeftHandHold.Enable();
        controls.Player.Toolbelt.Enable();
    }
    protected void faceMouse() {
        //TODO: Implement smooth rotation using RotationTowards.
        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        cameraPosition.y = transform.position.y;
        Vector3 relativePosition = transform.position - cameraPosition;
        float angle = Vector3.SignedAngle(relativePosition, Vector3.back, Vector3.up);
        transform.rotation = Quaternion.Euler(90f, 0f, angle);
    }
    protected override void Update() {
        base.Update();
        faceMouse();
        // * Animator values
        animator.SetFloat("velocity", RigidBody.velocity.magnitude);
        animator.SetBool("Crouch", crouch);
        animator.SetBool("Legs: Tap", upTap);
        animator.SetBool("Left Hand: Hold", leftHandHold);
        // * Control control
        upTap = controls.Player.UpTap.triggered;
        if (controls.Player.UpHold.triggered) upHold = true;
        if (upHold && !controls.Player.UpHold.IsPressed()) upHold = false;
        leftHandTap = controls.Player.LeftHandTap.triggered;
        if (controls.Player.LeftHandHold.triggered) leftHandHold = true;
        if (leftHandHold && !controls.Player.LeftHandHold.IsPressed()) leftHandHold = false;
    }
    protected void roll() {
        //TODO: Face forward while rolling and walking.
        Vector3 rollVelocity = RigidBody.velocity.normalized * 10;
        RigidBody.velocity = rollVelocity;
    }
    protected void jump() {
        print("jumping");
        throw new Exception("Jumping occasionally crashes all of unity for some reason. So it's disabled for now.");
        RigidBody.AddForce(new Vector3(0, 25, 0), ForceMode.VelocityChange);
    }
    protected void FixedUpdate() {
        // TODO: Fix diagonal movement when pressing one direction and letting go while holding another. THis is gonna be a tough one.
        // * Apply movement force.
        Vector2 movementVector = movement.ReadValue<Vector2>();
        Vector3 convertedMovement = new Vector3(movementVector.x, 0, movementVector.y) * (Legs.momentum * Time.fixedDeltaTime); // * Because the directions are different and we need to adjust for time and our momentum-based movement.
        RigidBody.AddForce(convertedMovement, ForceMode.Impulse);
        RigidBody.velocity = Vector3.ClampMagnitude(RigidBody.velocity, Legs.MaxSpeed);
        if (movementVector.magnitude < velocityCutoff) applyDeceleration();
        applyMaximumSpeed();
    }
    protected void applyDeceleration() {
        float frictionMomentum = Legs.momentum / RigidBody.mass; // ! WHAT THE FUCK. I SHOULD HAVE TO MULTIPLY THIS BY FIXEDDELTATIME FOR IT TO WORK RIGHT, BUT NO. NO WONDER IT TOOK ME AGES TO MAKE THIS WORK RIGHT. WTF UNITY? I AM SO ANGRY RN.
        float currentMomentum = RigidBody.velocity.magnitude * RigidBody.mass;
        if (frictionMomentum < currentMomentum) {
            Vector3 friction = -RigidBody.velocity.normalized * frictionMomentum;
            RigidBody.AddForce(friction, ForceMode.Impulse);
        } else {
            RigidBody.velocity = Vector3.zero;
        }
    }
    protected void applyMaximumSpeed() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Rolling")) return;
        if (RigidBody.velocity.magnitude > Legs.MaxSpeed) {
            RigidBody.velocity = Vector3.ClampMagnitude(RigidBody.velocity, Legs.MaxSpeed);
        }
    }
    protected void holsterGun() { //TODO: Make it so the connectionAnchorpoint position is set from the gun's connectionPoint. You'll need to use globaltolocal.
        LeftWeapon.transform.position = LHolster.transform.position;
        LeftWeapon.transform.rotation = LHolster.transform.rotation;
        LeftWeapon.transform.SetParent(null);
        LHolster.gameObject.GetComponent<ConfigurableJoint>().connectedBody = LeftWeapon.gameObject.GetComponent<Rigidbody>();
        LeftWeapon.GetComponent<Rigidbody>().isKinematic = false;
    }
    protected void unholsterGun() {
        LeftWeapon.transform.position = LeftHand.transform.position;
        LeftWeapon.transform.rotation = LeftHand.transform.rotation;
        LeftWeapon.transform.SetParent(LeftHand.transform, true);
        LeftWeapon.transform.position = LeftWeapon.gripPoint.position;

        LHolster.gameObject.GetComponent<ConfigurableJoint>().connectedBody = null;
        LeftWeapon.GetComponent<Rigidbody>().isKinematic = true;
    }
}

