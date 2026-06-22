using UnityEngine;
using TMPro;

public class HandGestureUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_LeftStatusText;
    [SerializeField] TextMeshProUGUI m_RightStatusText;
    [SerializeField] TextMeshProUGUI m_GeneralDebugText;

    private string m_LeftGesture = "None";
    private string m_RightGesture = "None";
    private string m_LeftMode = "Unknown";
    private string m_RightMode = "Unknown";

    public void UpdateLeftMode(string mode)
    {
        m_LeftMode = mode;
        UpdateUI();
    }

    public void UpdateRightMode(string mode)
    {
        m_RightMode = mode;
        UpdateUI();
    }

    public void SetLeftGesture(string gestureName)
    {
        m_LeftGesture = gestureName;
        UpdateUI();
    }

    public void SetRightGesture(string gestureName)
    {
        m_RightGesture = gestureName;
        UpdateUI();
    }

    public void ClearLeftGesture()
    {
        m_LeftGesture = "None";
        UpdateUI();
    }

    public void ClearRightGesture()
    {
        m_RightGesture = "None";
        UpdateUI();
    }
    
    public void SetDebugText(string text)
    {
        if (m_GeneralDebugText != null)
            m_GeneralDebugText.text = text;
    }

    void UpdateUI()
    {
        if (m_LeftStatusText != null)
            m_LeftStatusText.text = $"Left Hand: {m_LeftGesture} ({m_LeftMode})";
        if (m_RightStatusText != null)
            m_RightStatusText.text = $"Right Hand: {m_RightGesture} ({m_RightMode})";
    }
}
