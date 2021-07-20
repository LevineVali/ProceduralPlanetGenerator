using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

using Random = UnityEngine.Random;

public class PlanetOld : MonoBehaviour
{
    [Header("Planet Settings")]
    [Range(1, 300)]
    public int detail = 1;
    public bool sphere = false;

    [Header("Crater Settings")]
    public int NumCraters = 1;
    [Range(0, 1)]
    public float RadiusMin = 0;
    [Range(0, 1)]
    public float RadiusMax = 1;
    [Range(0, 1)]
    public float Bias = 1;
    public float RimWidth = 0;
    public float RimSteepness = 0;
    public float Smoothness = 0;
    public float FloorHeight = -1f;

    [Space(10)]
    public bool UseShader = true;
    public bool ShaderGraph = true;

    [Space(10)]
    public Texture2D CraterValues;
    public Material PlanetMaterial;
    public ComputeShader ReformPlanet;

    /// <summary>
    /// used Mesh
    /// </summary>
    private Mesh mesh;
    /// <summary>
    /// original mesh
    /// </summary>
    private Mesh originMesh;
    private List<Crater> cratersList = new List<Crater>();
    private Crater[] craters;

    // mesh variables
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    // add new row of vertices for the bottom side
    private List<List<Vector3>> helperLists = new List<List<Vector3>>();

    // control variables
    private int originDetail = 0;
    private bool originSphere = false;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct Crater
    {
        public Vector3 Position;
        public float Radius;
        public float OriginalRadius;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct SourceVertex
    {
        public Vector3 Position;
        public Vector3 Normals;
    }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct GeneratedVertex
    {
        public Vector3 Position;
    }

    private void Start()
    {
        UpdateValues();
    }

    private void OnValidate()
    {
        UpdateValues();
    }

    public void UpdateValues()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        if (originMesh == null)
        {
            originMesh = new Mesh();
        }
        originMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        SetCraterValues();

        if (UseShader)
        {
            if (originDetail != detail || originSphere != sphere)
            {
                originDetail = detail;
                originSphere = sphere;

                CalculateVertices(detail);
                CalculateTriangles();
                SetOriginMeshValues();

                if (gameObject.GetComponent<MeshRenderer>() == null)
                {
                    gameObject.AddComponent<MeshRenderer>();
                }

                gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

                if (ShaderGraph)
                {
                    GenerateTexture();
                    SetShaderGraphProperties();
                }
                else if (!ShaderGraph)
                {
                    SetComputeShaderProperties();
                    UseComputeShader();
                }
            }
            else
            {
                if (gameObject.GetComponent<MeshRenderer>() == null)
                {
                    gameObject.AddComponent<MeshRenderer>();
                }

                gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

                if (ShaderGraph)
                {
                    GenerateTexture();
                    SetShaderGraphProperties();
                }
                else if (!ShaderGraph)
                {
                    SetComputeShaderProperties();
                    UseComputeShader();
                }
            }
            SetMeshValues();
        }
        else
        {
            CalculateVertices(detail);
            CalculateTriangles();

            // generate 6 lists because my cpu has 6 cores
            List<Vector3> VertexPos1 = new List<Vector3>();
            List<Vector3> VertexPos2 = new List<Vector3>();
            List<Vector3> VertexPos3 = new List<Vector3>();
            List<Vector3> VertexPos4 = new List<Vector3>();
            List<Vector3> VertexPos5 = new List<Vector3>();
            List<Vector3> VertexPos6 = new List<Vector3>();
            List<Vector3> VertexPos7 = new List<Vector3>();
            List<Vector3> VertexPos8 = new List<Vector3>();
            List<Vector3> VertexPos9 = new List<Vector3>();
            List<Vector3> VertexPos10 = new List<Vector3>();
            List<Vector3> VertexPos11 = new List<Vector3>();
            List<Vector3> VertexPos12 = new List<Vector3>();

            // split vertices in 6 parts
            // fill all arrays with vertices
            for (int i = 0; i < vertices.Count; i++)
            {
                if (i < vertices.Count)
                {
                    VertexPos1.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos2.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos3.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos4.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos5.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos6.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos7.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos8.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos9.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos10.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos11.Add(vertices[i]);
                }
                i++;
                if (i < vertices.Count)
                {
                    VertexPos12.Add(vertices[i]);
                }
            }

            NativeArray<Vector3> NativeVertexPos1 = new NativeArray<Vector3>(VertexPos1.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos2 = new NativeArray<Vector3>(VertexPos2.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos3 = new NativeArray<Vector3>(VertexPos3.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos4 = new NativeArray<Vector3>(VertexPos4.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos5 = new NativeArray<Vector3>(VertexPos5.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos6 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos7 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos8 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos9 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos10 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos11 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);
            NativeArray<Vector3> NativeVertexPos12 = new NativeArray<Vector3>(VertexPos6.Count, Allocator.TempJob);

            for (int i = 0; i < VertexPos1.Count; i++)
            {
                NativeVertexPos1[i] = VertexPos1[i];
            }
            for (int i = 0; i < VertexPos2.Count; i++)
            {
                NativeVertexPos2[i] = VertexPos2[i];
            }
            for (int i = 0; i < VertexPos3.Count; i++)
            {
                NativeVertexPos3[i] = VertexPos3[i];
            }
            for (int i = 0; i < VertexPos4.Count; i++)
            {
                NativeVertexPos4[i] = VertexPos4[i];
            }
            for (int i = 0; i < VertexPos5.Count; i++)
            {
                NativeVertexPos5[i] = VertexPos5[i];
            }
            for (int i = 0; i < VertexPos6.Count; i++)
            {
                NativeVertexPos6[i] = VertexPos6[i];
            }
            for (int i = 0; i < VertexPos7.Count; i++)
            {
                NativeVertexPos7[i] = VertexPos7[i];
            }
            for (int i = 0; i < VertexPos8.Count; i++)
            {
                NativeVertexPos8[i] = VertexPos8[i];
            }
            for (int i = 0; i < VertexPos9.Count; i++)
            {
                NativeVertexPos9[i] = VertexPos9[i];
            }
            for (int i = 0; i < VertexPos10.Count; i++)
            {
                NativeVertexPos10[i] = VertexPos10[i];
            }
            for (int i = 0; i < VertexPos11.Count; i++)
            {
                NativeVertexPos11[i] = VertexPos11[i];
            }
            for (int i = 0; i < VertexPos12.Count; i++)
            {
                NativeVertexPos12[i] = VertexPos12[i];
            }

            NativeArray<Crater> NativeCraters1 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters2 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters3 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters4 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters5 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters6 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters7 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters8 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters9 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters10 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters11 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);
            NativeArray<Crater> NativeCraters12 = new NativeArray<Crater>(craters.Length, Allocator.TempJob);

            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters1[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters2[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters3[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters4[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters5[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters6[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters7[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters8[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters9[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters10[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters11[i] = craters[i];
            }
            for (int i = 0; i < craters.Length; i++)
            {
                NativeCraters12[i] = craters[i];
            }

            // crate native array for all jobhandels
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(12, Allocator.TempJob);

            for (int i = 0; i < 12; i++)
            {
                
                // create new job
                ReformPlanet reformPlanet = new ReformPlanet
                {
                    NumCraters = NumCraters,
                    RimWidth = RimWidth,
                    RimSteepness = RimSteepness,
                    Smoothness = Smoothness,
                    FloorHeight = FloorHeight
                };
                // add the right array of vertices to the jobs
                switch (i)
                {
                    case 0:
                        reformPlanet.VertexPos = NativeVertexPos1;
                        reformPlanet.Craters = NativeCraters1;
                        break;
                    case 1:
                        reformPlanet.VertexPos = NativeVertexPos2;
                        reformPlanet.Craters = NativeCraters2;
                        break;
                    case 2:
                        reformPlanet.VertexPos = NativeVertexPos3;
                        reformPlanet.Craters = NativeCraters3;
                        break;
                    case 3:
                        reformPlanet.VertexPos = NativeVertexPos4;
                        reformPlanet.Craters = NativeCraters4;
                        break;
                    case 4:
                        reformPlanet.VertexPos = NativeVertexPos5;
                        reformPlanet.Craters = NativeCraters5;
                        break;
                    case 5:
                        reformPlanet.VertexPos = NativeVertexPos6;
                        reformPlanet.Craters = NativeCraters6;
                        break;
                    case 6:
                        reformPlanet.VertexPos = NativeVertexPos7;
                        reformPlanet.Craters = NativeCraters7;
                        break;
                    case 7:
                        reformPlanet.VertexPos = NativeVertexPos8;
                        reformPlanet.Craters = NativeCraters8;
                        break;
                    case 8:
                        reformPlanet.VertexPos = NativeVertexPos9;
                        reformPlanet.Craters = NativeCraters9;
                        break;
                    case 9:
                        reformPlanet.VertexPos = NativeVertexPos10;
                        reformPlanet.Craters = NativeCraters10;
                        break;
                    case 10:
                        reformPlanet.VertexPos = NativeVertexPos11;
                        reformPlanet.Craters = NativeCraters11;
                        break;
                    case 11:
                        reformPlanet.VertexPos = NativeVertexPos12;
                        reformPlanet.Craters = NativeCraters12;
                        break;
                };
                // add Scheduled job to the jobhandels array
                jobHandles[i] = reformPlanet.Schedule();
            }
            // wait until all jobhandels are complete
            JobHandle.CompleteAll(jobHandles);

            jobHandles.Dispose();

            // recombine the splitet vertices in the right order to a full vertices list.
            // clear list
            vertices.Clear();

            for (int i = 0; i < VertexPos1.Count + 1; i++)
            {
                if (i < VertexPos1.Count)
                {
                    vertices.Add(VertexPos1[i]);
                }
                if (i < VertexPos2.Count)
                {
                    vertices.Add(VertexPos2[i]);
                }
                if (i < VertexPos3.Count)
                {
                    vertices.Add(VertexPos3[i]);
                }
                if (i < VertexPos4.Count)
                {
                    vertices.Add(VertexPos4[i]);
                }
                if (i < VertexPos5.Count)
                {
                    vertices.Add(VertexPos5[i]);
                }
                if (i < VertexPos6.Count)
                {
                    vertices.Add(VertexPos6[i]);
                }
                if (i < VertexPos7.Count)
                {
                    vertices.Add(VertexPos7[i]);
                }
                if (i < VertexPos8.Count)
                {
                    vertices.Add(VertexPos8[i]);
                }
                if (i < VertexPos9.Count)
                {
                    vertices.Add(VertexPos9[i]);
                }
                if (i < VertexPos10.Count)
                {
                    vertices.Add(VertexPos10[i]);
                }
                if (i < VertexPos11.Count)
                {
                    vertices.Add(VertexPos11[i]);
                }
                if (i < VertexPos12.Count)
                {
                    vertices.Add(VertexPos12[i]);
                }
            }

            SetMeshValues();
        }
    }

    private void UseComputeShader()
    {
        // calculate buffersize
        int positionSize = sizeof(float) * 3;
        int radiusSize = sizeof(float);
        int totalSize = positionSize + radiusSize + radiusSize;

        // generate buffers
        GraphicsBuffer CraterBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, NumCraters, totalSize);
        GraphicsBuffer VertexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, originMesh.vertexCount, sizeof(float) * 6);
        GraphicsBuffer generatedVerticesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, originMesh.vertexCount, sizeof(float) * 3);

        // set all buffers
        ReformPlanet.SetBuffer(0, "craters", CraterBuffer);
        ReformPlanet.SetBuffer(0, "originVertices", VertexBuffer);
        ReformPlanet.SetBuffer(0, "vertices", generatedVerticesBuffer);

        SourceVertex[] sv = new SourceVertex[originMesh.vertexCount];
        for (int i = 0; i < sv.Length; i++)
        {
            sv[i].Position = originMesh.vertices[i];
            sv[i].Normals = originMesh.normals[i];
        }

        Vector3[] filler = new Vector3[originMesh.vertexCount];

        // fill buffers 
        CraterBuffer.SetData(craters);
        VertexBuffer.SetData(sv);
        generatedVerticesBuffer.SetData(filler);

        // start ComputeShader
        ReformPlanet.Dispatch(0, 10, 1, 1);

        GeneratedVertex[] generatedVertices = new GeneratedVertex[originMesh.vertexCount];
        generatedVerticesBuffer.GetData(generatedVertices);

        vertices.Clear();
        for (int i = 0; i < generatedVertices.Length; i++)
        {
            vertices.Add(generatedVertices[i].Position);
        }

        CraterBuffer.Release();
        VertexBuffer.Release();
        generatedVerticesBuffer.Release();

        SetMeshValues();
    }

    private void SetCraterValues()
    {
        if (cratersList.Count < NumCraters)
        {
            AddGeneratedCrater();
        }

        if (RadiusMin > RadiusMax)
            RadiusMin = RadiusMax;

        if (NumCraters < 1)
            NumCraters = 1;

        craters = new Crater[(NumCraters > 0) ? NumCraters : 1];

        for (int i = 0; i < NumCraters; i++)
        {
            craters[i] = cratersList[i];
            craters[i].Radius = BiasFunction(craters[i].OriginalRadius, Bias);
        }
    }

    private void CalculateVertices(int detail)
    {
        // reset vertices list
        vertices = new List<Vector3>();

        // reset helper lists
        // add ne row of vertices for the bottom side
        helperLists = new List<List<Vector3>>();

        // first point
        Vector3 pointOnUnit = new Vector3(0, 1, 0);
        // add to list of vertices
        vertices.Add(pointOnUnit);

        // distance between points
        float distanceS = .5f / detail;
        float distanceL = distanceS * 2;

        // loop throu rows
        for (int i = 0; i < detail; i++)
        {
            if (i + 1 < detail)
            {
                helperLists.Add(new List<Vector3>());
            }

            float height = 1f - (distanceL * (i + 1));

            // loop through left side
            for (int a = 0; a < i + 1; a++)
            {
                pointOnUnit = new Vector3(distanceS * (i + 1), height, .5f - distanceL * a - ((i + 1 == detail) ? 0 : (distanceS * (detail - (i + 1)))));
                vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                if (i + 1 < detail)
                {
                    pointOnUnit = new Vector3(pointOnUnit.x, pointOnUnit.y * -1f, pointOnUnit.z);
                    helperLists[i].Add(sphere ? pointOnUnit.normalized : pointOnUnit);
                }
            }

            // loop through top side
            for (int b = 0; b < i + 1; b++)
            {
                pointOnUnit = new Vector3(.5f - distanceL * b - ((i + 1 == detail) ? 0 : (distanceS * (detail - (i + 1)))), height, -distanceS * (i + 1));
                vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                if (i + 1 < detail)
                {
                    pointOnUnit = new Vector3(pointOnUnit.x, pointOnUnit.y * -1f, pointOnUnit.z);
                    helperLists[i].Add(sphere ? pointOnUnit.normalized : pointOnUnit);
                }
            }

            // loop through right side
            for (int c = 0; c < i + 1; c++)
            {
                pointOnUnit = new Vector3(-distanceS * (i + 1), height, -.5f + distanceL * c + ((i + 1 == detail) ? 0 : (distanceS * (detail - (i + 1)))));
                vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                if (i + 1 < detail)
                {
                    pointOnUnit = new Vector3(pointOnUnit.x, pointOnUnit.y * -1f, pointOnUnit.z);
                    helperLists[i].Add(sphere ? pointOnUnit.normalized : pointOnUnit);
                }
            }

            // loop through bottom side
            for (int d = 0; d < i + 1; d++)
            {
                pointOnUnit = new Vector3(-.5f + distanceL * d + ((i + 1 == detail) ? 0 : (distanceS * (detail - (i + 1)))), height, distanceS * (i + 1));
                vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                if (i + 1 < detail)
                {
                    pointOnUnit = new Vector3(pointOnUnit.x, pointOnUnit.y * -1f, pointOnUnit.z);
                    helperLists[i].Add(sphere ? pointOnUnit.normalized : pointOnUnit);
                }
            }
        }

        // add vertices of helperlists in correct order
        for (int i = helperLists.Count - 1; i > -1; i--)
        {
            for (int j = 0; j < helperLists[i].Count; j++)
            {
                vertices.Add(helperLists[i][j]);
            }
        }

        // last point of octaheder
        pointOnUnit = new Vector3(0, -1f, 0);
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

        //Debug.Log("Vertex: " + (DateTime.Now - time).TotalSeconds);
    }

    private void CalculateTriangles()
    {
        // reset triangle lsit
        triangles = new List<int>();

        int startTop = 0;
        int startBottom = vertices.Count - 1;

        int currentRow;
        int nextRow;
        int rowBegin;

        // top tip of octaheder
        triangles.Add(startTop); triangles.Add(1); triangles.Add(2);
        triangles.Add(startTop); triangles.Add(2); triangles.Add(3);
        triangles.Add(startTop); triangles.Add(3); triangles.Add(4);
        triangles.Add(startTop); triangles.Add(4); triangles.Add(1);

        startTop = 1;

        for (int i = 1; i < detail; i++)
        {
            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startTop);
                triangles.Add(startTop + (i * 4));
                triangles.Add(startTop + (i * 4) + 1);

                if (i > a)
                {
                    triangles.Add(startTop);
                    triangles.Add(startTop + (i * 4) + 1);
                    triangles.Add(startTop + 1);
                    startTop++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startTop);
                triangles.Add(startTop + (i * 4) + 1);
                triangles.Add(startTop + (i * 4) + 2);

                if (i > a)
                {
                    triangles.Add(startTop);
                    triangles.Add(startTop + (i * 4) + 2);
                    triangles.Add(startTop + 1);
                    startTop++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startTop);
                triangles.Add(startTop + (i * 4) + 2);
                triangles.Add(startTop + (i * 4) + 3);

                if (i > a)
                {
                    triangles.Add(startTop);
                    triangles.Add(startTop + (i * 4) + 3);
                    triangles.Add(startTop + 1);
                    startTop++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                if (a + 1 == i + 1)
                {
                    currentRow = (i * 4) - 1;
                    nextRow = i * 4 + ((i + 1) * 4) - 1;
                    rowBegin = startTop - currentRow;

                    triangles.Add(rowBegin);
                    triangles.Add(rowBegin + currentRow);
                    triangles.Add(rowBegin + nextRow);

                    triangles.Add(rowBegin);
                    triangles.Add(rowBegin + nextRow);
                    triangles.Add(rowBegin + currentRow + 1);

                    startTop++;
                }
                else
                {
                    triangles.Add(startTop);
                    triangles.Add(startTop + (i * 4) + 3);
                    triangles.Add(startTop + (i * 4) + 4);

                    if (i - 1 > a)
                    {
                        triangles.Add(startTop);
                        triangles.Add(startTop + (i * 4) + 4);
                        triangles.Add(startTop + 1);
                        startTop++;
                    }
                }
            }
        }

        // bottom tip of octaheder
        triangles.Add(startBottom); triangles.Add(startBottom - 3); triangles.Add(startBottom - 4);
        triangles.Add(startBottom); triangles.Add(startBottom - 2); triangles.Add(startBottom - 3);
        triangles.Add(startBottom); triangles.Add(startBottom - 1); triangles.Add(startBottom - 2);
        triangles.Add(startBottom); triangles.Add(startBottom - 4); triangles.Add(startBottom - 1);

        for (int i = 1; i < detail; i++)
        {
            startBottom -= ((i * 2) - 1) * 4;

            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startBottom);
                triangles.Add(startBottom - ((i + 1) * 4) + 1);
                triangles.Add(startBottom - ((i + 1) * 4));

                if (i > a)
                {
                    triangles.Add(startBottom);
                    triangles.Add(startBottom + 1);
                    triangles.Add(startBottom - ((i + 1) * 4) + 1);
                    startBottom++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startBottom);
                triangles.Add(startBottom - ((i + 1) * 4) + 2);
                triangles.Add(startBottom - ((i + 1) * 4) + 1);

                if (i > a)
                {
                    triangles.Add(startBottom);
                    triangles.Add(startBottom + 1);
                    triangles.Add(startBottom - ((i + 1) * 4) + 2);
                    startBottom++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                triangles.Add(startBottom);
                triangles.Add(startBottom - ((i + 1) * 4) + 3);
                triangles.Add(startBottom - ((i + 1) * 4) + 2);

                if (i > a)
                {
                    triangles.Add(startBottom);
                    triangles.Add(startBottom + 1);
                    triangles.Add(startBottom - ((i + 1) * 4) + 3);
                    startBottom++;
                }
            }
            for (int a = 0; a < i + 1; a++)
            {
                if (a + 1 == i + 1)
                {
                    currentRow = (i * 4) - 1;
                    rowBegin = startBottom - currentRow;

                    triangles.Add(rowBegin);
                    triangles.Add(rowBegin - 1);
                    triangles.Add(rowBegin + currentRow);

                    triangles.Add(rowBegin);
                    triangles.Add(rowBegin - ((i + 1) * 4));
                    triangles.Add(rowBegin - 1);

                    startBottom++;
                }
                else
                {
                    triangles.Add(startBottom);
                    triangles.Add(startBottom - ((i + 1) * 4) + 4);
                    triangles.Add(startBottom - ((i + 1) * 4) + 3);

                    if (i - 1 > a)
                    {
                        triangles.Add(startBottom);
                        triangles.Add(startBottom + 1);
                        triangles.Add(startBottom - ((i + 1) * 4) + 4);
                        startBottom++;
                    }
                }
            }
        }

        //Debug.Log("Tris: " + (DateTime.Now - time).TotalSeconds);
    }

    private void SetMeshValues()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }


    private void SetOriginMeshValues()
    {
        originMesh.Clear();
        originMesh.vertices = vertices.ToArray();
        originMesh.triangles = triangles.ToArray();
        originMesh.RecalculateNormals();

        //Debug.Log("Mesh: " + (DateTime.Now - time).TotalSeconds);
    }

    private float BiasFunction(float x, float bias)
    {
        float k = Mathf.Pow(1 - bias, 3);
        return (x * k) / (x * k - x + 1);
    }

    private void AddGeneratedCrater()
    {
        int count = cratersList.Count;

        for (int i = 0; i < NumCraters - count; i++)
        {
            Crater c;
            // set random position betwen 0 and 1 normalized
            c.Position = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            // set random radius betwen MinRadius and MaxRadius
            c.Radius = Random.Range(RadiusMin, RadiusMax);
            // set Original Radius
            c.OriginalRadius = c.Radius;

            // add new Crater to list
            cratersList.Add(c);
        }
    }

    public void GenerateRandomCrater(int _NumCraters)
    {
        // clear CraterList
        cratersList.Clear();

        for (int i = 0; i < _NumCraters; i++)
        {
            Crater c;
            // set random position betwen 0 and 1
            c.Position = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            // set random radius betwen 0 and 1
            c.Radius = Random.Range(RadiusMin, RadiusMax);
            // set original radius
            c.OriginalRadius = c.Radius;

            // add new Crater to list
            cratersList.Add(c);
        }
    }

    public void GenerateTexture()
    {
        // reset preGeneratedTexture
        CraterValues = new Texture2D(NumCraters, 1, TextureFormat.RGBAFloat, false);

        Color c;
        for (int i = 0; i < NumCraters; i++)
        {
            // get color out of craterValues (r = x, g = y, b = z, a = Radius)
            c = new Color(cratersList[i].Position.x, cratersList[i].Position.y, cratersList[i].Position.z, cratersList[i].Radius);
            // set pixel on preGeneratedTexture
            CraterValues.SetPixel(i, 0, c);
        }
        // apply settings of texture
        CraterValues.Apply();
    }

    public void SetShaderGraphProperties()
    {
        PlanetMaterial.SetTexture("Craters", CraterValues);
        PlanetMaterial.SetFloat("NumCraters", NumCraters);
        PlanetMaterial.SetVector("RadiusMinMax", new Vector4(RadiusMin, RadiusMax, 0, 0));
        PlanetMaterial.SetFloat("RimWidth", RimWidth);
        PlanetMaterial.SetFloat("RimSteepness", RimSteepness);
        PlanetMaterial.SetFloat("Smoothness", Smoothness);
        PlanetMaterial.SetFloat("FloorHeight", FloorHeight);
        PlanetMaterial.SetFloat("Bias", Bias);
    }

    public void SetComputeShaderProperties()
    {
        ReformPlanet.SetInt("NumCraters", NumCraters);
        ReformPlanet.SetFloat("RimWidth", RimWidth);
        ReformPlanet.SetFloat("RimSteepness", RimSteepness);
        ReformPlanet.SetFloat("Smoothness", Smoothness);
        ReformPlanet.SetFloat("FloorHeight", FloorHeight);
    }
}
