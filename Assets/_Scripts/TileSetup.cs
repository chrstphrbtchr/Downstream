using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    static GameObject tilePrefab;
    static List<Tile> tiles = new List<Tile>();
    static GameObject[] waterTiles;
    static GameObject[] otherTiles;

    public static void PlaceBlankTiles(List<Node> nodes)
    {
        foreach(Tile t in tiles)
        {
            Destroy(t);
        }
        tiles.Clear();

        tilePrefab = Resources.Load("TilePrefab", typeof(GameObject)) as GameObject;

        for(int i = 0; i < nodes.Count; i++)
        {
            Tile t = Instantiate(tilePrefab, nodes[i].GetLocation(), Quaternion.identity).AddComponent<Tile>();
            t.gameObject.name = "Tile " + i;
            //Tile t = new GameObject("Tile " + i, typeof(Tile)).GetComponent<Tile>();
            t.thisNode = nodes[i];
            t.gameObject.transform.position = nodes[i].GetLocation();
            tiles.Add(t);
        }

        waterTiles = Resources.LoadAll("Tiles/Water", typeof (GameObject)) as GameObject[];
        otherTiles = Resources.LoadAll("Tiles/Other", typeof (GameObject)) as GameObject[];

    }

    static void WaterTilePlacement()
    {

    }
}
