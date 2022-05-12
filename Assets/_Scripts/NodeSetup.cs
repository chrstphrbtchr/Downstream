using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeSetup : MonoBehaviour
{
    Vector2[] nodeDistances = new Vector2[]
    {
        new Vector2( 0.5f, -0.866f),
        new Vector2(   1f,      0f),
        new Vector2( 0.5f,  0.866f),
        new Vector2(-0.5f,  0.866f),
        new Vector2(  -1f,      0f),
        new Vector2(-0.5f, -0.866f)
    };

    List<Node> nodes = new List<Node>();

    // Start is called before the first frame update
    void Start()
    {
        CreateGameboard(3);
    }

    void CreateGameboard(int level)
    {
        nodes.Clear();

        Node current = new Node(Vector3.zero);
        nodes.Add(current);

        int sideLen = 1;
        for (int l = 1; l <= level; l++)
        {
            Node first = new Node(new Vector3(-(l), 0, 0));
            first.SetRingSize(l);
            current = first;
            nodes.Add(current);
            // Left
            for (int a = 0; a < nodeDistances.Length; a++)
            {
                for (int i = 0; i < (a < nodeDistances.Length - 1 ? sideLen : sideLen - 1); i++)
                {
                    Vector3 newPos = new Vector3(current.GetLocation().x + nodeDistances[a].x, 
                        current.GetLocation().y, current.GetLocation().z + nodeDistances[a].y);
                    Node newNode = new Node(newPos);
                    newNode.SetRingSize(l);
                    nodes.Add(newNode);
                    newNode.SetClockwiseNeighbor(current);
                    current.SetAnticlockwiseNeighbor(newNode);
                    current = newNode;
                }
            }
            current.SetAnticlockwiseNeighbor(first);
            sideLen++;
        }

        TileSetup.PlaceBlankTiles(nodes);
        // Create node @ 0,0,0
        // then create one node at -1, 0, 0
        //      a = 1
        // that node links up SE a,
        //                    E  a,
        //                    NE a,
        //                    NW a,
        //                    W  a, &
        //                    SW a.
        // Each node assigns its right parter
        //  the last one assigns the first in the series.
        //      & only goes level - 1 nodes.
        // do this for level number of times,
        //      a++ each loop.

        // Once nodes have been created, set down tiles for the path
        //      rotate these clockwise a number of times (0 - 5)
        //      each path tile has a number of turns (0 - 3) necessary
        //      to be "correct", but the max # of moves is (tiles in path) + (total turns) + 1
        //      (this can be changed)
        // - this (+ the next) should be in the TileSetup script, which takes an array of nodes?

        // once the path has been created, fill in all nodes with
        //  tiles, rotating them 0 - 5x 
        
        // Once this is complete, the level can start.
    }
}
