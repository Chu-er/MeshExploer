using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class SampleMesh : MonoBehaviour
{

    public int xSize, ySize;
    private Vector3[]  vertices ;

    private Mesh mesh;
    void Start()
    {
         Generate ();
    }

    /// <summary>
    /// 生成顶点
    /// </summary>
     void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";
        int vCount = (xSize + 1) * (ySize+1);
        vertices = new Vector3[vCount];
        Vector2[] uv = new Vector2[vCount];
        Vector4[] trangent = new Vector4[vCount];  Vector4 unityTangent = new Vector4(1,0,0,-1);
        //顶点和UV ,切线一起算
        for (int y = 0 ,i=0 ; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++,i++)
            {
                vertices[i] =new  Vector3(x,y);
                uv[i] = new Vector2((float)x/xSize, (float)y/ySize);
                trangent[i] = unityTangent;
            }
        }


        int[] triangles = new int[xSize*ySize *6];
        //********注意 顶点索引在进入下一行要加1 因为每行的最后一个顶点不是下一个正方形的第一个点
        for (int y = 0 , ti = 0 , vi = 0 ; y < ySize; y++ ,vi++)
        {
            //画第一行 ti 顶点索引的迭代 六个一个循环  vi 顶点的索引 0开始  第0个正方形 从0 开始  第1个正方形 顶点索引从1开始
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                triangles[ti + 1] = triangles[ti + 4] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
                //Debug.Log("Current :VI" + vi);
            }
        }

        mesh.vertices = this.vertices;
        mesh.tangents = trangent;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();//自动计算法线
    }


    private void OnDrawGizmos()
    {
        if (vertices==null) return;

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i],0.1f);
        }
    }

}
