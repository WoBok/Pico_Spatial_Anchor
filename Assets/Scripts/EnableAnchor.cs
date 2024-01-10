using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnableAnchor : MonoBehaviour
{
    InputActions m_InputActions;
    void Awake()
    {
        m_InputActions = new InputActions();
        m_InputActions.XRIRightHand.PrimaryButton.performed += Yes;
        m_InputActions.XRIRightHand.SecondaryButton.performed += No;
        m_InputActions.Enable();
    }
    public void Yes(InputAction.CallbackContext context)
    {
        PXR_ProjectSetting projectConfig = PXR_ProjectSetting.GetProjectConfig();
        projectConfig.spatialAnchor = true;
        LoadNextScene();
    }
    public void No(InputAction.CallbackContext context)
    {
        LoadNextScene();
    }
    void LoadNextScene()
    {
        SceneManager.LoadScene(1);
    }
}