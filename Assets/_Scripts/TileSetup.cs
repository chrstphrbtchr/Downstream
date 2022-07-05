using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class TileSetup : MonoBehaviour
{
    private void Update()// delete me?
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(0);
        }
    }

    int iterations = 0, maxIterations = 10;

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
            Testing(currentPath, true);
            Debug.LogError("End not found.");
            // In this case, restart the level and try again.
        }
        else
        {
            Testing(currentPath, false);
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
                    // instantiate tiles[j]
                    // rotate it so tiles[j].possiblesides[waterTileIndex] is at edges[i]
                    // if edges[i-1] does not overlap with a water tile's edge:
                    // rotate it so tiles[j].possiblesides[waterTileIndex] is at edges[i - 1] instead.
                    matchingTileFound = true;
                }
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

    GameObject ChooseTile(bool water) => water ? waterTiles[Random.Range(0, waterTiles.Length)] : 
                                                 otherTiles[Random.Range(0, otherTiles.Length)];

    int WaterTileFitsSpace(GameObject tile, Edge inEdge, Edge outEdge)
    {
        int result = -1;
        // Determine if the given Water Tile can connect
        //  the given Edges
        WaterTile wt = tile.GetComponent<WaterTile>();
        bool found = false;
        int r = Random.Range(0, wt.possibleEdges.Length);
        for(int e = 0; e < wt.possibleEdges.Length && !found; e++)
        {
            int i = (e + r) % wt.possibleEdges.Length;
            for(int f = e + 1; f < wt.possibleEdges.Length && !found; f++)
            {
                
            }
        }
        return result;
    }
}
