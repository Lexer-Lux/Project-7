using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using static MyCamera;

#nullable enable

namespace BodyParts.Player {
	[RequireComponent(typeof(PlayerInput)), DisallowMultipleComponent] public sealed class Brain : MonoBehaviour {
		private                                             BodyParts.Legs legs        => GetComponent<BodyParts.Legs>();
		private                                             BodyParts.Arms arms        => GetComponent<BodyParts.Arms>();
		private                                             PlayerInput    playerInput => GetComponent<PlayerInput>();
		[SerializeField, ShowInInspector, Required] private MyCamera       myCamera;
		[SerializeField, ShowInInspector, Required] private GameObject     cameraFollowTransform;
		[SerializeField, ShowInInspector, Required] private GameObject     cursorTransform;
		private                                             Vector2        crosshairScreenPosition => new Vector2(Screen.width / 2, Screen.height / 2);
		public                                              float          MouseTurnSpeed            = 1;
		public                                              float          GamepadTurnSpeed          = 5;
		public                                              float          TopDownCameraSpeedMouse   = 0.5f;
		public                                              float          TopDownCameraSpeedGamepad = 0.5f;

		private Vector2 lookInputValue;
		private Vector2 moveInputValue;

		public void OnDown(InputValue value) {
			legs.DownHeld = value.isPressed;
			Debug.Log("down is down");
		}
		public void OnUp(InputValue value)       => legs.UpHeld = value.isPressed;
		public void OnMove(InputValue value)     => moveInputValue = value.Get<Vector2>();
		public void OnLook(InputValue value)     => lookInputValue = value.Get<Vector2>();

		public void OnLeftHand(InputValue value) {
			Debug.Log("Left Hand");
			arms.LeftHandPressed = value.isPressed;
			
		} 
		public void OnLeftArm(InputValue value) => arms.LeftArmPressed = value.isPressed;
		public void OnRightHand(InputValue value) => arms.RightHandPressed = value.isPressed;
		public void OnRightArm(InputValue value) => arms.RightArmPressed = value.isPressed;


		public void Update() {
			updateMoveValue();
			updateLookValue();
			if(myCamera.CameraMode == CameraModes.TopDown) turnTowardsCursor();

			void updateMoveValue() {
				switch (myCamera.CameraMode) {
					case CameraModes.TopDown:
						legs.Movement = moveInputValue;
						break;
					case CameraModes.ThirdPerson or CameraModes.FirstPerson: {
						// * TODO make movement work camera-relatively.
						legs.Movement = moveInputValue;
						break;
					}
				}
			}

			void updateLookValue() {
				switch (myCamera.CameraMode) {
					case CameraModes.TopDown:
						/*Vector3 newCursorTransform = cursorTransform.transform.position;
						newCursorTransform.x               += lookInputValue.x * lookSpeed;
						newCursorTransform.z               += lookInputValue.y * lookSpeed;
						cursorTransform.transform.position =  newCursorTransform;*/
						Rigidbody camFollow = cursorTransform.GetComponent<Rigidbody>();
						camFollow.velocity = new Vector3(lookInputValue.x * lookSpeed, 0, lookInputValue.y * lookSpeed);
						break;
					case CameraModes.ThirdPerson or CameraModes.FirstPerson:
						cursorTransform.transform.position = myCamera.Camera.ScreenToWorldPoint(new Vector3(crosshairScreenPosition.x, crosshairScreenPosition.y, 25f));
						gameObject.transform.Rotate(Vector3.forward, lookInputValue.x * lookSpeed * -1);
						cameraFollowTransform.transform.Rotate(Vector3.right, lookInputValue.y * lookSpeed * -1);
						break;
				}
			}

			void turnTowardsCursor() {
				//TODO: Implement smooth rotation using addtorque so it looks better and makes the pigtail physics get used.
				Vector3 cameraPosition                  = cursorTransform.transform.position;
				cameraPosition.y = transform.position.y;
				Vector3 relativePosition                = transform.position - cameraPosition;
				float   angle                           = Vector3.SignedAngle(relativePosition, Vector3.back, Vector3.up);
				transform.rotation = Quaternion.Euler(90f, 0f, angle);
			}
		}

		public void Start() {
			myCamera.eCameraModeChanged.AddListener(cameraModeChanged);
			updateLookSpeed();
		}
		public void cameraModeChanged() {
			print("camera mode changed");
			updateLookSpeed();
			switch (myCamera.CameraMode) {
				case CameraModes.FirstPerson or CameraModes.ThirdPerson:
					cameraFollowTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
					break;
				case CameraModes.TopDown:
					cursorTransform.transform.position = new Vector3(gameObject.transform.position.x, 0f, gameObject.transform.position.z);
					break;
			}
		}

		[ShowInInspector, ReadOnly] private float lookSpeed;
		public                              void  OnControlsChanged(PlayerInput playerInput) { updateLookSpeed(); }
		private void updateLookSpeed() {
			switch (myCamera.CameraMode) {
				case CameraModes.TopDown when playerInput.currentControlScheme == "Keyboard&Mouse":
					lookSpeed = TopDownCameraSpeedMouse;
					break;
				case CameraModes.TopDown when playerInput.currentControlScheme == "Gamepad":
					lookSpeed = TopDownCameraSpeedGamepad;
					break;
				case CameraModes.ThirdPerson or CameraModes.FirstPerson when playerInput.currentControlScheme == "Keyboard&Mouse":
					lookSpeed = MouseTurnSpeed;
					break;
				case CameraModes.ThirdPerson or CameraModes.FirstPerson when playerInput.currentControlScheme == "Gamepad":
					lookSpeed = GamepadTurnSpeed;
					break;
				default: throw new ArgumentOutOfRangeException($"$Either camera mode or input mode is invalid. Input mode is: {playerInput.currentControlScheme}, Camera mode is {myCamera.CameraMode}");
			}
		}
	}
}