using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    public LineRenderer LineRenderer;
    [SerializeField] private int Subdivisions = 20;
    [SerializeField] private float Radius = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float AngleStep = 2.5f * Mathf.PI / Subdivisions;
        LineRenderer.positionCount = Subdivisions;

        for (int i = 0; i < Subdivisions; i++) 
        
        {
        float XPosition = Radius * Mathf.Cos(AngleStep * i);
        float ZPosition = Radius * Mathf.Sin(AngleStep * i);

        Vector3 PointInCircle = new Vector3(XPosition, 0f, ZPosition);

        LineRenderer.SetPosition(i, PointInCircle);

        }
    }
}
