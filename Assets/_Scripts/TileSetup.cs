using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    static List<Tile> tiles = new List<Tile>();

    public static void PlaceBlankTiles(List<Node> nodes)
    {
        foreach(Tile t in tiles)
        {
            Destroy(t);
        }
        tiles.Clear();

        for(int i = 0; i < nodes.Count; i++)
        {
            Tile t = new GameObject("Tile " + i, typeof(Tile)).GetComponent<Tile>();
            t.thisNode = nodes[i];
            t.gameObject.transform.position = nodes[i].GetLocation();
            tiles.Add(t);
        }


    }

    static void WaterTilePlacement()
    {

    }
}
