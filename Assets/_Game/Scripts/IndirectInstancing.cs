using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectInstancing : MonoBehaviour
{
    int instanceCount;
    public MapManager mapManager;
    public Mesh instanceMesh;
    public Material instanceMaterial;

    private int cachedInstanceCount = -1;
    private Mesh currentMesh;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer colorBuffer;
    private ComputeBuffer scaleBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    void OnEnable()
    {
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        mapManager.InitBuffers();
    }

    private void Start()
    {
        UpdateMesh(instanceMesh);
    }

    void Update()
    {
        // Update starting position buffer
        if (cachedInstanceCount != instanceCount) UpdateBuffers();

        // Render
        Graphics.DrawMeshInstancedIndirect(currentMesh, 0, instanceMaterial, new Bounds(Vector3.zero, new Vector3(1000.0f, 1000.0f, 1000.0f)), argsBuffer);
    }

    public void UpdateMesh(Mesh mesh)
    {
        currentMesh = mesh;
    }
    
    public void UpdateBuffers()
    {
        instanceCount = mapManager.listTiles.Count;
        if (instanceCount < 1) instanceCount = 1;

        // Positions & Colors
        if (positionBuffer != null) positionBuffer.Release();
        if (colorBuffer != null) colorBuffer.Release();
        if (scaleBuffer != null) scaleBuffer.Release();

        positionBuffer = new ComputeBuffer(instanceCount, 16);
        colorBuffer = new ComputeBuffer(instanceCount, 4 * 4);
        scaleBuffer = new ComputeBuffer(instanceCount, 16);

        Vector4[] positions = mapManager.posTiles;
        Vector4[] colors = mapManager.colorTiles;
        Vector4[] scales = mapManager.scaleTiles;

        positionBuffer.SetData(positions);
        colorBuffer.SetData(colors);
        scaleBuffer.SetData(scales);

        instanceMaterial.SetBuffer("positionBuffer", positionBuffer);
        instanceMaterial.SetBuffer("colorBuffer", colorBuffer);
        instanceMaterial.SetBuffer("scaleBuffer", scaleBuffer);

        // indirect args
        uint numIndices = (currentMesh != null) ? (uint)currentMesh.GetIndexCount(0) : 0;
        args[0] = numIndices;
        args[1] = (uint)instanceCount;
        argsBuffer.SetData(args);
        cachedInstanceCount = instanceCount;
    }

    void OnDisable()
    {
        if (positionBuffer != null) positionBuffer.Release();
        positionBuffer = null;

        if (colorBuffer != null) colorBuffer.Release();
        colorBuffer = null;

        if (scaleBuffer != null) scaleBuffer.Release();
        scaleBuffer = null;

        if (argsBuffer != null) argsBuffer.Release();
        argsBuffer = null;
    }
}
