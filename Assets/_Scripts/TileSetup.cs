using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSetup : MonoBehaviour
{
    int iterations = 0, maxIterations = 100;

    List<Tile> tiles  = new List<Tile>();
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

        CreatePath(level);

        //WaterTilePlacement();
    }

    void CreatePath(int level)
    {
        int northL, northR, southL, southR;
        bool endFound = false;
        
        List<Tile> tPath = new List<Tile>();        // The path of Tiles to get from top to bottom.
        List<Tile> checkedTiles = new List<Tile>(); // All checked Tiles (so as to not check multiple times).
        List<Edge> currentPath = new List<Edge>();  // The current path of Edges to follow for the Tile path.

        // Calculate in & out tiles
        northL = (level * level * 3) + (level + 1);
        northR = northL + level;
        southR = (level * level * 3) - (level - 1);
        southL = southR - level;

        int tmp = 0;
        i_Edges = new Edge[2 * (level + 1)];        // Edges at the top of the map to be used as the starting point.
        List<Tile> exitTiles = new List<Tile>();    // Tiles at the bottom of the map whose SE/SW Edge will be used as the exit.

        for (int a = northL; a <= northR; a++)
        {
            i_Edges.SetValue(tiles[a].edges[0], tmp++);
            i_Edges.SetValue(tiles[a].edges[1], tmp++);
        }

        for (int b = southL; b <= southR; b++)
        {
            exitTiles.Add(tiles[b]);
        }

        Edge currentEdge = i_Edges[Random.Range(0, i_Edges.Length)];    // Declare starting Edge.
        Edge nextEdge = null;                                           // Init. next Edge.
        tPath.Add(currentEdge.GetTile());                               // Add Tile to path.
        currentPath.Add(currentEdge);                                   // Add Edge to path
        int currentTileIndex = 0;                                       // Init. index for backtracking.
        for(int e = 0; e < (tiles.Count * 6) && !endFound; e++)
        {
            bool sideFound = false;
            int rnd = Random.Range(0, 6);
            for(int s = 0; s < 6 && !sideFound; s++)
            {
                int q = (rnd + s) % 6;
                //Debug.LogFormat("<color=magenta>E: {0}, CTI: {4} :: Q: {1} = RND: {2} + S: {3}</color>", e, q, rnd,s, currentTileIndex);// delete.
                currentEdge = tPath[currentTileIndex].edges[q];
                nextEdge = currentEdge.EdgeParter();
                if(nextEdge != null)
                {
                    if (!tPath.Contains(nextEdge.GetTile()))
                    {
                        tPath.Add(nextEdge.GetTile());
                        currentPath.Add(currentEdge);
                        currentEdge = nextEdge;
                        nextEdge = null;
                        sideFound = true;
                    }
                }
            }

            if (!sideFound)
            {
                //Debug.LogError("SIDE NOT FOUND.");

                // Remove unnecessary Tile & Edge from their respective lists.
                tPath.RemoveAt(tPath.Count - 1);
                currentTileIndex = (currentTileIndex > 1 ? (currentTileIndex - 1) : 0);
            }
            else
            {
                currentTileIndex++;
            }

            if (exitTiles.Contains(currentEdge.GetTile()))
            {
                endFound = true;
                int coin = Random.Range(0, 2);
                currentPath.Add((coin < 1 ? currentEdge.GetTile().edges[3] : currentEdge.GetTile().edges[4]));
                //Debug.Log("<color=cyan>OK!</color>");
            }

            //                  TODO:   In case of no edge partner
            //                          In case of overlap
            // endFound must be checked!
        }
        if (!endFound)
        {
            Debug.LogError("End not found.");
            // In this case, restart the level and try again.
        }
        else
        {
            WaterTilePlacement();
        }
    }

    void WaterTilePlacement()
    {
        
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

    bool WaterTileFitsSpace(WaterTile wt, Edge inEdge, Edge outEdge)
    {
        bool result = false;
        // Determine if the given Water Tile can connect
        //  the given Edges.
        return result;
    }
}
