using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Serializable]
    private struct CameraStruct
    {
        public MovementState movementState;
        public CameraParams cameraParams;

        [Serializable]
        public struct CameraParams
        {
            [SceneObjectsOnly] public CinemachineCamera camera;
            public int priority;
        }
    }

    #region Variables

    #region Serialized

    [Title("Cameras")]
    [SerializeField] private CameraStruct[] cameras;
    [SerializeField] private float transitionTime;

    [Title("Rotation Limits")]
    [SerializeField] private float minHeadPitch;
    [SerializeField] private float maxHeadPitch;

    [Title("Sensitivity")]
    [SerializeField, Range(0.1f, 2)] private float sensitivity_X;
    [SerializeField, Range(0.1f, 2)] private float sensitivity_Y;

    [Title("References")]
    [SerializeField, Required] private InputManager inputManager;
    [SerializeField, Required] private PlayerMovement playerMovement;

    #endregion Serialized

    #region Not Serialized

    private Dictionary<MovementState, CameraStruct.CameraParams> cameraDictionary = new();

    private Vector3 lookRotation = new();
    private float targetCameraYaw;
    private float targetCameraPitch;

    #endregion Not Serialized

    #endregion Variables

    #region Unity Functions

    private void Awake()
    {
        void SubscribeToEvents()
        {
            playerMovement.OnCrouching += PlayerMovement_OnCrouching;
        }
        void FillDictionary()
        {
            foreach (CameraStruct cameraStruct in cameras)
                cameraDictionary.Add(cameraStruct.movementState, cameraStruct.cameraParams);
        }

        SubscribeToEvents();
        FillDictionary();

        targetCameraPitch = transform.eulerAngles.x;
        targetCameraYaw = transform.eulerAngles.y;

        if (Camera.main.TryGetComponent(out CinemachineBrain pBrain))
            pBrain.DefaultBlend.Time = transitionTime;
    }

    private void PlayerMovement_OnCrouching(object sender, bool isCrouched)
        => cameraDictionary[MovementState.Crouching].camera.Priority = isCrouched ? cameraDictionary[MovementState.Crouching].priority : 0;

    private void LateUpdate()
    {
        Rotation();
    }

    #endregion Unity Functions

    #region Camera Rotation

    private void Rotation()
    {
        void CalculateTargetRotation()
        {
            targetCameraYaw += inputManager.GetInputAction(InputManager.EAction.Look).ReadValue<Vector2>().x * sensitivity_X;
            targetCameraPitch += inputManager.GetInputAction(InputManager.EAction.Look).ReadValue<Vector2>().y * sensitivity_Y;
            targetCameraPitch = Mathf.Clamp(targetCameraPitch, minHeadPitch, maxHeadPitch);
        }
        void Rotate()
        {
            lookRotation.x = targetCameraPitch;
            lookRotation.y = targetCameraYaw;
            transform.rotation = Quaternion.Euler(lookRotation);
        }

        CalculateTargetRotation();
        Rotate();
    }

    #endregion Camera Rotation
}