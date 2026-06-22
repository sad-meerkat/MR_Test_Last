using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.XR;

/// <summary>
/// 시작 UI → X버튼으로 영상 시작 → 3개 시간대에 영상 멈춤 → 각 Phase 실행 → 완료 시 영상 재개
/// </summary>
public class VideoTutorialController : MonoBehaviour
{
    [Header("Start UI")]
    [SerializeField]
    [Tooltip("시작 화면 UI 오브젝트")]
    GameObject m_StartUI;

    [SerializeField]
    [Tooltip("나가기 시 이동할 씬 이름 (비어있으면 이전 씬)")]
    string m_ExitSceneName;

    [Header("Video")]
    [SerializeField]
    [Tooltip("영상을 재생할 VideoPlayer")]
    VideoPlayer m_VideoPlayer;

    [SerializeField]
    [Tooltip("영상 클립")]
    VideoClip m_VideoClip;

    [SerializeField]
    [Tooltip("영상이 표시되는 오브젝트")]
    GameObject m_VideoScreen;

    [Header("Action")]
    [SerializeField]
    [Tooltip("DropSwapLaunchAction 스크립트")]
    DropSwapLaunchAction m_Action;

    [Header("BGM")]
    [SerializeField]
    [Tooltip("배경음악을 재생할 AudioSource")]
    AudioSource m_BgmSource;

    [SerializeField]
    [Tooltip("배경음악 클립")]
    AudioClip m_BgmClip;

    [Header("Trigger Times (3개)")]
    [SerializeField]
    [Tooltip("Phase 1 시작 시간 (초)")]
    float m_TriggerTime1;

    [SerializeField]
    [Tooltip("Phase 2 시작 시간 (초)")]
    float m_TriggerTime2;

    [SerializeField]
    [Tooltip("Phase 3 시작 시간 (초)")]
    float m_TriggerTime3;

    bool m_Started;
    bool m_PrevX;
    bool m_PrevY;
    int m_CurrentPhase; // 0: 아직 안 멈춤, 1~3: 해당 Phase 진행 중
    bool m_WaitingForPhase;
    float[] m_TriggerTimes;

    void Start()
    {
        m_Started = false;
        m_CurrentPhase = 0;
        m_WaitingForPhase = false;

        m_TriggerTimes = new float[] { m_TriggerTime1, m_TriggerTime2, m_TriggerTime3 };

        if (m_Action != null)
        {
            m_Action.enabled = false;
            m_Action.OnPhaseCompleted += OnPhaseCompleted;
        }

        if (m_VideoScreen != null)
            m_VideoScreen.SetActive(false);

        if (m_StartUI != null)
            m_StartUI.SetActive(true);
    }

    void OnDestroy()
    {
        if (m_Action != null)
            m_Action.OnPhaseCompleted -= OnPhaseCompleted;
    }

    void Update()
    {
        if (!m_Started)
        {
            // 1. 더 광범위한 입력 체크: 
            // Controller.Active는 현재 손이나 컨트롤러 중 활성화된 기기를 자동으로 찾습니다.
            // RawButton.X는 오큘러스 터치 컨트롤러의 X 버튼을 직접 지칭합니다.
            bool xPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch) ||
                            OVRInput.Get(OVRInput.RawButton.X);

            bool yPressed = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch) ||
                            OVRInput.Get(OVRInput.RawButton.Y);

            // 2. 입력 감지 로그 (디버깅용)
            // 만약 작동 안 한다면 콘솔 창에 이 로그가 찍히는지 확인해 보세요.
            if (xPressed) Debug.Log("X Button Detected!");

            if (xPressed && !m_PrevX)
                OnStart();

            if (yPressed && !m_PrevY)
                OnExit();

            m_PrevX = xPressed;
            m_PrevY = yPressed;
            return;
        }
        // Phase 진행 중 및 영상 체크 로직 (이하 동일)
        if (m_WaitingForPhase)
            return;

        if (m_VideoPlayer == null || !m_VideoPlayer.isPlaying)
            return;

        if (m_CurrentPhase < 3)
        {
            if (m_VideoPlayer.time >= m_TriggerTimes[m_CurrentPhase])
            {
                m_CurrentPhase++;
                m_WaitingForPhase = true;

                // UI를 먼저 활성화하여 사용자가 시각적으로 끊김을 느끼지 않게 함
                if (m_Action != null)
                {
                    m_Action.enabled = true;
                    m_Action.StartPhase(m_CurrentPhase);
                }

                // UI 활성 후 영상을 일시정지
                m_VideoPlayer.Pause();

                if (m_CurrentPhase >= 3 && m_VideoScreen != null)
                    m_VideoScreen.SetActive(false);
            }
        }
    }

    void OnPhaseCompleted()
    {
        m_WaitingForPhase = false;

        // Phase 3 완료 시 영상 재개 안 함 (End UI가 뜸)
        if (m_CurrentPhase >= 3)
            return;

        // 영상 화면 다시 표시 및 재개
        if (m_VideoScreen != null)
            m_VideoScreen.SetActive(true);

        if (m_VideoPlayer != null)
            m_VideoPlayer.Play();
    }

    public void OnStart()
    {
        m_Started = true;

        if (m_StartUI != null)
            m_StartUI.SetActive(false);

        if (m_VideoScreen != null)
            m_VideoScreen.SetActive(true);

        if (m_VideoPlayer != null && m_VideoClip != null)
        {
            m_VideoPlayer.clip = m_VideoClip;
            m_VideoPlayer.Play();
        }

        if (m_BgmSource != null && m_BgmClip != null)
        {
            m_BgmSource.clip = m_BgmClip;
            m_BgmSource.loop = true;
            m_BgmSource.Play();
        }
    }

    public void OnExit()
    {
        if (!string.IsNullOrEmpty(m_ExitSceneName))
            SceneManager.LoadScene(m_ExitSceneName);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
