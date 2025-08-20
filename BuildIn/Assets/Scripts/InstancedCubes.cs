using UnityEngine;
using System.Collections.Generic;

public class InstancedCubes : MonoBehaviour
{
    [Header("Settings")]
    public int countX = 10;
    public int countY = 10;
    public int countZ = 10;
    public float spacing = 2f;

    [Header("Movement")]
    public float moveAmplitude = 0.5f;
    public float moveSpeed = 2f;

    [Header("Rendering")]
    public Mesh mesh;               // Куб
    public Material material;       // Материал (с галочкой Enable GPU Instancing!)

    private Matrix4x4[] matrices;   // Позиции кубов
    private Vector3[] basePositions;
    private Matrix4x4[] batchBuffer; // Предвыделенный буфер для батчей (без аллокаций в Update)

    void Start()
    {
        int total = countX * countY * countZ;
        matrices = new Matrix4x4[total];
        basePositions = new Vector3[total];
        batchBuffer = new Matrix4x4[Mathf.Min(1023, total)];

        int i = 0;
        for (int x = 0; x < countX; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = 0; z < countZ; z++)
                {
                    Vector3 pos = new Vector3(x * spacing, y * spacing, z * spacing);
                    basePositions[i] = pos;
                    matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                    i++;
                }
            }
        }
    }

    void Update()
    {
        float t = Time.time * moveSpeed;

        for (int i = 0; i < basePositions.Length; i++)
        {
            Vector3 pos = basePositions[i];
            pos.y += Mathf.Sin(t + (pos.x + pos.y + pos.z) * 0.1f) * moveAmplitude; 
            matrices[i] = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
        }


        int batchSize = 1023;
        for (int i = 0; i < matrices.Length; i += batchSize)
        {
            int length = Mathf.Min(batchSize, matrices.Length - i);
            System.Array.Copy(matrices, i, batchBuffer, 0, length);
            Graphics.DrawMeshInstanced(mesh, 0, material, batchBuffer, length);
        }

    }
}
