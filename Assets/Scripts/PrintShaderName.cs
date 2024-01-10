using UnityEngine;

public class PrintShaderName : MonoBehaviour
{
    void Start()
    {
        var shaderName = GetComponent<MeshRenderer>().material.shader.name;
        VRDebug.Log(shaderName);
    }
}