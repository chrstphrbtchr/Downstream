using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeSetup : MonoBehaviour
{
    const float zDiff = 0.866f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void CreateGameboard(int level)
    {
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
