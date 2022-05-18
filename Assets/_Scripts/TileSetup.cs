using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    List<Tile> tiles = new List<Tile>();

    [Header("Tiles")]
    public GameObject tilePrefab;
    public GameObject[] waterTiles;
    public GameObject[] otherTiles;

    [Header("Edges")]
    [SerializeField] Edge[] i_Edges;
    [SerializeField] Edge[] o_Edges;

    public void PlaceBlankTiles(List<Node> nodes, int level)
    {
        int northL, northR, southL, southR;

        foreach(Tile t in tiles)
        {
            Destroy(t);
        }
        tiles.Clear();

        //tilePrefab = Resources.Load("TilePrefab", typeof(GameObject)) as GameObject;

        for(int i = 0; i < nodes.Count; i++)
        {
            Tile t = Instantiate(tilePrefab, nodes[i].GetLocation(), Quaternion.identity).GetComponent<Tile>();
            t.gameObject.name = "Tile " + i;
            t.thisNode = nodes[i];
            t.gameObject.transform.position = nodes[i].GetLocation();
            tiles.Add(t);
        }

        // Calculate in & out tiles
        northL = (level * level * 3) + (level + 1);
        northR = northL + level;
        southR = (level * level * 3) - (level - 1);
        southL = southR - level;

        int tmp = 0;
        i_Edges = new Edge[2 * (level + 1)];
        o_Edges = new Edge[2 * (level + 1)];

        for(int a = northL; a <= northR; a++)
        {
            i_Edges[tmp++] = tiles[a].edgeNW;
            i_Edges[tmp++] = tiles[a].edgeNE;
        }

        tmp = 0;

        for(int b = southL; b <= southR; b++)
        {
            o_Edges[tmp++] = tiles[b].edgeSE;
            o_Edges[tmp++] = tiles[b].edgeSW;
        }

        WaterTilePlacement();
    }

    void WaterTilePlacement()
    {
        // Declare start tile.
        Edge start = i_Edges[Random.Range(0, i_Edges.Length)];

    }
}
