using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

using Random = UnityEngine.Random;

public class Planet : MonoBehaviour
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

    [Space(10)]
    public Texture2D CraterValues;
    public Texture2D CraterFloorHeight;
    public Material PlanetMaterial;

    /// <summary>
    /// used Mesh
    /// </summary>
    private Mesh mesh;
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
    private float originBias = 0;

    public struct Crater
    {
        public Vector3 Position;
        public float Radius;
        public float OriginalRadius;
        public float FloorHeight;
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

        if (originBias != Bias)
            RecalculateRadius();

        SetCraterValues();

        if (originDetail != detail || originSphere != sphere)
        {
            originDetail = detail;
            originSphere = sphere;

            CalculateVertices(detail);
            CalculateTriangles();

            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }

            gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            GenerateTextures();
            SetShaderGraphProperties();
        }
        else
        {
            if (gameObject.GetComponent<MeshRenderer>() == null)
            {
                gameObject.AddComponent<MeshRenderer>();
            }

            gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;

            GenerateTextures();
            SetShaderGraphProperties();
        }
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

            // calculate radius
            float radius = Random.Range(0f, 1f);
            radius = BiasFunction(radius, Bias);
            radius = radius * RadiusMax * 2;
            radius = Mathf.Clamp(radius, RadiusMin, RadiusMax);

            // set radius
            c.Radius = radius;
            // set Original Radius
            c.OriginalRadius = c.Radius;
            // set random floorheight
            c.FloorHeight = Random.Range(.2f, 1f);

            // add new Crater to list
            cratersList.Add(c);
        }
    }

    public void GenerateRandomCrater()
    {
        // clear CraterList
        cratersList.Clear();

        for (int i = 0; i < NumCraters; i++)
        {
            Crater c;
            // set random position betwen 0 and 1 normalized
            c.Position = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            // calculate radius
            float radius = Random.Range(0f, 1f);
            // set Original Radius
            c.OriginalRadius = radius;
            radius = BiasFunction(radius, Bias);
            radius = radius * RadiusMax * 2;
            radius = Mathf.Clamp(radius, RadiusMin, RadiusMax);

            // set radius
            c.Radius = radius;
            // set random floorheight
            c.FloorHeight = Random.Range(.2f, 1f);

            // add new Crater to list
            cratersList.Add(c);
        }
    }

    public void RecalculateRadius()
    {
        List<Crater> tmp = new List<Crater>();
        for (int i = 0; i < cratersList.Count; i++)
        {
            // recalculate radius
            float radius = BiasFunction(craters[i].OriginalRadius, Bias);
            radius = radius * RadiusMax * 2;
            radius = Mathf.Clamp(radius, RadiusMin, RadiusMax);

            Crater c = new Crater
            {
                Position = cratersList[i].Position,
                OriginalRadius = cratersList[i].OriginalRadius,
                Radius = radius,
            };
            tmp.Add(c);
        }

        cratersList = tmp;
    }

    public void GenerateTextures()
    {
        // reset preGeneratedTexture
        CraterValues = new Texture2D(NumCraters, 1, TextureFormat.RGBAFloat, false);
        CraterFloorHeight = new Texture2D(NumCraters, 1, TextureFormat.RGBAFloat, false);

        Color c;
        for (int i = 0; i < ((NumCraters > 0) ? NumCraters : 1); i++)
        {
            // get color out of craterValues (r = x, g = y, b = z, a = Radius)
            c = new Color(cratersList[i].Position.x, cratersList[i].Position.y, cratersList[i].Position.z, cratersList[i].Radius);
            // set pixel on preGeneratedTexture
            CraterValues.SetPixel(i, 0, c);

            // get new color with random floor Height (r = floorHeight)
            c = new Color(cratersList[i].FloorHeight, 1f, 1f);
            // set pixel on preGeneratedTexture
            CraterFloorHeight.SetPixel(i, 0, c);
        }
        // apply settings of texture
        CraterValues.Apply();
        CraterFloorHeight.Apply();
    }

    public void SetShaderGraphProperties()
    {
        PlanetMaterial.SetTexture("Craters", CraterValues);
        PlanetMaterial.SetTexture("FloorHeight", CraterFloorHeight);
        PlanetMaterial.SetFloat("NumCraters", NumCraters);
        PlanetMaterial.SetVector("RadiusMinMax", new Vector4(RadiusMin, RadiusMax, 0, 0));
        PlanetMaterial.SetFloat("RimWidth", RimWidth);
        PlanetMaterial.SetFloat("RimSteepness", RimSteepness);
        PlanetMaterial.SetFloat("Smoothness", Smoothness);
        PlanetMaterial.SetFloat("Bias", Bias);
    }
}
