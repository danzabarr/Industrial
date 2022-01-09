using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceRenderer : MonoBehaviour
{
    public Mesh mesh;
    public Material material;
    [Range(0, 1023)] public int count;
    public Matrix4x4[] matrices;
    public MaterialPropertyBlock properties;
    public UnityEngine.Rendering.ShadowCastingMode castShadows;

    private void Start()
    {
        count = 100;
        matrices = new Matrix4x4[count];
        
        for (int i = 0; i < count; i++)
        {
            matrices[i] = Matrix4x4.identity;
            matrices[i] *= Matrix4x4.Translate(Random.insideUnitCircle.x0y() * 10);
            matrices[i] *= Matrix4x4.Scale(Vector3.one * Random.Range(.1f, .3f));
            matrices[i] *= Matrix4x4.Rotate(Quaternion.AngleAxis(90, Vector3.forward));
            matrices[i] *= Matrix4x4.Rotate(Quaternion.AngleAxis(Random.Range(0, 360), Vector3.left));
        }

    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count);
    }
}
