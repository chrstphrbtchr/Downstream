using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{
    public GameObject[] possibleEdges;

    public void Turn(bool clockwise) => this.transform.RotateAround(transform.position, 
        transform.TransformDirection(Vector3.up), (clockwise ? 30 : -30) ); 
}
