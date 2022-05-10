using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    static List<Tile> tiles = new List<Tile>();

    public static void PlaceBlankTiles(Node[] nodes)
    {
        foreach(Tile t in tiles)
        {
            Destroy(t);
        }
        tiles.Clear();

        for(int i = 0; i < nodes.Length; i++)
        {

        }
    }

}
