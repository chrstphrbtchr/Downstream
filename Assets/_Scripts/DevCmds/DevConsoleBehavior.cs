using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class DevConsoleBehavior : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

    [Header("UI!")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField, Tooltip("TOOLTIP!")] private TMP_InputField inputField = null;

    private float pausedTimeScale;

    private static DevConsoleBehavior instance;
    private DeveloperConsole dc;
    private DeveloperConsole DevCon
    {
        get
        {
            if(dc != null) { return dc; }
            return dc = new DeveloperConsole(prefix, commands);
        }
    }

    private void Awake()
    {
        // Singleton stuff
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void Toggle(InputAction.CallbackContext contxt)
    {
        if(!contxt.action.triggered) { return; }
        if (uiCanvas.activeSelf)
        {
            Time.timeScale = pausedTimeScale;
            uiCanvas.SetActive(false);
        }
        else
        {
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        DevCon.ProcessCommand(inputValue);
        inputField.text = string.Empty;
    }
}
