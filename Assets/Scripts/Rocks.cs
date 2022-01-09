using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocks : MonoBehaviour
{
    public Camera cam;
    public MeshRenderer meshRenderer;
    public Material material;

    public RenderTexture texture;
    public Vector2Int resolution;

    public void Awake()
    {
        texture = new RenderTexture(resolution.x, resolution.y, 0);
        meshRenderer.material = material;
        cam.targetTexture = texture;
        meshRenderer.material.SetTexture("_Noise", texture);
    }
}
