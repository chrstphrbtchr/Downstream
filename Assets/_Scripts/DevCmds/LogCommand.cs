using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Log Command", menuName = "Log Command")]
public class LogCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        string logtxt = string.Join(" ", args);
        Debug.Log(logtxt);
        return true;
    }
}
