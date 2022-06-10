using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    int iterations = 0, maxIterations = 100;

    List<Tile> tiles = new List<Tile>();
    List<GameObject> critPath = new List<GameObject>();

    [Header("Tiles")]
    public GameObject tilePrefab;
    public GameObject[] waterTiles;
    public GameObject[] otherTiles;

    [Header("Edges")]
    [SerializeField, Tooltip("The Northern edges of the map where the ship begins its journey.")]         Edge[] i_Edges = null;
    [SerializeField, Tooltip("The Southern edges of the map where the ship's journey comes to a close.")] Edge[] o_Edges = null;

    public void PlaceBlankTiles(List<Node> nodes, int level)
    {
        int northL, northR, southL, southR;

        foreach(Tile t in tiles)
        {
            Destroy(t);
        }
        tiles.Clear();
        critPath.Clear();

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
            i_Edges.SetValue(tiles[a].edgeNW, tmp++);
            i_Edges.SetValue(tiles[a].edgeNE, tmp++);
        }

        tmp = 0;

        for(int b = southL; b <= southR; b++)
        {
            o_Edges.SetValue(tiles[b].edgeSW, tmp++);
            o_Edges.SetValue(tiles[b].edgeSE, tmp++);
        }

        WaterTilePlacement();
    }

    void WaterTilePlacement()
    {
        Edge start = i_Edges[Random.Range(0, i_Edges.Length)];  // Declare starting edge.
                                                                // Pick random water tile:
        GameObject currentTile = Instantiate(ChooseTile(true), start.thisTile.transform.position, Quaternion.identity);
        WaterTile w = currentTile.GetComponent<WaterTile>();    // Get the WaterTile script.
        bool alligned = false;                                  // Bool for checking if current is alligned w/ start.
        bool accepted = false;                                  // Bool for checking if next step has been found.

        // Remove unnecessary edges
        for (int s = 0; s < i_Edges.Length; s++) { if (i_Edges[s] != start) { Destroy(i_Edges[s]); } }

        int rnd = Random.Range(0, w.possibleEdges.Length);
        for (int h = 0; h < 6 && !alligned; h++)
        {
            for (int p = 0; p < w.possibleEdges.Length && !alligned; p++)
            {
                int q = (rnd + p) % w.possibleEdges.Length;
                if (Vector3.Distance(w.possibleEdges[q].transform.position, start.transform.position) <= 0.1f)
                {
                    critPath.Add(w.possibleEdges[q]);
                    alligned = true;
                    break;
                }
            }
            if (!alligned) { w.Turn(true); }    // Rotate WaterTile
        }

        rnd = Random.Range(0, w.possibleEdges.Length);
        for (int r = 0; r < w.possibleEdges.Length && !accepted; r++)
        {
            int q = (rnd + r) % w.possibleEdges.Length;
            if (!critPath.Contains(w.possibleEdges[q]))
            {
                critPath.Add(w.possibleEdges[q]);
                accepted = true;
            }
        }
        // --- TODO:////////////////////////////////////////////////
        //          OK, SO:
        //          Instead of doing it this way, it should
        //          select the path FIRST before placing
        //          any Prefabs in the Tiles.
        //          Once the path has been created
        //          (from a starting TILE--not edge--
        //          to an ending TILE--not edge),
        //          Begin laying prefabs down that line
        //          up their water edges with the previous
        //          & next edges in their list until @
        //          an ending Tile (again, not Edge).
        //              I THINK THIS WILL BE BETTER!
        //              IT WILL KEEP THE NO. OF ITERATIONS DOWN!
        //              BUT I WILL HAVE TO REWRITE SOME OF THIS NOW!
        //              OH WELL!!!
        ////////////////////////////////////////////////////////////

        // Check for coll. w/ EDGE
        //      !!! Check if that edge (or any, for that matter)
        //          are in the o_Edges list. !!! <--------- do this before???
        // Check if EDGE's Tile has a Prefab in it
        // Put prefab in the Tile.
        // Rotate to line up water with EDGE
        // Rinse, repeat.
    }

    GameObject ChooseTile(bool water) => water ? waterTiles[Random.Range(0, waterTiles.Length)] : 
                                                 otherTiles[Random.Range(0, otherTiles.Length)];
}
