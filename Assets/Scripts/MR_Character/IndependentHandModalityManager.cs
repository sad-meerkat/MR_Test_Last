using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using System.Collections.Generic;

public class IndependentHandModalityManager : MonoBehaviour
{
    [System.Serializable]
    public class HandSideTracker
    {
        public bool isLeft;
        public GameObject handObject;
        public GameObject controllerObject;
        public MonoBehaviour handInteractor;

        [Header("Input System Actions (XRI 바인딩 연동)")]
        public InputActionProperty trackingStateAction; // 컨트롤러 트래킹 상태 감지
        public InputActionProperty positionAction;      // 컨트롤러 위치 변화 직접 추적

        [HideInInspector] public Vector3 lastPosition;
        [HideInInspector] public float lastActiveTime;
        [HideInInspector] public bool currentIsController;
    }

    [Header("Left Side")]
    [SerializeField] HandSideTracker m_LeftTrack = new HandSideTracker { isLeft = true };
    [Header("Right Side")]
    [SerializeField] HandSideTracker m_RightTrack = new HandSideTracker { isLeft = false };

    [Header("UI Feedback")]
    [SerializeField] HandGestureUI m_GestureUI;

    [Header("Sensitivity Settings")]
    [SerializeField, Tooltip("0.01~0.03 추천 (미세 움직임 포착)")]
    float m_PositionDeltaThreshold = 0.015f;
    [SerializeField] float m_SwitchDelay = 0.4f;

    private XRHandSubsystem m_HandSubsystem;

    void OnEnable()
    {
        EnableActions(m_LeftTrack);
        EnableActions(m_RightTrack);
    }

    void OnDisable()
    {
        DisableActions(m_LeftTrack);
        DisableActions(m_RightTrack);
    }

    void EnableActions(HandSideTracker tracker)
    {
        tracker.trackingStateAction.action?.Enable();
        tracker.positionAction.action?.Enable();
    }

    void DisableActions(HandSideTracker tracker)
    {
        tracker.trackingStateAction.action?.Disable();
        tracker.positionAction.action?.Disable();
    }

    void Update()
    {
        if (m_HandSubsystem == null || !m_HandSubsystem.running)
        {
            var subsystems = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(subsystems);
            if (subsystems.Count > 0) m_HandSubsystem = subsystems[0];
        }

        EvaluateSideModality(m_LeftTrack);
        EvaluateSideModality(m_RightTrack);
    }

    void EvaluateSideModality(HandSideTracker tracker)
    {
        bool hasControllerActivity = false;

        // 1. Input System 기반의 실시간 물리 위치 변화 값 직접 연산
        if (tracker.positionAction.action != null)
        {
            Vector3 currentPos = tracker.positionAction.action.ReadValue<Vector3>();
            float distanceMoved = Vector3.Distance(currentPos, tracker.lastPosition);

            // 기기가 값을 보내고 있고, 노이즈 한계치보다 크게 움직였다면 활동 상태로 추정
            if (currentPos != Vector3.zero && distanceMoved > m_PositionDeltaThreshold)
            {
                hasControllerActivity = true;
            }
            tracker.lastPosition = currentPos;
        }

        // 2. 컨트롤러의 트래킹 플래그가 하드웨어 단에서 완전 유효한지 2차 검증
        if (tracker.trackingStateAction.action != null)
        {
            int trackingState = tracker.trackingStateAction.action.ReadValue<int>();
            // 3 이면 InputTrackingState.Position | InputTrackingState.Rotation (정상 추적 상태)
            if (trackingState >= 3)
            {
                if (tracker.positionAction.action != null && tracker.positionAction.action.ReadValue<Vector3>() != Vector3.zero)
                    hasControllerActivity = true;
            }
        }

        // 3. 서브시스템의 핸드 트래킹 상태 확인
        bool isHandTracked = false;
        if (m_HandSubsystem != null)
        {
            var xrHand = tracker.isLeft ? m_HandSubsystem.leftHand : m_HandSubsystem.rightHand;
            isHandTracked = xrHand.isTracked;
        }

        if (hasControllerActivity)
        {
            tracker.lastActiveTime = Time.time;
        }

        // 최종 모드 결정 조건문
        bool targetIsController = (Time.time - tracker.lastActiveTime < m_SwitchDelay) || !isHandTracked;

        if (targetIsController != tracker.currentIsController)
        {
            tracker.currentIsController = targetIsController;
            ApplyModality(tracker, isHandTracked);
        }
    }

    //  Meta SHC 및 브릿지 아키텍처에 맞춰 전면 수정된 모달리티 적용 함수
    void ApplyModality(HandSideTracker tracker, bool handTracked)
    {
        if (tracker.currentIsController)
        {
            // 1. 컨트롤러 오브젝트 켬
            tracker.controllerObject.SetActive(true);

            //  [수정 핵심]: 컨트롤러 활성화 시 손을 꺼버리던 코드(SetActive(false))를 제거하고 무조건 'true'로 킵합니다.
            // 이렇게 해야 Meta 런타임 하드웨어 세션이 양손 동시 트래킹 모드를 끊지 않고 유지합니다.
            tracker.handObject.SetActive(true);

            // 2. 대신 컨트롤러 모드일 때는 해당 손의 XRI 인터렉터(Ray, Grab 기능 등)만 비활성화하여 오작동을 막습니다.
            if (tracker.handInteractor != null) tracker.handInteractor.enabled = false;

            // 3. UI 캔버스에 "현재 왼손/오른손은 컨트롤러 모드임"을 리턴
            UpdateUI(tracker.isLeft, "Controller");
        }
        else
        {
            // 1. 기본 핸드 트래킹 모드 상태 세팅
            tracker.controllerObject.SetActive(false);
            tracker.handObject.SetActive(true);

            // 2. 손이 카메라 시야 내부에서 실제로 물리 트래킹될 때만 기능 인터렉터 활성화
            if (tracker.handInteractor != null) tracker.handInteractor.enabled = handTracked;

            // 3. UI 캔버스 피드백 업데이트
            UpdateUI(tracker.isLeft, handTracked ? "Hand" : "None");
        }
    }

    void UpdateUI(bool isLeft, string mode)
    {
        if (m_GestureUI != null)
        {
            if (isLeft) m_GestureUI.UpdateLeftMode(mode);
            else m_GestureUI.UpdateRightMode(mode);
        }
    }
}