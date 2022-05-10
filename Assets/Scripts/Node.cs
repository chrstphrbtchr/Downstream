using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] Node clockwiseNeighbor;
    [SerializeField] Node anticlockwiseNeighbor;
    [SerializeField] Vector3 location;
    [SerializeField] int ringSize;

    public Node(Vector3 pos)
    {
        this.location = pos;
    }

    #region Getters & Setters

    public Node GetClockwiseNeighbor() => clockwiseNeighbor;
    public Node GetAnticlockwiseNeighbor() => anticlockwiseNeighbor;
    public void SetClockwiseNeighbor(Node n) => this.clockwiseNeighbor = n;
    public void SetAnticlockwiseNeighbor(Node n) => this.anticlockwiseNeighbor = n;
    public Vector3 GetLocation() => location;
    public void SetLocation(Vector3 v) => this.location = v;
    public int GetRingSize() => ringSize;
    public void SetRingSize(int r) => this.ringSize = r;

    #endregion
}
