using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public struct ReformPlanet : IJob
{
    [DeallocateOnJobCompletionAttribute]
    public NativeArray<Vector3> VertexPos;
    [DeallocateOnJobCompletionAttribute]
    public NativeArray<PlanetOld.Crater> Craters;
    public int NumCraters;
    public float RimWidth;
    public float RimSteepness;
    public float Smoothness;
    public float FloorHeight;

    public void Execute()
    {
        for (int i = 0; i < VertexPos.Length; i++)
        {
            float craterHeight = 0;

            for (int j = 0; j < NumCraters; j++)
            {
                float x = Vector3.Distance(VertexPos[i], Craters[j].Position.normalized) / Craters[j].Radius;

                float cavity = x * x - 1;
                float rimX = Mathf.Min(x - 1 - RimWidth, 0);
                float rim = RimSteepness * rimX * rimX;

                float craterShape = SmoothMax(cavity, FloorHeight, Smoothness);
                craterShape = SmoothMin(craterShape, rim, Smoothness);

                craterHeight += craterShape * Craters[j].Radius;
            }

            float Height = 1 + craterHeight;

            VertexPos[i] = new Vector3(VertexPos[i].x * Height, VertexPos[i].y * Height, VertexPos[i].z * Height);
        }
    }

    private float SmoothMin(float a, float b, float k)
    {
        float h = Mathf.Clamp01((b - a + k) / (2 * k));
        return a * h + b * (1 - h) - k * h * (1 - h);
    }

    private float SmoothMax(float a, float b, float k)
    {
        float h = Mathf.Clamp01((b - a + (-k)) / (2 * (-k)));
        return a * h + b * (1 - h) - (-k) * h * (1 - h);
    }
}
