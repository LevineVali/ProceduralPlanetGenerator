using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator_OLD : MonoBehaviour
{
    [Range(1, 50)]
    public int detail = 1;
    public bool sphere = false;

    Mesh mesh;

    private void OnValidate()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }

        CreateMesh(detail);

        if (gameObject.GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
        }
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>();
        }
    }

    public void CreateMesh(int detail)
    {
        List<int> triangles = new List<int>();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> verticesFlipped = new List<Vector3>();

        int triCounter = 0;
        float halfLength = 1f / detail;
        int triCountFirstFace = 2;

        #region FrontTopFace
        // create first triangle
        Vector3 pointOnUnit = new Vector3(0, 1, 0);
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);
        triangles.Add(triCounter);
        triCounter++;

        Vector3 pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);

        pointOnUnit = new Vector3(0 + halfLength, 1f / detail * (detail - 1), -1f + (halfLength * (detail - 1)));
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);
        triangles.Add(triCounter);
        triCounter++;

        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);

        pointOnUnit = new Vector3(0 - halfLength, 1f / detail * (detail - 1), -1f + (halfLength * (detail - 1)));
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);
        triangles.Add(triCounter);
        triCounter++;

        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);

        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (j == 0)
                {
                    pointOnUnit = new Vector3(halfLength * (i + 1) - halfLength * j, 1 - halfLength * (i + 1), 0 - halfLength * (i + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    pointOnUnit = new Vector3(halfLength * (i + 1) - halfLength * (j + 2), 1 - halfLength * (i + 1), 0 - halfLength * (i + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(triCounter - 2 - (i - 1));
                    triangles.Add(triCounter);
                    triangles.Add(triCounter + 1);

                    triCounter += 2;
                }
                else
                {
                    pointOnUnit = new Vector3(halfLength * (i + 1) - (halfLength * 2) * (j + 1), 1 - halfLength * (i + 1), 0 - halfLength * (i + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(triCounter - 1);
                    triangles.Add(triCounter - 3 - (i - 1));
                    triangles.Add(triCounter - 4 - (i - 1));

                    triangles.Add(triCounter);
                    triangles.Add(triCounter - 3 - (i - 1));
                    triangles.Add(triCounter - 1);

                    triCountFirstFace = triCounter;
                    triCounter++;
                }
            }
        }
        #endregion

        #region LeftTopFace
        // create first triangle
        triangles.Add(0);
        triangles.Add(2);

        pointOnUnit = new Vector3(-1f + (halfLength * (detail - 1)), 1f / detail * (detail - 1), 0 + halfLength);
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);
        triangles.Add(triCounter);
        triCounter++;

        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);

        int countHelper = 2;
        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (j == 0)
                {
                    pointOnUnit = new Vector3(0 - halfLength * (i + 1), 1 - halfLength * (i + 1), -halfLength * (i + 1) + halfLength * (j + 2));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(countHelper);
                    triangles.Add(countHelper + (i + 2));
                    triangles.Add(triCounter);

                    triangles.Add(triCounter);
                    triangles.Add(triCounter - i);
                    triangles.Add(countHelper);

                    countHelper += (i + 2);
                    triCounter++;
                }
                else
                {
                    pointOnUnit = new Vector3(0 - halfLength * (i + 1), 1 - halfLength * (i + 1), -halfLength * (i + 1) + (halfLength * 2) * (j + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(triCounter - 1);
                    triangles.Add(triCounter);
                    triangles.Add(triCounter - (i + 1));

                    if (j < i)
                    {
                        triangles.Add(triCounter);
                        triangles.Add(triCounter - i);
                        triangles.Add(triCounter - (i + 1));
                    }

                    triCounter++;
                }
            }
        }
        #endregion

        #region BackTopFace
        // create first triangle
        countHelper = 2 + triCountFirstFace - 1;

        triangles.Add(0);
        triangles.Add(countHelper);

        pointOnUnit = new Vector3(0 + halfLength, 1f / detail * (detail - 1), 1f - (halfLength * (detail - 1)));
        vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);
        triangles.Add(triCounter);
        triCounter++;

        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);

        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (j == 0)
                {
                    pointOnUnit = new Vector3(-halfLength * (i + 1) + halfLength * (j + 2), 1 - halfLength * (i + 1), 0 + halfLength * (i + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(countHelper);
                    triangles.Add(countHelper + (i + 1));
                    triangles.Add(triCounter);

                    triangles.Add(triCounter);
                    triangles.Add(triCounter - i);
                    triangles.Add(countHelper);

                    countHelper += (i + 1);
                    triCounter++;
                }
                else
                {
                    pointOnUnit = new Vector3(-halfLength * (i + 1) + (halfLength * 2) * (j + 1), 1 - halfLength * (i + 1), 0 + halfLength * (i + 1));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(triCounter - 1);
                    triangles.Add(triCounter);
                    triangles.Add(triCounter - (i + 1));

                    if (j < i)
                    {
                        triangles.Add(triCounter);
                        triangles.Add(triCounter - i);
                        triangles.Add(triCounter - (i + 1));
                    }

                    triCounter++;
                }
            }
        }
        #endregion

        #region RightTopFace
        // create first triangle
        countHelper = 1;
        for (int i = 0; i < detail; i++)
        {
            countHelper += i + 1;
        }
        countHelper += triCountFirstFace;
        int countHelper_2 = 1;

        triangles.Add(0);
        triangles.Add(countHelper);
        triangles.Add(countHelper_2);

        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                // first triangle in new row
                if (j == 0)
                {
                    pointOnUnit = new Vector3(0 + halfLength * (i + 1), 1 - halfLength * (i + 1), halfLength * (i + 1) - halfLength * (j + 2));
                    vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                    if (i < detail - 1)
                    {
                        pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                        verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                    }

                    triangles.Add(countHelper);
                    triangles.Add(countHelper + (2 + (i - 1)));
                    triangles.Add(triCounter - (i - 1));

                    countHelper += i + 1;
                    triCounter++;
                }
                else
                {
                    // last triangles of the 2nd row
                    if (j == i && i == 1)
                    {
                        triangles.Add(countHelper_2);
                        triangles.Add(triCounter - i);
                        triangles.Add(countHelper_2 + (i + 1));

                        triangles.Add(triCounter - i);
                        triangles.Add(countHelper_2 - (i - 1));
                        triangles.Add(countHelper - (i + 1));

                        countHelper_2 += (i + 1);
                    }
                    // 3rd+ row
                    else
                    {
                        // triangles on the right edge
                        if (j == 1)
                        {
                            triangles.Add(countHelper - (i + 1));
                            triangles.Add(triCounter - i);
                            triangles.Add(triCounter - (i + (i - 1)));

                            triangles.Add(triCounter - (i + (i - 1)));
                            triangles.Add(triCounter - i);
                            triangles.Add(triCounter - (i - 1));
                        }
                        // triangles in the middle
                        else if (j > 1 && j < i)
                        {
                            pointOnUnit = new Vector3(0 + halfLength * (i + 1), 1 - halfLength * (i + 1), halfLength * (i + 1) - (halfLength * 2) * j);
                            vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                            if (i < detail - 1)
                            {
                                pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                                verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                            }

                            triangles.Add(triCounter - (i + i));
                            triangles.Add(triCounter - i);
                            triangles.Add(triCounter - (i + (i - 1)));

                            triangles.Add(triCounter - (i + (i - 1)));
                            triangles.Add(triCounter - i);
                            triangles.Add(triCounter - (i - 1));
                        }
                        // triangles on theleft edge
                        else
                        {
                            pointOnUnit = new Vector3(0 + halfLength * (i + 1), 1 - halfLength * (i + 1), halfLength * (i + 1) - (halfLength * 2) * j);
                            vertices.Add(sphere ? pointOnUnit.normalized : pointOnUnit);

                            if (i < detail - 1)
                            {
                                pointOnUnitFlipped = new Vector3(pointOnUnit.x, -pointOnUnit.y, pointOnUnit.z);
                                verticesFlipped.Add(sphere ? pointOnUnitFlipped.normalized : pointOnUnitFlipped);
                            }

                            triangles.Add(triCounter - (i + j));
                            triangles.Add(triCounter - i);
                            triangles.Add(countHelper_2);

                            triangles.Add(countHelper_2);
                            triangles.Add(triCounter - i);
                            triangles.Add(countHelper_2 + (i + 1));

                            countHelper_2 += (i + 1);
                        }
                    }

                    triCounter++;
                }
            }
        }
        #endregion

        for (int i = 0; i < verticesFlipped.Count; i++)
        {
            vertices.Add(verticesFlipped[i]);
        }

        #region FrontBottomFace
        // create first triangle
        triCounter -= (detail - 1);
        int index = triCounter;
        // tri from topfrontface, bottom right one
        int tri = (index - 1) / 4;
        triangles.Add(triCounter);
        triCounter++;

        triangles.Add(triCounter + 1);

        triangles.Add(triCounter);
        triCounter += 2;

        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (i < detail - 1)
                {
                    if (j == 0)
                    {
                        triangles.Add(triCounter - 2 - (i - 1));
                        triangles.Add(triCounter + 1);
                        triangles.Add(triCounter);

                        triCounter += 2;
                    }
                    else
                    {
                        triangles.Add(triCounter - 1);
                        triangles.Add(triCounter - 4 - (i - 1));
                        triangles.Add(triCounter - 3 - (i - 1));

                        triangles.Add(triCounter);
                        triangles.Add(triCounter - 1);
                        triangles.Add(triCounter - 3 - (i - 1));

                        triCounter++;
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        triangles.Add(triCounter - (detail));
                        triangles.Add(tri + 1);
                        triangles.Add(tri);
                        tri++;
                    }
                    else
                    {
                        triangles.Add(triCounter - (detail - (j - 1)));
                        triangles.Add(triCounter - (detail - j));
                        triangles.Add(tri);

                        triangles.Add(tri);
                        triangles.Add(triCounter - (detail - j));
                        triangles.Add(tri + 1);

                        tri++;
                    }
                }
            }
        }
        #endregion

        #region LeftBottomFace
        // create first triangle
        triangles.Add(index);
        triangles.Add(triCounter);
        triangles.Add(index + 2);

        triCounter++;

        countHelper = index + 2;

        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (i < detail - 1)
                {
                    if (j == 0)
                    {
                        triangles.Add(countHelper);
                        triangles.Add(triCounter);
                        triangles.Add(countHelper + (i + 2));

                        triangles.Add(triCounter);
                        triangles.Add(countHelper);
                        triangles.Add(triCounter - i);

                        countHelper += (i + 2);
                    }
                    else
                    {
                        triangles.Add(triCounter - 1);
                        triangles.Add(triCounter - (i + 1));
                        triangles.Add(triCounter);

                        if (j < i)
                        {
                            triangles.Add(triCounter);
                            triangles.Add(triCounter - (i + 1));
                            triangles.Add(triCounter - i);
                        }
                    }
                    triCounter++;
                }
                else
                {
                    if (j == 0)
                    {
                        triangles.Add(tri);
                        triangles.Add(countHelper);

                        for (int k = 0; k < i; k++)
                        {
                            tri += detail - k;
                        }
                        tri -= (i - 1);

                        triangles.Add(tri);


                        triangles.Add(tri);
                        triangles.Add(countHelper);
                        triangles.Add(triCounter - i);

                        countHelper += (i + 1);
                    }
                    else
                    {
                        triangles.Add(tri);
                        triangles.Add(triCounter - (i - (j - 1)));
                        triangles.Add(tri + 1);

                        if (j < i)
                        {
                            triangles.Add(tri + 1);
                            triangles.Add(triCounter - (i - (j - 1)));
                            triangles.Add(triCounter - (i - j));
                        }
                        tri++;
                    }
                }
            }
        }
        #endregion

        #region BackBottomFace
        // create first triangle
        if (detail == 1)
        {
            countHelper += 1;
        }
        else
        {
            countHelper -= (detail - 1);
        }

        triangles.Add(index);
        triangles.Add(triCounter);
        triangles.Add(countHelper);
        triCounter++;


        for (int i = 1; i < detail; i++)
        {
            for (int j = 0; j < i + 1; j++)
            {
                if (i < detail - 1)
                {
                    if (j == 0)
                    {
                        triangles.Add(countHelper);
                        triangles.Add(triCounter);
                        triangles.Add(countHelper + (i + 1));

                        triangles.Add(triCounter);
                        triangles.Add(countHelper);
                        triangles.Add(triCounter - i);

                        countHelper += (i + 1);
                    }
                    else
                    {
                        triangles.Add(triCounter - 1);
                        triangles.Add(triCounter - (i + 1));
                        triangles.Add(triCounter);

                        if (j < i)
                        {
                            triangles.Add(triCounter);
                            triangles.Add(triCounter - (i + 1));
                            triangles.Add(triCounter - i);
                        }
                    }
                    triCounter++;
                }
                else
                {
                    if (j == 0)
                    {
                        triangles.Add(tri);
                        triangles.Add(countHelper);

                        for (int k = 0; k < i; k++)
                        {
                            tri += detail - k;
                        }
                        tri -= (i - 1);

                        triangles.Add(tri);


                        triangles.Add(tri);
                        triangles.Add(countHelper);
                        triangles.Add(triCounter - i);

                        countHelper += (i + 1);
                    }
                    else
                    {
                        triangles.Add(tri);
                        triangles.Add(triCounter - (i - (j - 1)));
                        triangles.Add(tri + 1);

                        if (j < i)
                        {
                            triangles.Add(tri + 1);
                            triangles.Add(triCounter - (i - (j - 1)));
                            triangles.Add(triCounter - (i - j));
                        }
                        tri++;
                    }
                }
            }
        }
        #endregion

        #region RightTopFace
        // create first triangle
        if (detail == 1)
        {
            triangles.Add(5);
            triangles.Add(1);
            triangles.Add(4);
        }
        else
        {

            int tmp = 4;
            countHelper = index;
            for (int i = 1; i < detail - 1; i++)
            {
                countHelper += tmp + i + 1;
            }
            //countHelper += 1;
            countHelper_2 = index + 1;

            triangles.Add(index);
            triangles.Add(countHelper_2);
            triangles.Add(countHelper);
        }

        //for (int i = 1; i < detail; i++)
        //{
        //    for (int j = 0; j < i + 1; j++)
        //    {
        //        // first triangle in new row
        //        if (j == 0)
        //        {
        //            triangles.Add(countHelper);
        //            triangles.Add(triCounter - (i - 1));
        //            triangles.Add(countHelper + (2 + (i - 1)));
        //
        //            countHelper += i + 1;
        //            triCounter++;
        //        }
        //        else
        //        {
        //            // last triangles of the 2nd row
        //            if (j == i && i == 1)
        //            {
        //                triangles.Add(countHelper_2);
        //                triangles.Add(countHelper_2 + (i + 1));
        //                triangles.Add(triCounter - i);
        //
        //                triangles.Add(triCounter - i);
        //                triangles.Add(countHelper - (i + 1));
        //                triangles.Add(countHelper_2 - (i - 1));
        //
        //                countHelper_2 += (i + 1);
        //            }
        //            // 3rd+ row
        //            else
        //            {
        //                // triangles on the right edge
        //                if (j == 1)
        //                {
        //                    triangles.Add(countHelper - (i + 1));
        //                    triangles.Add(triCounter - (i + (i - 1)));
        //                    triangles.Add(triCounter - i);
        //
        //                    triangles.Add(triCounter - (i + (i - 1)));
        //                    triangles.Add(triCounter - (i - 1));
        //                    triangles.Add(triCounter - i);
        //                }
        //                // triangles in the middle
        //                else if (j > 1 && j < i)
        //                {
        //                    triangles.Add(triCounter - (i + i));
        //                    triangles.Add(triCounter - (i + (i - 1)));
        //                    triangles.Add(triCounter - i);
        //
        //                    triangles.Add(triCounter - (i + (i - 1)));
        //                    triangles.Add(triCounter - (i - 1));
        //                    triangles.Add(triCounter - i);
        //                }
        //                // triangles on theleft edge
        //                else
        //                {
        //                    triangles.Add(triCounter - (i + j));
        //                    triangles.Add(countHelper_2);
        //                    triangles.Add(triCounter - i);
        //
        //                    triangles.Add(countHelper_2);
        //                    triangles.Add(countHelper_2 + (i + 1));
        //                    triangles.Add(triCounter - i);
        //
        //                    countHelper_2 += (i + 1);
        //                }
        //            }
        //
        //            triCounter++;
        //        }
        //    }
        //}
        #endregion

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }
}
