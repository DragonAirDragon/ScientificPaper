using UnityEngine;

public class IndirectInstancedCubes : MonoBehaviour
{
    [Header("Grid")]
    public int countX = 10;
    public int countY = 10;
    public int countZ = 10;
    public float spacing = 2f;

    [Header("Wave")]
    public float moveAmplitude = 0.5f;
    public float moveSpeed = 2f;

    [Header("Rendering")]
    public Mesh mesh;                 // Куб (любой меш с индексами)
    public Material material;         // Шейдер ниже, instancing включен не обязателен (мы шлём буфер)
    public ComputeShader compute;     // Компьют-шейдер ниже
    public bool autoBounds = true;

    ComputeBuffer matricesBuffer;     // float4x4 на инстанс
    ComputeBuffer basePosBuffer;      // float3 на инстанс
    ComputeBuffer argsBuffer;         // 5 uint для DrawMeshInstancedIndirect

    Bounds drawBounds;
    int kernel;
    int instanceCount;
    int groupsX;

    static readonly int ID_Matrices = Shader.PropertyToID("_Matrices");
    static readonly int ID_BasePos  = Shader.PropertyToID("_BasePositions");
    static readonly int ID_Time     = Shader.PropertyToID("_Time");
    static readonly int ID_Amp      = Shader.PropertyToID("_Amp");
    static readonly int ID_Speed    = Shader.PropertyToID("_Speed");

    void Start()
    {
        // количество инстансов
        instanceCount = countX * countY * countZ;

        // буфер базовых позиций
        var basePos = new Vector3[instanceCount];
        int i = 0;
        for (int x = 0; x < countX; x++)
        for (int y = 0; y < countY; y++)
        for (int z = 0; z < countZ; z++)
            basePos[i++] = new Vector3(x * spacing, y * spacing, z * spacing);

        basePosBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 3);
        basePosBuffer.SetData(basePos);

        // буфер матриц (float4x4)
        matricesBuffer = new ComputeBuffer(instanceCount, sizeof(float) * 16, ComputeBufferType.Structured);

        // косвенные аргументы: indexCountPerInstance, instanceCount, startIndex, baseVertex, startInstance
        uint[] args = new uint[5];
        args[0] = (uint)mesh.GetIndexCount(0);
        args[1] = (uint)instanceCount;
        args[2] = (uint)mesh.GetIndexStart(0);
        args[3] = (uint)mesh.GetBaseVertex(0);
        args[4] = 0;
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);

        // большие границы для проверки (потом верни на расчёт по сетке)
        drawBounds = new Bounds(Vector3.zero, Vector3.one * 1000f);

        // компьюта
        kernel = compute.FindKernel("CSMain");
        compute.SetBuffer(kernel, ID_BasePos, basePosBuffer);
        compute.SetBuffer(kernel, ID_Matrices, matricesBuffer);
        compute.SetInt("_InstanceCount", instanceCount);

        // материал должен знать про буфер матриц
        material.SetBuffer(ID_Matrices, matricesBuffer);
        material.enableInstancing = true;

        // группы (numthreads = 64)
        groupsX = Mathf.CeilToInt(instanceCount / 64.0f);
        if (groupsX < 1) groupsX = 1;
    }

    void Update()
    {
        // передаём параметры волны
        compute.SetFloat(ID_Time, Time.time);
        compute.SetFloat(ID_Amp, moveAmplitude);
        compute.SetFloat(ID_Speed, moveSpeed);

        // считаем матрицы на GPU
        compute.Dispatch(kernel, groupsX, 1, 1);

        // при необходимости обновляем bounds (если объект двигается)
        if (!autoBounds) drawBounds.center = transform.position;

        // рисуем
        Graphics.DrawMeshInstancedIndirect(mesh, 0, material, drawBounds, argsBuffer);
    }

    void OnDisable()
    {
        SafeRelease(ref matricesBuffer);
        SafeRelease(ref basePosBuffer);
        SafeRelease(ref argsBuffer);
    }
    static void SafeRelease(ref ComputeBuffer b)
    {
        if (b != null) { b.Release(); b = null; }
    }
}
