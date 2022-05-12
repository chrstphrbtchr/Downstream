using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField] private string commandWord = "";
    public string CommandWord => commandWord;

    public abstract bool Process(string[] args);
}
