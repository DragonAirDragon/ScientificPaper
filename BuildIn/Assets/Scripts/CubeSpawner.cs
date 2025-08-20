using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [Header("Settings")]
    public int countX = 10;   // кубы по X
    public int countY = 10;   // кубы по Y
    public int countZ = 10;   // кубы по Z
    public float spacing = 2f;

    [Header("Movement")]
    public float moveAmplitude = 0.5f;
    public float moveSpeed = 2f;

    private GameObject[,,] cubes;

    void Start()
    {
        cubes = new GameObject[countX, countY, countZ];

        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    Vector3 pos = new Vector3(x * spacing, y * spacing, z * spacing);
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = pos;
                    cube.transform.parent = transform;

                    cubes[x, y, z] = cube;
                }
            }
        }
    }

    void Update()
    {
        float t = Time.time * moveSpeed;

        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    GameObject cube = cubes[x, y, z];
                    if (cube != null)
                    {
                        Vector3 basePos = new Vector3(x * spacing, y * spacing, z * spacing);
                        cube.transform.position = basePos + Vector3.up * Mathf.Sin(t + (x + y + z) * 0.1f) * moveAmplitude;
                    }
                }
            }
        }
    }
}
