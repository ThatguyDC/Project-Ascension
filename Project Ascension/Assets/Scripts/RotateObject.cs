using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{


    public Transform Center;

    [SerializeField] private float RotationSpeed; //speed of rotation movement
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }


    public void Rotate()
    {
        Vector3 center = GetComponent<Renderer>().bounds.center;
        transform.position -= center;
        Center.transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime); //rotate the object at a given vector with a given speed times delta T
        transform.position += center;
    }
}
