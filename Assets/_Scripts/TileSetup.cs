using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TileSetup : MonoBehaviour
{
    private void Update()// delete me?
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(0);
        }
#endif
    }

    const int maxIterations = 6;

    List<Tile> tiles  = new List<Tile>();
    List<GameObject> critPath = new List<GameObject>();

    [Header("Tiles")]
    public GameObject tilePrefab;
    public GameObject[] waterTiles;
    public GameObject[] otherTiles;

    [Header("Edges")]
    [SerializeField] Edge[] i_Edges = null;


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

        CreatePath(level, 1);
    }

    void CreatePath(int level, int iteration)
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
                        currentPath.Add(nextEdge);
                        currentEdge = nextEdge;
                        nextEdge = null;
                        sideFound = true;
                    }
                }
            }

            if (!sideFound)
            {
                // Remove unnecessary Tile & Edge from their respective lists.
                tPath.RemoveAt(tPath.Count - 1);
                currentTileIndex = (currentTileIndex > 1 ? (currentTileIndex - 1) : 0);
                // TODO: Make this better. In event of failure, we end up with like
                //       two-hundred points on top of one another.
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
                // =====================================TODO:=====================================
                //  MAYYYYYBE ADD A COIN TOSS W/ INC. PROB. TO SEE IF *THIS* END TILE IS THE END?
                // =======================================?=======================================
                //Debug.Log("<color=cyan>OK!</color>");
            }

            //                  TODO:   In case of no edge partner
            //                          In case of overlap
            // endFound must be checked!
        }
        if (!endFound)
        {
            if(iteration < maxIterations)
            {
                CreatePath(level, ++iteration);
            }
            else
            {
                Debug.LogError("End not found.");
                // FAIL OUT TO TITLE W/O PENALTY.
            }
        }
        else
        {
#if UNITY_EDITOR
            Testing(currentPath, false);
            for(int i = 0; i < currentPath.Count; i++)
            {
                Debug.LogFormat("<color=magenta>{0}</color>(<color=yellow>{2}</color>) @ <color=cyan>{1}</color>", currentPath[i].thisTile, currentPath[i].transform.position, currentPath[i].name);
            }
#endif
            WaterTilePlacement(edges: currentPath);
        }
    }

    private void Testing(List<Edge> currentPath, bool failed)
    {
        int i = currentPath.Count;
        int j = currentPath.Count;
        foreach(Edge edge in currentPath)
        {
            GameObject g = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), edge.transform.position, edge.transform.rotation);
            g.transform.localScale = new Vector3(1f, 1f, 1f)/4f;
            Color theColor = failed ? Color.Lerp(Color.black, Color.red, (float)i / j) : Color.Lerp(Color.magenta, Color.cyan, (float)i / j);
            g.GetComponent<MeshRenderer>().material.color = theColor;
            var iconContent = EditorGUIUtility.IconContent("sv_label_1");
            g.name = "POINT " + (j - i);
            EditorGUIUtility.SetIconForObject(g, (Texture2D)iconContent.image);
            i--;
        }
    }

    void WaterTilePlacement(List<Edge> edges)
    {
        for(int i = 1; i < edges.Count; i++)
        {
            bool matchingTileFound = false;
            int rand = Random.Range(0, waterTiles.Length);
            for(int j = 0; j < waterTiles.Length && !matchingTileFound; j++)
            {
                int r = (j + rand) % waterTiles.Length;
                int waterTileIndex;
                if((waterTileIndex = WaterTileFitsSpace(waterTiles[r], edges[i - 1], edges[i])) >=0)
                {
                    Debug.LogFormat("<color=green>YEAH</color>: {0}", i);
                    // instantiate tiles[j]
                    // rotate it so tiles[j].possiblesides[waterTileIndex] is at edges[i]
                    // if edges[i-1] does not overlap with a water tile's edge:
                    // rotate it so tiles[j].possiblesides[waterTileIndex] is at edges[i - 1] instead.
                    matchingTileFound = true;
                }
            }
            if (!matchingTileFound)
            {
                
            }
            // check i-1 & i against a random water tile.
            // perhaps this could be multithreaded?
            // rotate the tile until one of the water sides matches with i
            // check to see if i - 1 matches with another
            // if not, rotate
            // after 5 rotations, discard & delete.
            // tiles++ % tiles.count
            // once found, move on to the next one.
            // once all have been found, rejoice!
            // THEN ROTATE EACH OF THEM A NUMBER OF TIMES
            // THEN ROTATE EACH OF THEIR LOOPS AS WELL.
            // THEN ADD WATER TILES FOR REMAINING WATER SIDES.
            // (until no more water sides not touching a filled tile)
            // THEN TWIST ALL OF THOSE
        }
    }

    /// <summary>
    /// Selects a random tile from the array. If water is true will select only water tiles.
    /// </summary>
    /// <param name="water">Whether returning a water tile specifically.</param>
    /// <returns>A random Tile (as a GameObject).</returns>
    GameObject ChooseTile(bool water) => water ? waterTiles[Random.Range(0, waterTiles.Length)] : 
                                                 otherTiles[Random.Range(0, otherTiles.Length)];
    /// <summary>
    /// If the given two Edges can be mapped onto the given Tile (as a GameObject),
    /// returns the index of an appropriate Edge in the given Tile.
    /// </summary>
    /// <param name="tile">The Tile that is being tested.</param>
    /// <param name="inEdge">One of the edges to be tested against.</param>
    /// <param name="outEdge">Another of the edges to be tested against.</param>
    /// <returns>Returns the index of an appropriate Edge within the WaterTile of the given Tile.</returns>
    int WaterTileFitsSpace(GameObject tile, Edge inEdge, Edge outEdge)
    {
        if (tile == null || inEdge == null || outEdge == null)
        {
            Debug.LogError("WaterTileFitsSpace error: one or more inputs invalid.");
            return -232;
        }
        float threshold = 0.1f;     // The threshold for a corresponding match.
        WaterTile wt = tile.GetComponent<WaterTile>();      // The WaterTile of the given Tile GameObject.
        int r = Random.Range(0, wt.possibleEdges.Length);   // Random Edge to start looking at.

        // The distance between the two input Edges.
        float inputDistance = Vector2.Distance(inEdge.transform.position, outEdge.transform.position);

        for(int e = 0; e < wt.possibleEdges.Length; e++)
        {
            int i = (e + r) % wt.possibleEdges.Length;

            for(int f = e + 1; f < wt.possibleEdges.Length; f++)
            {
                int j = (f + r) % wt.possibleEdges.Length;
                float possibleDistance = Vector2.Distance(wt.possibleEdges[i].transform.position,
                                                            wt.possibleEdges[j].transform.position);
                if (Mathf.Abs(possibleDistance - inputDistance) <= threshold)    
                {
                    return i;
                }
            }
        }
        //Debug.LogFormat("{0} & {1} : <color=yellow>{2}</color>", inEdge.thisTile, outEdge.thisTile, inputDistance);
        return -1;
    }
}
