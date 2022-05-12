using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeveloperConsole
{
    private readonly string prefix;
    private readonly IEnumerable<IConsoleCommand> commands;
    public DeveloperConsole(string pre, IEnumerable<IConsoleCommand> cmds)
    {
        this.prefix = pre;
        this.commands = cmds;
    }

    public void ProcessCommand(string inputVal)
    {
        if(!inputVal.StartsWith(prefix)) { return; }
        inputVal = inputVal.Remove(0, prefix.Length);
        string[] inputSplit = inputVal.Split(' ');
        string cmdInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();

        ProcessCommand(cmdInput, args);
    }

    public void ProcessCommand(string cmdInput, string[] args)
    {
        foreach(var command in commands)
        {
            if(!cmdInput.Equals(command.CommandWord, System.StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (command.Process(args))
            {
                return;
            }
        }
    }
}
