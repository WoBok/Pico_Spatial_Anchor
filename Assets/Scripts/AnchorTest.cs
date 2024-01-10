using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnchorTest : MonoBehaviour
{
    public GameObject anchorPrefab;
    public GameObject wallPrefab;
    InputActions inputActions;

    void Awake()
    {
        PXR_MixedReality.EnableVideoSeeThrough(true);
        inputActions = new InputActions();
        BindInput();
        BindAnchorEvent();
    }
    void BindInput()
    {
        inputActions.XRILeftHand.PrimaryButton.performed += LeftPrimaryButtonPressed;
        inputActions.XRILeftHand.SecondaryButton.performed += LeftSecondaryButtonPressed;
        inputActions.XRIRightHand.PrimaryButton.performed += RightPrimaryButtonPressed;
        inputActions.XRIRightHand.SecondaryButton.performed += RightSecondaryButtonPressed;
        inputActions.Enable();
    }
    void BindAnchorEvent()
    {
        PXR_Manager.SpatialTrackingStateUpdate += SpatialTrackingStateUpdate;
        PXR_Manager.AnchorEntityLoaded += AnchorEntityLoaded;
    }
    void LeftPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        OnBtnPressedLoadRoomData();
        VRDebug.Log("LeftPrimaryButtonPressed");
    }
    void LeftSecondaryButtonPressed(InputAction.CallbackContext context)
    {

    }
    void RightPrimaryButtonPressed(InputAction.CallbackContext context)
    {
        var handPosition = inputActions.XRIRightHand.Position.ReadValue<Vector3>();
        var handRotation = inputActions.XRIRightHand.Rotation.ReadValue<Quaternion>();
        VRDebug.Log($"Hand Position: {handPosition}, Hand Rotation: {handRotation}");
    }
    void RightSecondaryButtonPressed(InputAction.CallbackContext context)
    {

    }
    void OnBtnPressedLoadRoomData()
    {
        PxrSpatialSceneDataTypeFlags[] flags = { PxrSpatialSceneDataTypeFlags.Ceiling, PxrSpatialSceneDataTypeFlags.Door, PxrSpatialSceneDataTypeFlags.Floor, PxrSpatialSceneDataTypeFlags.Opening,
        PxrSpatialSceneDataTypeFlags.Window,PxrSpatialSceneDataTypeFlags.Wall,PxrSpatialSceneDataTypeFlags.Object };
        PXR_MixedReality.LoadAnchorEntityBySceneFilter(flags, out var taskId);
    }
    void SpatialTrackingStateUpdate(PxrEventSpatialTrackingStateUpdate stateInfo)
    {
        if (stateInfo.state == PxrSpatialTrackingState.Invalid || stateInfo.state == PxrSpatialTrackingState.Limited)
        {
            VRDebug.Log("PXR_MRSample TrackingState Event:" + stateInfo.state + ", Restart Spatial Scene Capture.");
            PXR_MixedReality.StartSpatialSceneCapture(out var taskId);
        }
    }
    //�ٷ�����
    void AnchorEntityLoaded(PxrEventAnchorEntityLoaded result)
    {
        if (result.result == PxrResult.SUCCESS && result.count != 0)
        {
            PXR_MixedReality.GetAnchorEntityLoadResults(result.taskId, result.count, out var loadedAnchors);
            foreach (var key in loadedAnchors.Keys)
            {
                Debug.Log("PXR_MRSample AnchorEntityLoaded handle:" + key);
                // ����Ҫ�������� anchorPrefab
                GameObject anchorObject = Instantiate(anchorPrefab);

                PXR_MixedReality.GetAnchorPose(key, out var orientation, out var position);
                anchorObject.transform.position = position;
                anchorObject.transform.rotation = orientation;


                PXR_MixedReality.GetAnchorSceneLabel(key, out var label);
                Debug.Log("PXR_MRSample SceneLabel:" + label);
                switch (label)
                {
                    // ƽ�棺ê��λ��ƽ������ģ�X �᷽��Ϊ��Y �᷽��Ϊ�ߣ�Z �᷽��Ϊ������
                    case PxrSceneLabel.Floor:
                    case PxrSceneLabel.Wall:
                        {
                            PXR_MixedReality.GetAnchorPlaneBoundaryInfo(key, out var center, out var extent);
                            // ����Ҫ�������� wallPrefab
                            var wall = Instantiate(wallPrefab);
                            wall.transform.parent = anchorObject.transform;
                            wall.transform.localPosition = Vector3.zero;
                            wall.transform.localRotation = Quaternion.identity;
                            wall.transform.Rotate(90f, 0, 0);
                            wall.transform.localScale = new Vector3(extent.x, 0.001f, extent.y);
                            VRDebug.Log($"anchor position: {position}, boundary center: {center}, boundary extent: {extent}");
                        }
                        break;
                    // �����壺ê��λ���������ϱ�������ģ�X �᷽��Ϊ��Y �᷽��Ϊ�ߣ�Z �᷽��Ϊ���
                    case PxrSceneLabel.Sofa:
                    case PxrSceneLabel.Table:
                        {
                            PXR_MixedReality.GetAnchorVolumeInfo(key, out var center, out var extent);
                            var roomObject = Instantiate(wallPrefab);
                            roomObject.transform.parent = anchorObject.transform;
                            roomObject.transform.localPosition = Vector3.zero;
                            roomObject.transform.localRotation = Quaternion.identity;
                            roomObject.transform.localPosition += center;
                            roomObject.transform.localScale = extent;
                        }
                        break;
                }
            }
        }
    }
    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_MixedReality.EnableVideoSeeThrough(true);
        }
    }
}