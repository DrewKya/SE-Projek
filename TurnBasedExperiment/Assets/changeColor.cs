using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeColor : MonoBehaviour
{
    MeshRenderer Mesh;
    public void SetNewColor(Color newColor)
    {
        Mesh = GetComponent<MeshRenderer>();
        Mesh.material.color = newColor;
    }
}
