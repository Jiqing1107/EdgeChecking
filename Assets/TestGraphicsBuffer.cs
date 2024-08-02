using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TestGraphicsBuffer : MonoBehaviour
{
    private int kernelIndex;
    private GraphicsBuffer testGraphicsBuffer;
    public VisualEffect vfxGraph;

    private Vector4[] testBufferData = new Vector4[]
{
        new Vector4(1.0f / 100, 1.0f /100, 1.0f / 100, 0f / 100),
        new Vector4(2.0f / 100, 2.0f / 100, 2.0f / 100, 0f /100),
        new Vector4(3.0f / 100, 3.0f / 100, 3.0f / 100, 0f / 100),
        new Vector4(4.0f / 100, 4.0f / 100, 4.0f / 100, 0f / 100),
        new Vector4(5.0f / 100, 5.0f / 100, 5.0f / 100, 0f / 100)
};

    // Start is called before the first frame update
    void Start()
    {
        vfxGraph = GetComponent<VisualEffect>();
        testGraphicsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 5, sizeof(float) * 4);
        testGraphicsBuffer.SetData(testBufferData);
        vfxGraph.SetGraphicsBuffer("TestGraphicsBuffer", testGraphicsBuffer);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
