using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class ComboDisplay : MonoBehaviour
    {
        [SerializeField] private Image[] m_StepImages;
        [SerializeField] private Image m_CooldownImage;

        public Image[] StepImages => m_StepImages;
        public Image CooldownImage => m_CooldownImage;

        public void SetStepStatus(int stepIndex, bool active)
        {
            if (stepIndex >= 0 && stepIndex < m_StepImages.Length)
            {
                m_StepImages[stepIndex].color = active ? Color.yellow : Color.white;
                
                // If it has a highlight outline child, enable/disable it
                Transform highlight = m_StepImages[stepIndex].transform.Find("HighlightOutline");
                if (highlight != null)
                {
                    highlight.gameObject.SetActive(active);
                }
            }
        }

        public void ResetSteps()
        {
            foreach (var img in m_StepImages)
            {
                img.color = Color.white;
                Transform highlight = img.transform.Find("HighlightOutline");
                if (highlight != null)
                {
                    highlight.gameObject.SetActive(false);
                }
            }
        }
    }
}
