using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    public class FightingHUDManager : MonoBehaviour
    {
        [Header("Player 1")]
        public Image p1HealthBar;
        public TextMeshProUGUI p1HealthText;
        public TextMeshProUGUI p1Name;
        public GameObject[] p1Wins;

        [Header("Player 2")]
        public Image p2HealthBar;
        public TextMeshProUGUI p2HealthText;
        public TextMeshProUGUI p2Name;
        public GameObject[] p2Wins;

        [Header("Timer")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI winnerText;
        public float gameTime = 180f;
        private bool m_TimerActive = false;

        public static FightingHUDManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            if (winnerText != null) winnerText.gameObject.SetActive(false);
        }

        private void Start()
        {
            // Force standard fighting game fill directions
            if (p1HealthBar != null)
            {
                p1HealthBar.type = Image.Type.Filled;
                p1HealthBar.fillMethod = Image.FillMethod.Horizontal;
                p1HealthBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
            if (p2HealthBar != null)
            {
                p2HealthBar.type = Image.Type.Filled;
                p2HealthBar.fillMethod = Image.FillMethod.Horizontal;
                p2HealthBar.fillOrigin = (int)Image.OriginHorizontal.Right;
            }
        }

        public void StartTimer()
        {
            m_TimerActive = true;
            gameTime = 180f;
            if (winnerText != null) winnerText.gameObject.SetActive(false);

            // 초기 체력 바를 200 / 200 으로 리셋
            UpdateHealth(0, 200f, 200f);
            UpdateHealth(1, 200f, 200f);
        }

        public void StopTimer()
        {
            m_TimerActive = false;
        }

        public void DisplayWinner(string message)
        {
            if (winnerText != null)
            {
                winnerText.text = message;
                winnerText.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (m_TimerActive && gameTime > 0)
            {
                gameTime -= Time.deltaTime;
                timerText.text = Mathf.CeilToInt(gameTime).ToString();
                if (gameTime <= 0)
                {
                    gameTime = 0;
                    m_TimerActive = false;
                    // Handle time out
                }
            }
        }

        public void UpdateHealth(int playerIndex, float currentHealth, float maxHealth)
        {
            // 체력 비율 계산 (0 ~ 1)
            float healthPercent = maxHealth > 0 ? Mathf.Clamp01(currentHealth / maxHealth) : 0f;

            // 화면에 보여줄 텍스트 설정 (예: HP 200 / 200)
            string hpString = $"HP {currentHealth:0} / {maxHealth:0}";

            if (playerIndex == 0)
            {
                if (p1HealthBar != null)
                {
                    p1HealthBar.fillAmount = healthPercent;
                }
                if (p1HealthText != null) p1HealthText.text = hpString;
            }
            else if (playerIndex == 1)
            {
                if (p2HealthBar != null)
                {
                    p2HealthBar.fillAmount = healthPercent;
                }
                if (p2HealthText != null) p2HealthText.text = hpString;
            }
        }

        public void SetPlayerName(int playerIndex, string name)
        {
            if (playerIndex == 0)
            {
                if (p1Name != null) p1Name.text = name;
            }
            else if (playerIndex == 1)
            {
                if (p2Name != null) p2Name.text = name;
            }
        }
    }
}
