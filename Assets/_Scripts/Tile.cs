using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Node thisNode;
    public Edge[] edges;
    GameObject occupyingTile;

    public void SetOccupyingTile(GameObject g) => this.occupyingTile = g;
    public GameObject GetOccupyingTile() => this.occupyingTile;
}
