using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDefomer : MonoBehaviour
{


    public float springForce = 2f;
    public float damp;
    Mesh defomingMesh;

    Vector3[] originalVertices, displacedVertices;


    Vector3[] verticesVelocties; //每个顶点的速度
    void Start()
    {
        defomingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = defomingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        verticesVelocties = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }

        MaterialPropertyBlock block = new MaterialPropertyBlock();
        Vector3 toColorP = Random.onUnitSphere;
        block.SetColor("_Color", new Color(toColorP.x, toColorP.y, toColorP.z) );
        GetComponent<Renderer>().SetPropertyBlock(block);
    }

    private void Update()
    {
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            updateVertex(i);
        }

        defomingMesh.vertices = displacedVertices;
        defomingMesh.RecalculateNormals();
    }

    void updateVertex(int index)
    {
        Vector3 velocity = verticesVelocties[index];
        Vector3 displacedMent = displacedVertices[index] - originalVertices[index];//与原来点的 相对位置
        velocity -= displacedMent * springForce * Time.deltaTime;//回弹
        velocity *= 1 - damp * Time.deltaTime;
        verticesVelocties[index] = velocity;
        displacedVertices[index] += velocity * Time.deltaTime;
    }
    public void AddDefomingFoce(Vector3 point,  float force)
    {
        Debug.DrawLine(Camera.main.transform.position, point, Color.magenta);
        Vector3 localPoint = transform.InverseTransformPoint(point);
        for (int i = 0; i < displacedVertices.Length; i++)
        {
            addForce2Vertex(i, localPoint, force);
        }
    }
    void addForce2Vertex(int index, Vector3 point , float force)
    {
        Vector3 pointToVertex = displacedVertices[index] - point;// 方向

        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude);
        float velocty = attenuatedForce * Time.deltaTime; //
        verticesVelocties[index] += pointToVertex.normalized * velocty;
    }
}
