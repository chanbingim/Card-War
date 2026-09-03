using System;
using UnityEngine;

public class InstancingComponent : MonoBehaviour
{
    private GraphicsBuffer _InstanceBuffer;
    private GraphicsBuffer _ArgsBuffer;

    private MaterialPropertyBlock _PropertyBlock;
    private int InstanceStride;

    private RenderParams _renderParams;
    private Mesh        _Mesh = null;
    private Material    _Material = null;
    private uint[]      _Args = null;

    public void Initailize(Mesh mesh, Material material,int DataLen, int Stride)
    {
        _Mesh = mesh;
        _Material = material;

        _InstanceBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, DataLen, Stride);
        _ArgsBuffer = new GraphicsBuffer(
          GraphicsBuffer.Target.IndirectArguments,
          1,
          sizeof(uint) * 5
        );

        _Args = new uint[] { 
                        _Mesh.GetIndexCount(0),
                        0, // InstanceCount
                        _Mesh.GetIndexStart(0),
                        _Mesh.GetBaseVertex(0),
                        0 
        };

        _PropertyBlock = new MaterialPropertyBlock();
        _PropertyBlock.SetBuffer("_InstanceBuffer", _InstanceBuffer);

        _renderParams = new RenderParams(_Material);
        _renderParams.matProps = _PropertyBlock;

        _renderParams.worldBounds = new Bounds(Vector3.zero, Vector3.one * 10000f);
    }

    public void SetData<T>(int Count, T[] Data)
    {
        _InstanceBuffer.SetData(Data, 0, 0, Count);

        _Args[1] = (uint)Count;
        _ArgsBuffer.SetData(_Args);
        _Material.SetBuffer("_InstanceBuffer", _InstanceBuffer);
    }

    public void OnDraw()
    {
        if (_Args[1] <= 0)
            return;

        Graphics.RenderMeshIndirect(_renderParams, _Mesh, _ArgsBuffer);
    }

    private void OnDestroy()
    {
        _InstanceBuffer?.Release();
        _ArgsBuffer?.Release();

        _InstanceBuffer = null;
        _ArgsBuffer = null;
    }
}
