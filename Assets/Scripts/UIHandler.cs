using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI Handler script that stats or stops the voice recording
/// and updates the UI
/// </summary>
public class UIHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject voiceInputButton;

    [SerializeField]
    private MicrophoneManager microphoneManager;

    /// <summary>
    /// Starts the mic and saves audio input into audio clip on stop
    /// </summary>
    public void StartOrStopRecording()
    {
        if (!microphoneManager.IsRecording)
        {
            microphoneManager.StartRecording();
            voiceInputButton.GetComponent<Image>().enabled = true;
        }
        else
        {
            voiceInputButton.GetComponent<Image>().enabled = false;
            microphoneManager.StopRecording();
        }

    }
}
