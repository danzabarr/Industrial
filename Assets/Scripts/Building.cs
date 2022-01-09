using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Building : MonoBehaviour
{
    public int X;
    public int Y;
    public int Rotation;
    public int width;
    public int height;

    public void OnValidate()
    {
        ResetPosition();
    }

    public void SetPositionAndRotation(int x, int y, int rotation)
    {
        X = x;
        Y = y;
        Rotation = rotation;
        ResetPosition();
    }

    public void ResetPosition()
    {
        transform.position = new Vector3(X + (Rotation % 2 == 0 ? width : height) / 2f, 0, Y + (Rotation % 2 == 0 ? height : width) / 2f);
        transform.rotation = Quaternion.Euler(0, 90 * Rotation, 0);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .25f);
        for (int x = GetMinX(); x <= GetMaxX(); x++)
            for (int y = GetMinY(); y <= GetMaxY(); y++)
                Gizmos.DrawCube(new Vector3(x + 0.5f, 0, y + 0.5f), new Vector3(0.95f, 0.1f, 0.95f));

        //Gizmos.DrawCube(transform.position, new Vector3((Rotation % 2 == 0 ? width : height), .1f, (Rotation % 2 == 0 ? height : width)));
    }

    public int GetMinX() => X;
    public int GetMaxX() => X + (Rotation % 2 == 0 ? width : height) - 1;
    public int GetMinY() => Y;
    public int GetMaxY() => Y + (Rotation % 2 == 0 ? height : width) - 1;

}
