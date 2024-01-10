using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrowBall : MonoBehaviour
{
    public GameObject anchorPrefab;
    public GameObject ballPrefab;
    public GameObject tablePrefab;

    InputActions m_InputActions;

    bool m_IsLoadedAnchor;
    float m_ForceFactor = 0.6f;

    List<GameObject> m_Colliders = new List<GameObject>();
    GameObject m_CurrentBall;
    List<GameObject> m_Balls = new List<GameObject>();

    Transform m_RightHand;
    Transform RightHand
    {
        get
        {
            if (m_RightHand == null)
            {
                m_RightHand = GameObject.Find("XR Origin/Camera Offset/RightController").transform;
            }
            return m_RightHand;
        }
    }
    void Awake()
    {
        PXR_MixedReality.EnableVideoSeeThrough(true);
        BindInput();
        BindEvent();
        PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();
        VRDebug.Log($"projectConfig.spatialAnchor: {projectConfig.spatialAnchor}");
    }
    void BindInput()
    {
        m_InputActions = new InputActions();
        m_InputActions.XRIRightHand.Trigger.performed += RightHandTriggerPerformed;
        m_InputActions.XRIRightHand.Trigger.canceled += RightHandTriggerCanceled;
        m_InputActions.XRIRightHand.PrimaryButton.performed += RightHandPrimaryButtonPreformed;
        m_InputActions.XRIRightHand.SecondaryButton.performed += RightHandSecondaryButtonPreformed;
        m_InputActions.XRILeftHand.PrimaryButton.performed += (ctx) => m_ForceFactor += 0.1f;
        m_InputActions.XRILeftHand.SecondaryButton.performed += (ctx) => m_ForceFactor -= 0.1f;
        m_InputActions.Enable();
    }
    void BindEvent()
    {
        PXR_Manager.SpatialTrackingStateUpdate += SpatialTrackingStateUpdate;
        PXR_Manager.AnchorEntityLoaded += AnchorEntityLoaded;
    }
    void SpatialTrackingStateUpdate(PxrEventSpatialTrackingStateUpdate trackingInfo)//Q: Is it always update? A: 
    {
        VRDebug.Log("SpatialTrackingStateUpdate state: " + trackingInfo.state);
        if (trackingInfo.state == PxrSpatialTrackingState.Invalid || trackingInfo.state == PxrSpatialTrackingState.Limited)
        {
            PXR_MixedReality.StartSpatialSceneCapture(out var taskId);
        }
        if (trackingInfo.state == PxrSpatialTrackingState.Valid)
        {
            if (!m_IsLoadedAnchor)
            {
                m_IsLoadedAnchor = true;
                LoadAnchor();
            }
        }
    }
    void LoadAnchor()
    {
        PxrSpatialSceneDataTypeFlags[] flags = { PxrSpatialSceneDataTypeFlags.Ceiling, PxrSpatialSceneDataTypeFlags.Door, PxrSpatialSceneDataTypeFlags.Floor, PxrSpatialSceneDataTypeFlags.Opening,
        PxrSpatialSceneDataTypeFlags.Window,PxrSpatialSceneDataTypeFlags.Wall,PxrSpatialSceneDataTypeFlags.Object };
        PXR_MixedReality.LoadAnchorEntityBySceneFilter(flags, out var taskId);
    }
    void AnchorEntityLoaded(PxrEventAnchorEntityLoaded loadingInfo)
    {
        if (loadingInfo.result == PxrResult.SUCCESS && loadingInfo.count != 0)
        {
            LoadCollider(loadingInfo);
        }
    }
    void LoadCollider(PxrEventAnchorEntityLoaded loadingInfo)
    {
        PXR_MixedReality.GetAnchorEntityLoadResults(loadingInfo.taskId, loadingInfo.count, out var loadedAnchors);
        foreach (var handle in loadedAnchors.Keys)
        {
            PXR_MixedReality.GetAnchorPose(handle, out var orientation, out var position);
            PXR_MixedReality.GetAnchorSceneLabel(handle, out var lable);
            PXR_MixedReality.GetAnchorPlaneBoundaryInfo(handle, out var boundaryCenter, out var boundaryExtent);
            VRDebug.Log($"{lable}'s boundary center: {boundaryCenter}, boundaryExtent: {boundaryExtent}");

            var anchor = new GameObject(lable.ToString());
            anchor.transform.position = position;
            anchor.transform.rotation = orientation;
            VRDebug.Log($"{lable}'s position: {position}, rotation: {orientation.eulerAngles}");

            switch (lable)
            {
                case PxrSceneLabel.Wall:
                case PxrSceneLabel.Door:
                    var collider_W_D = Instantiate(anchorPrefab);
                    collider_W_D.transform.SetParent(anchor.transform);
                    collider_W_D.transform.localPosition = Vector3.zero;
                    collider_W_D.transform.localRotation = Quaternion.identity;
                    m_Colliders.Add(collider_W_D);
                    collider_W_D.transform.localScale = new Vector3(boundaryExtent.x, boundaryExtent.y, 0.001f);
                    break;
                case PxrSceneLabel.Floor:
                case PxrSceneLabel.Ceiling:
                    var collider_F_C = Instantiate(anchorPrefab);
                    collider_F_C.transform.SetParent(anchor.transform);
                    collider_F_C.transform.localPosition = Vector3.zero;
                    collider_F_C.transform.localRotation = Quaternion.identity;
                    m_Colliders.Add(collider_F_C);
                    collider_F_C.transform.localScale = new Vector3(boundaryExtent.x * 100, boundaryExtent.y * 100, 0.001f);
                    break;
                case PxrSceneLabel.Table:
                    var collider_T = Instantiate(tablePrefab);
                    collider_T.transform.SetParent(anchor.transform);
                    collider_T.transform.localPosition = Vector3.zero;
                    collider_T.transform.localRotation = Quaternion.identity;
                    var material = collider_T.GetComponent<MeshRenderer>().material;
                    var shaderName = material.shader.name;
                    VRDebug.Log($"Shader Name: {shaderName}");

                    var collider_T1 = Instantiate(anchorPrefab);
                    collider_T1.transform.SetParent(anchor.transform);
                    collider_T1.transform.localPosition = Vector3.zero;
                    collider_T1.transform.localRotation = Quaternion.identity;
                    m_Colliders.Add(collider_T1);

                    PXR_MixedReality.GetAnchorVolumeInfo(handle, out var volumeCenter, out var volumeExtent);
                    collider_T.transform.localPosition += volumeCenter;
                    collider_T.transform.localScale = volumeExtent;

                    collider_T1.transform.localPosition += volumeCenter;
                    collider_T1.transform.localScale = volumeExtent;

                    VRDebug.Log($"{lable}'s volume center: {volumeCenter}, volumeExtent: {volumeExtent}");
                    break;
            }
        }
    }
    void RightHandTriggerPerformed(InputAction.CallbackContext context)
    {
        m_CurrentBall = Instantiate(ballPrefab);
        if (m_CurrentBall != null)
        {
            m_CurrentBall.transform.SetParent(RightHand);
            m_CurrentBall.transform.localPosition = Vector3.zero;
            var color = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
            m_CurrentBall.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
            m_CurrentBall.GetComponent<Rigidbody>().isKinematic = true;

            m_Balls.Add(m_CurrentBall);
            if (m_Balls.Count > 30)
            {
                Destroy(m_Balls[0].gameObject);
                m_Balls.RemoveAt(0);
            }
        }
    }
    void RightHandTriggerCanceled(InputAction.CallbackContext context)
    {
        if (m_CurrentBall != null)
        {
            m_CurrentBall.transform.SetParent(null);
            var rigidbody = m_CurrentBall.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                m_CurrentBall.GetComponent<Rigidbody>().isKinematic = false;
                var force = m_InputActions.XRIRightHand.Volecity.ReadValue<Vector3>();
                rigidbody.AddForce(force * m_ForceFactor, ForceMode.Impulse);
            }
        }
    }
    void RightHandPrimaryButtonPreformed(InputAction.CallbackContext context)
    {
        foreach (var c in m_Colliders)
        {
            var meshRender = c.GetComponent<MeshRenderer>();
            if (meshRender != null)
            {
                meshRender.enabled = !meshRender.enabled;
            }
        }
    }
    void RightHandSecondaryButtonPreformed(InputAction.CallbackContext context)
    {
        LoadAnchor();
    }
    void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            PXR_MixedReality.EnableVideoSeeThrough(true);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RightHandTriggerPerformed(new InputAction.CallbackContext());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            RightHandTriggerCanceled(new InputAction.CallbackContext());
        }
    }
}