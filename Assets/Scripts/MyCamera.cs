using System;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

#nullable enable

[RequireComponent(typeof(Camera)), DisallowMultipleComponent, HideMonoScript] public sealed class MyCamera : MonoBehaviour {
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera topDownCamera;
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera thirdPersonCamera;
	[ShowInInspector, SerializeField, Required] private CinemachineVirtualCamera firstPersonCamera;
	private static                                      MyCamera?                 Instance;
	[ShowInInspector, SerializeField, Required] private GameObject                topDownCameraUpOverride;
	private                                             CinemachineBrain          cinemachineBrain => GetComponent<CinemachineBrain>();
	public                                              Camera                    Camera           => GetComponent<Camera>();
	[HideInInspector] public                            UnityEvent                eCameraModeChanged;

	[SerializeField] private float cameraDistance;
	[ShowInInspector, PropertyRange(0.1, 25)]
	public float CameraDistance{
		get => cameraDistance;
		set {
			topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_MaximumOrthoSize = value;
			topDownCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_MinimumOrthoSize = value;
			cameraDistance                                                                          = value;
		}
	}

	public enum CameraModes { TopDown, ThirdPerson, FirstPerson }
	private CameraModes cameraMode = CameraModes.TopDown;
	[ShowInInspector, EnumToggleButtons]
	public CameraModes CameraMode{
		get => cameraMode;
		set {
			if (value == cameraMode) return;
			switch (value) {
				case CameraModes.FirstPerson:
					firstPersonCamera.MoveToTopOfPrioritySubqueue();
					cinemachineBrain.m_WorldUpOverride = null;
					Camera.orthographic                = false;
					break;
				case CameraModes.ThirdPerson:
					thirdPersonCamera.MoveToTopOfPrioritySubqueue();
					cinemachineBrain.m_WorldUpOverride = null;
					Camera.orthographic                = false;
					break;
				case CameraModes.TopDown:
					topDownCamera.MoveToTopOfPrioritySubqueue();
					cinemachineBrain.m_WorldUpOverride = topDownCameraUpOverride.transform;
					Camera.orthographic                = true;
					break;
				default: throw new Exception("Not a valid camera type.");
			}

			cameraMode = value;
			print("invoking event");
			eCameraModeChanged.Invoke();
		}
	}

	private bool requirementsSet => topDownCamera != null && thirdPersonCamera != null && firstPersonCamera != null && topDownCameraUpOverride != null;

	public MyCamera() => eCameraModeChanged = new();
	private void Start() {
		CameraDistance = CameraDistance;
		if (Instance != null) throw new Exception("MyCamera is meant to be a singleton.");
		if (!requirementsSet) throw new Exception("Required value not set.");
		Instance = this;

		CameraMode = CameraMode;
	}
}