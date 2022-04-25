using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDeformeInput : MonoBehaviour
{
    public float force;
    public float forceOffset = 0.1f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            handleInput();
        }
    }

    void handleInput()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast( ray,  out RaycastHit info , 150f))
        {
            MeshDefomer meshDefomer = info.collider.GetComponent<MeshDefomer>();
            if (meshDefomer)
            {
                Vector3 point = info.point;
                point += info.normal * forceOffset;
                meshDefomer.AddDefomingFoce(point , force);
            }
        }
    }
}
