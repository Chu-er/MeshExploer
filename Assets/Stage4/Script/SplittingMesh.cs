using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SplittingMesh : MonoBehaviour
{
    public int xSize, ySize, zSize;
    public int Roundness;

    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;


    void Start()
    {
        GenerateVertices();
        GenerateTriangles();
    }
    /// <summary>
    /// 生成顶点
    /// </summary>
    void GenerateVertices()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube ";

        int cornerVertices = 8; //八个角 八个顶点
        int edgeVertices = (xSize + ySize + zSize - 3) * 4; //不考虑顶点公用的话 应该是 (xSize+ySize+zSize +1)*4
        int faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)
         ) * 2;
        int vCount = cornerVertices + edgeVertices + faceVertices;
        vertices = new Vector3[vCount];
        normals = new Vector3[vertices.Length];
        //Debug.Log("顶点数量" + vCount);

        int v = 0;
        //赋予前后左右四个面顶点
        for (int y = 0; y <= ySize; y++)
        {
            //前面
            for (int x = 0; x <= xSize; x++)
            {
                SetVertex(v++, x, y, 0);
            }
            //右面 一开始  因为上面已经把最后一个点 公共顶点绘制完成
            for (int z = 1; z <= zSize; z++)
            {
                SetVertex(v++, xSize, y, z);
            }
            //后面
            for (int x = xSize - 1; x >= 0; x--)
            {
                SetVertex(v++, x, y, zSize);
            }
            //左面
            for (int z = zSize - 1; z > 0; z--)
            {
                SetVertex(v++, 0, y, z);

            }
        }

        //赋予上面顶点
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, ySize, z);
            }
        }
        //赋予下面顶点
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
            {
                SetVertex(v++, x, 0, z);
            }
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    /// <summary>
    /// 计算顶点 法线
    /// </summary>
    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);
        if (x < Roundness)
        {
            inner.x = Roundness;
        }
        else if (x > xSize - Roundness)
        {
            inner.x = xSize - Roundness;
        }
        if (y < Roundness)
        {
            inner.y = Roundness;
        }
        else if (y > ySize - Roundness)
        {
            inner.y = ySize - Roundness;
        }
        if (z < Roundness)
        {
            inner.z = Roundness;
        }
        else if (z > zSize - Roundness)
        {
            inner.z = zSize - Roundness;
        }

        normals[i] = (vertices[i] - inner).normalized;

        vertices[i] = inner + normals[i] * Roundness;
    }

    void GenerateTriangles()
    {

        int[] trianglesZ = new int[xSize * ySize * 12];
        int[] trianglesX = new int[ySize*zSize*12];
        int[] trianglesY = new int[xSize * zSize * 12];
        int ring = (xSize + zSize) * 2;
        int tZ = 0, tX = 0, tY = 0  ,v = 0; 


    }

    /// <summary>
    /// 创建上面
    /// </summary>
    /// <returns></returns>
    int CreateTopFace(int[] trinagles, int t, int ring)
    {
        int v = ring * ySize;
        for (int x = 0; x < xSize - 1; x++, v++)
        {
            t = SetQuad(trinagles, t, v, v + 1, v + ring - 1, v + ring);
        }
        t = SetQuad(trinagles, t, v, v + 1, v + ring - 1, v + 2);

        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = v + 2;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMid++, vMax++)
        {
            t = SetQuad(trinagles, t, vMin, vMid, vMin - 1, vMid + xSize - 1);
            for (int x = 1; x < xSize - 1; x++, vMid++)
            {
                t = SetQuad(trinagles, t, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
            }

            t = SetQuad(trinagles, t, vMid, vMax, vMid + xSize - 1, vMax + 1);
        }
        //最后一行
        int vTop = vMin - 2;
        t = SetQuad(trinagles, t, vMin, vMid, vTop + 1, vTop);
        for (int x = 1; x < xSize - 1; x++, vTop--, vMid++)
        {
            t = SetQuad(trinagles, t, vMid, vMid + 1, vTop, vTop - 1);
        }
        t = SetQuad(trinagles, t, vMid, vTop - 2, vTop, vTop - 1);

        return t;
    }

    /// <summary>
    /// 创建底面
    /// </summary>
    /// <param name="trinagles"></param>
    /// <param name="t"></param>
    /// <param name="ring"></param>
    /// <returns></returns>
    int CreateBottomFace(int[] trinagles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);
        int vOriginMid = vMid;

        t = SetQuad(trinagles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++, vMid++, v++)
        {
            t = SetQuad(trinagles, t, vMid, vMid + 1, v, v + 1);
        }
        t = SetQuad(trinagles, t, vMid, v + 2, v, v + 1);

        /////////////////
        int vMin = ring - 2;
        int vMax = v + 2;
        vMid++;

        for (int z = 1; z < zSize - 1; z++, vMin--, vMax++, vMid++, vOriginMid++)
        {
            t = SetQuad(trinagles, t, vMin, vMid, vMin + 1, vOriginMid);
            for (int x = 1; x < xSize - 1; x++, vMid++, vOriginMid++)
            {
                t = SetQuad(trinagles, t, vMid, vMid + 1, vOriginMid, vOriginMid + 1);
            }
            t = SetQuad(trinagles, t, vMid, vMax + 1, vOriginMid, vMax);
        }

        int vTop = vMin - 1;

        t = SetQuad(trinagles, t, vTop + 1, vTop, vTop + 2, vOriginMid);
        for (int x = 1; x < xSize - 1; x++, vOriginMid++, vTop--)
        {
            t = SetQuad(trinagles, t, vTop, vTop - 1, vOriginMid, vOriginMid + 1);
        }

        t = SetQuad(trinagles, t, vTop, vTop - 1, vOriginMid, vTop - 2);
        return t;
    }



    /// <summary>
    /// 我们封装这个 三角形拼接四边形的方法 返回三角形的下一轮第一个索引  参数v00 是顶点的索引
    /// </summary>
    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 5] = v11;
        return i + 6;
    }


    private void OnDrawGizmos()
    {
        if (vertices == null) return;

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[i], 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[i], normals[i]);
        }

    }


}
