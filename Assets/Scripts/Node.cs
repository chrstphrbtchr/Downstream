using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node clockwiseNeighbor;
    public Node anticlockwiseNeighbor;
    public Vector3 location;

    public Node(Vector3 pos)
    {
        this.location = pos;
    }
}
