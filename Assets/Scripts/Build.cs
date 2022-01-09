using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public const int Size = 8;

    public readonly int x, y;


    public Chunk(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class Build : MonoBehaviour
{
    public static Build Instance { get; private set; }

    [SerializeField] private Belt beltPrefab;
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Material grid;

    private int rotation;
    private Belt heldBelt;

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (heldBelt == null)
            heldBelt = Instantiate(beltPrefab);

        if (MouseOnFloor(out Vector3 mouseFloor))
        {
            grid.SetVector("_MousePos", mouseFloor);

            if (Input.GetMouseButtonDown(1))
            {
                GameObject cube = Instantiate(cubePrefab);
                cube.transform.position = mouseFloor + Vector3.up * 2;
            }

        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            rotation--;
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            rotation++;
        }

        if (MouseTile(out Vector2Int mouseTile))
        {
            heldBelt.SetPositionAndRotation(mouseTile.x, mouseTile.y, rotation);
        }

        if (Input.GetMouseButtonDown(0))
        {
            heldBelt = null;
        }
    }

    public bool MouseOnFloor(out Vector3 point)
    {
        point = Vector3.zero;

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane floor = new Plane(Vector3.up, 0);
        
        if (floor.Raycast(mouseRay, out float distance))
        {
            point = mouseRay.GetPoint(distance);
            return true;
        }

        return false;
    }

    public bool MouseTile(out Vector2Int tile)
    {
        tile = Vector2Int.zero;

        if (MouseOnFloor(out Vector3 point))
        {
            tile = Vector2Int.FloorToInt(new Vector2(point.x , point.z ));
            return true;
        }

        return false;
    }

}
