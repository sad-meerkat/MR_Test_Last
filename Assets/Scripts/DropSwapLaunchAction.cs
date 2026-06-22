using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Hands.Samples.GestureSample;

/// <summary>
/// 튜토리얼 통합 액션:
/// 1단계: 제스처 → 오브젝트가 아래로 떨어지고, 사라진 뒤 프리팹들이 순서대로 생성
/// 2단계: 제스처 → 생성된 프리팹들이 새 프리팹으로 교체
/// 3단계: 제스처 → 교체된 프리팹들이 원형 궤도를 그리며 전방으로 날아감
/// VideoTutorialController가 각 Phase를 시작하고, 완료되면 영상을 재개합니다.
/// </summary>
public class DropSwapLaunchAction : MonoBehaviour
{
    [Header("Phase 1 - Drop Gesture")]
    [SerializeField]
    [Tooltip("1단계: 오브젝트 떨어뜨리기 제스처")]
    StaticHandGesture m_DropGesture;

    [Header("Drop Object")]
    [SerializeField]
    [Tooltip("아래로 떨어질 오브젝트 (씬에 미리 배치)")]
    GameObject m_DropObject;

    [SerializeField]
    [Tooltip("떨어지는 속도")]
    float m_DropSpeed = 3f;

    [SerializeField]
    [Tooltip("떨어진 후 사라지기까지 시간 (초)")]
    float m_DropDuration = 3f;

    [Header("Spawn After Drop")]
    [SerializeField]
    [Tooltip("순서대로 생성할 프리팹들")]
    GameObject[] m_SpawnPrefabs;

    [SerializeField]
    [Tooltip("각 프리팹의 생성 위치")]
    Transform[] m_SpawnPoints;

    [SerializeField]
    [Tooltip("각 프리팹 생성 간격 (초)")]
    float m_SpawnInterval = 0.5f;

    [SerializeField]
    [Tooltip("프리팹이 아래에서 올라오는 거리")]
    float m_RiseOffset = 2f;

    [SerializeField]
    [Tooltip("프리팹이 올라오는 시간 (초)")]
    float m_RiseDuration = 0.5f;

    [Header("Phase 2 - Swap Gesture")]
    [SerializeField]
    [Tooltip("2단계: 프리팹 교체 제스처")]
    StaticHandGesture m_SwapGesture;

    [SerializeField]
    [Tooltip("교체할 새 프리팹")]
    GameObject m_ReplacePrefab;

    [Header("Phase 3 - Launch Gesture")]
    [SerializeField]
    [Tooltip("3단계: 발사 제스처")]
    StaticHandGesture m_LaunchGesture;

    [SerializeField]
    [Tooltip("전방 이동 속도")]
    float m_ForwardSpeed = 10f;

    [SerializeField]
    [Tooltip("원형 궤도 반경")]
    float m_OrbitRadius = 2f;

    [SerializeField]
    [Tooltip("원형 궤도 회전 속도 (도/초)")]
    float m_OrbitSpeed = 360f;

    [SerializeField]
    [Tooltip("발사 후 자동 소멸 시간 (초)")]
    float m_Lifetime = 5f;

    [Header("Tutorial UI (각 Phase별 UI 오브젝트)")]
    [SerializeField]
    [Tooltip("Phase 1 튜토리얼 UI 오브젝트")]
    GameObject m_Phase1UI;

    [SerializeField]
    [Tooltip("Phase 2 튜토리얼 UI 오브젝트")]
    GameObject m_Phase2UI;

    [SerializeField]
    [Tooltip("Phase 3 튜토리얼 UI 오브젝트")]
    GameObject m_Phase3UI;

    [Header("End UI")]
    [SerializeField]
    [Tooltip("Phase 3 완료 후 표시할 End UI")]
    GameObject m_EndUI;

    [SerializeField]
    [Tooltip("End UI가 나타나기까지 대기 시간 (초)")]
    float m_EndUIDelay = 3f;

    [SerializeField]
    [Tooltip("나가기 시 이동할 씬 이름 (비어있으면 이전 씬)")]
    string m_ExitSceneName;

    [Header("Audio")]
    [SerializeField]
    [Tooltip("효과음을 재생할 AudioSource")]
    AudioSource m_SfxSource;

    [SerializeField]
    [Tooltip("배경음악을 재생할 AudioSource")]
    AudioSource m_BgmSource;

    [SerializeField]
    [Tooltip("배경음악 클립")]
    AudioClip m_BgmClip;

    [SerializeField]
    [Tooltip("Phase 1 제스처 효과음")]
    AudioClip m_Phase1Sfx;

    [SerializeField]
    [Tooltip("Phase 2 제스처 효과음")]
    AudioClip m_Phase2Sfx;

    [SerializeField]
    [Tooltip("Phase 3 제스처 효과음")]
    AudioClip m_Phase3Sfx;

    enum Phase { Idle, WaitDrop, Dropping, WaitSwap, WaitLaunch, Launching, Done, End }

    Phase m_Phase;
    bool m_Completed;
    bool m_PhaseActionCompleted;
    GameObject[] m_SpawnedObjects;
    bool m_PrevX;
    bool m_PrevY;
    GameObject m_CurrentUI;

    /// <summary>현재 Phase의 액션이 완료되었는지</summary>
    public bool IsPhaseActionCompleted => m_PhaseActionCompleted;

    public bool IsCompleted => m_Completed;

    /// <summary>Phase 완료 시 호출되는 콜백</summary>
    public event Action OnPhaseCompleted;

    public void ResetAction()
    {
        m_Completed = false;
        m_Phase = Phase.Idle;
        m_PhaseActionCompleted = false;
        m_SpawnedObjects = null;

        if (m_DropObject != null)
            m_DropObject.SetActive(true);
    }

    /// <summary>VideoTutorialController가 호출: 해당 Phase를 시작</summary>
    public void StartPhase(int phase)
    {
        m_PhaseActionCompleted = false;

        switch (phase)
        {
            case 1:
                m_Phase = Phase.WaitDrop;
                SubscribeGesture(m_DropGesture, OnDropGesture);
                ShowPhaseUI(m_Phase1UI);
                break;
            case 2:
                m_Phase = Phase.WaitSwap;
                SubscribeGesture(m_SwapGesture, OnSwapGesture);
                ShowPhaseUI(m_Phase2UI);
                break;
            case 3:
                m_Phase = Phase.WaitLaunch;
                SubscribeGesture(m_LaunchGesture, OnLaunchGesture);
                ShowPhaseUI(m_Phase3UI);
                break;
        }
    }

    void OnDisable()
    {
        UnsubscribeAll();
        HideUI();
    }

    void SubscribeGesture(StaticHandGesture gesture, UnityEngine.Events.UnityAction callback)
    {
        if (gesture != null)
            gesture.gesturePerformed.AddListener(callback);
    }

    void UnsubscribeAll()
    {
        if (m_DropGesture != null)
            m_DropGesture.gesturePerformed.RemoveListener(OnDropGesture);
        if (m_SwapGesture != null)
            m_SwapGesture.gesturePerformed.RemoveListener(OnSwapGesture);
        if (m_LaunchGesture != null)
            m_LaunchGesture.gesturePerformed.RemoveListener(OnLaunchGesture);
    }

    void ShowPhaseUI(GameObject ui)
    {
        HideUI();
        m_CurrentUI = ui;
        if (m_CurrentUI != null)
            m_CurrentUI.SetActive(true);
    }

    void HideUI()
    {
        if (m_CurrentUI != null)
            m_CurrentUI.SetActive(false);
        m_CurrentUI = null;
    }

    void PlaySfx(AudioClip clip)
    {
        if (m_SfxSource != null && clip != null)
            m_SfxSource.PlayOneShot(clip);
    }

    void PhaseCompleted()
    {
        m_PhaseActionCompleted = true;
        UnsubscribeAll();
        OnPhaseCompleted?.Invoke();
    }

    // === Phase 1: Drop ===
    void OnDropGesture()
    {
        if (m_Phase != Phase.WaitDrop)
            return;

        m_Phase = Phase.Dropping;
        HideUI();
        PlaySfx(m_Phase1Sfx);
        StartCoroutine(DropThenSpawn());
    }

    IEnumerator DropThenSpawn()
    {
        if (m_DropObject != null)
        {
            float elapsed = 0f;
            while (elapsed < m_DropDuration)
            {
                if (m_DropObject == null) break;
                m_DropObject.transform.position += Vector3.down * m_DropSpeed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (m_DropObject != null)
                m_DropObject.SetActive(false);
        }

        int count = Mathf.Min(
            m_SpawnPrefabs != null ? m_SpawnPrefabs.Length : 0,
            m_SpawnPoints != null ? m_SpawnPoints.Length : 0
        );

        for (int i = 0; i < count; i += 2)
        {
            for (int j = i; j < Mathf.Min(i + 2, count); j++)
            {
                if (m_SpawnPrefabs[j] == null || m_SpawnPoints[j] == null)
                    continue;

                Vector3 targetPos = m_SpawnPoints[j].position;
                Vector3 startPos = targetPos + Vector3.down * m_RiseOffset;

                GameObject obj = Instantiate(m_SpawnPrefabs[j], startPos, m_SpawnPoints[j].rotation, m_SpawnPoints[j]);
                StartCoroutine(RiseUp(obj.transform, startPos, targetPos));
            }

            if (i + 2 < count)
                yield return new WaitForSeconds(m_SpawnInterval);
        }

        // RiseUp이 끝날 시간 대기
        yield return new WaitForSeconds(m_RiseDuration);

        PhaseCompleted();
    }

    IEnumerator RiseUp(Transform obj, Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        while (elapsed < m_RiseDuration)
        {
            if (obj == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / m_RiseDuration);
            obj.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        if (obj != null)
            obj.position = to;
    }

    // === Phase 2: Swap ===
    void OnSwapGesture()
    {
        if (m_Phase != Phase.WaitSwap)
            return;

        HideUI();
        PlaySfx(m_Phase2Sfx);

        m_SpawnedObjects = new GameObject[m_SpawnPoints.Length];
        for (int i = 0; i < m_SpawnPoints.Length; i++)
        {
            if (m_SpawnPoints[i] == null) continue;

            foreach (Transform child in m_SpawnPoints[i])
                Destroy(child.gameObject);

            if (m_ReplacePrefab != null)
            {
                m_SpawnedObjects[i] = Instantiate(m_ReplacePrefab, m_SpawnPoints[i].position, m_SpawnPoints[i].rotation);
            }
        }

        PhaseCompleted();
    }

    // === Phase 3: Launch ===
    void OnLaunchGesture()
    {
        if (m_Phase != Phase.WaitLaunch)
            return;

        m_Phase = Phase.Launching;
        HideUI();
        PlaySfx(m_Phase3Sfx);
        StartCoroutine(LaunchSpiral());
    }

    IEnumerator LaunchSpiral()
    {
        Transform cam = Camera.main != null ? Camera.main.transform : null;
        if (cam == null)
        {
            m_Completed = true;
            m_Phase = Phase.Done;
            PhaseCompleted();
            yield break;
        }

        Vector3 forwardDir = cam.forward;
        Vector3 center = Vector3.zero;

        int count = 0;
        for (int i = 0; i < m_SpawnedObjects.Length; i++)
        {
            if (m_SpawnedObjects[i] != null)
            {
                center += m_SpawnedObjects[i].transform.position;
                count++;
            }
        }
        if (count > 0) center /= count;

        float[] angles = new float[m_SpawnedObjects.Length];
        for (int i = 0; i < m_SpawnedObjects.Length; i++)
        {
            angles[i] = (360f / m_SpawnedObjects.Length) * i;
        }

        float elapsed = 0f;
        while (elapsed < m_Lifetime)
        {
            bool anyAlive = false;
            center += forwardDir * m_ForwardSpeed * Time.deltaTime;

            for (int i = 0; i < m_SpawnedObjects.Length; i++)
            {
                if (m_SpawnedObjects[i] == null) continue;
                anyAlive = true;

                angles[i] += m_OrbitSpeed * Time.deltaTime;
                float rad = angles[i] * Mathf.Deg2Rad;

                Vector3 orbitOffset = cam.right * Mathf.Cos(rad) * m_OrbitRadius
                                    + cam.up * Mathf.Sin(rad) * m_OrbitRadius;

                m_SpawnedObjects[i].transform.position = center + orbitOffset;
                m_SpawnedObjects[i].transform.rotation = Quaternion.LookRotation(forwardDir) * Quaternion.Euler(0f, 0f, angles[i]);
            }

            if (!anyAlive) break;

            elapsed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < m_SpawnedObjects.Length; i++)
        {
            if (m_SpawnedObjects[i] != null)
                Destroy(m_SpawnedObjects[i]);
        }

        m_Completed = true;
        m_Phase = Phase.Done;

        PhaseCompleted();
        StartCoroutine(ShowEndUI());
    }

    IEnumerator ShowEndUI()
    {
        yield return new WaitForSeconds(m_EndUIDelay);

        if (m_EndUI != null)
            m_EndUI.SetActive(true);

        m_Phase = Phase.End;
    }

    void Update()
    {
        // 튜토리얼이 끝난 상태(Phase.End)에서만 입력을 확인합니다.
        if (m_Phase != Phase.End)
            return;

        // OVRInput을 사용하여 왼쪽 컨트롤러의 X(One) 버튼과 Y(Two) 버튼을 감지합니다.
        bool xPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch);
        bool yPressed = OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch);

        // X 버튼: 현재 씬 재시작 (Restart)
        if (xPressed && !m_PrevX)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Y 버튼: 나가기 (Exit)
        if (yPressed && !m_PrevY)
        {
            if (!string.IsNullOrEmpty(m_ExitSceneName))
                SceneManager.LoadScene(m_ExitSceneName);
            else
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }

        // 상태 저장 (다음 프레임에서 연속 입력을 방지하기 위함)
        m_PrevX = xPressed;
        m_PrevY = yPressed;
    }
}
