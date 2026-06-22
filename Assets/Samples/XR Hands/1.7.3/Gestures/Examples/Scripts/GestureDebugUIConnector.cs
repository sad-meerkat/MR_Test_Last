using System.Reflection;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Samples.Gestures.DebugTools;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// StaticHandGesture의 gesturePerformed/gestureEnded 이벤트를
    /// Hand Shape Debugger UI 컴포넌트에 런타임으로 연결합니다.
    ///
    /// 씬에서 m_Target 참조가 null로 설정된 경우를 자동 수정하며,
    /// 씬에 있는 XRHandShapeDebugUI를 자동으로 탐지해 오른손/왼손을 연결합니다.
    /// </summary>
    public class GestureDebugUIConnector : MonoBehaviour
    {
        [Header("오른손 Debug UI")]
        [SerializeField]
        [Tooltip("오른손 Gesture Detection의 결과를 표시할 XRHandShapeDebugUI")]
        XRHandShapeDebugUI m_RightHandShapeDebugUI;

        [SerializeField]
        [Tooltip("오른손 선택된 제스처 이름을 표시할 XRSelectedHandShapeDebugUI")]
        XRSelectedHandShapeDebugUI m_RightSelectedHandShapeDebugUI;

        [Header("왼손 Debug UI (선택)")]
        [SerializeField]
        [Tooltip("왼손 Gesture Detection의 결과를 표시할 XRHandShapeDebugUI")]
        XRHandShapeDebugUI m_LeftHandShapeDebugUI;

        [SerializeField]
        [Tooltip("왼손 선택된 제스처 이름을 표시할 XRSelectedHandShapeDebugUI")]
        XRSelectedHandShapeDebugUI m_LeftSelectedHandShapeDebugUI;

        // XRHandShapeDebugUI.m_XRAllFingerShapesDebugUI 필드 접근용
        static readonly FieldInfo s_FingerUIField = typeof(XRHandShapeDebugUI)
            .GetField("m_XRAllFingerShapesDebugUI", BindingFlags.NonPublic | BindingFlags.Instance);

        void Start()
        {
            if (m_RightHandShapeDebugUI == null || m_LeftHandShapeDebugUI == null)
                AutoFindDebugUI();

            ConnectGestures();
        }

        void AutoFindDebugUI()
        {
#if UNITY_EDITOR
            Debug.Log("[GestureDebugUIConnector] XRHandShapeDebugUI 자동 탐색 중...");
#endif
            var allShapeUIs = FindObjectsByType<XRHandShapeDebugUI>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var ui in allShapeUIs)
            {
                Handedness hand = GetHandednessFromDebugUI(ui);
                if (hand == Handedness.Right && m_RightHandShapeDebugUI == null)
                {
                    m_RightHandShapeDebugUI = ui;
                    if (m_RightSelectedHandShapeDebugUI == null)
                        m_RightSelectedHandShapeDebugUI = FindSelectedDebugUI(ui);
                }
                else if (hand == Handedness.Left && m_LeftHandShapeDebugUI == null)
                {
                    m_LeftHandShapeDebugUI = ui;
                    if (m_LeftSelectedHandShapeDebugUI == null)
                        m_LeftSelectedHandShapeDebugUI = FindSelectedDebugUI(ui);
                }
            }

            if (m_RightHandShapeDebugUI == null)
                Debug.LogWarning("[GestureDebugUIConnector] 오른손 XRHandShapeDebugUI를 찾지 못했습니다. Inspector에서 직접 할당하세요.", this);
        }

        // reflection으로 XRHandShapeDebugUI의 m_XRAllFingerShapesDebugUI를 읽어 handedness 반환
        static Handedness GetHandednessFromDebugUI(XRHandShapeDebugUI ui)
        {
            if (s_FingerUIField == null) return Handedness.Invalid;

            var fingerUI = s_FingerUIField.GetValue(ui) as XRAllFingerShapesDebugUI;
            if (fingerUI != null) return fingerUI.handedness;

            // fallback: 오브젝트 이름 기반 추정
            string name = ui.gameObject.name.ToLower();
            if (name.Contains("right")) return Handedness.Right;
            if (name.Contains("left"))  return Handedness.Left;
            return Handedness.Invalid;
        }

        // XRHandShapeDebugUI와 같은 Transform 계층에서 XRSelectedHandShapeDebugUI 탐색
        static XRSelectedHandShapeDebugUI FindSelectedDebugUI(XRHandShapeDebugUI ui)
        {
            // 같은 GameObject
            var found = ui.GetComponent<XRSelectedHandShapeDebugUI>();
            if (found != null) return found;

            // 부모 계층
            found = ui.GetComponentInParent<XRSelectedHandShapeDebugUI>(true);
            if (found != null) return found;

            // 같은 부모의 자식들 탐색
            var parent = ui.transform.parent;
            if (parent != null)
                found = parent.GetComponentInChildren<XRSelectedHandShapeDebugUI>(true);
            return found;
        }

        void ConnectGestures()
        {
            var gestures = FindObjectsByType<StaticHandGesture>(
                FindObjectsInactive.Include, FindObjectsSortMode.None);

            int rightCount = 0, leftCount = 0, skipped = 0;

            foreach (var gesture in gestures)
            {
                if (gesture.handShapeOrPose == null || gesture.handTrackingEvents == null)
                {
                    skipped++;
                    continue;
                }

                bool isRight = gesture.handTrackingEvents.handedness == Handedness.Right;
                var shapeUI    = isRight ? m_RightHandShapeDebugUI    : m_LeftHandShapeDebugUI;
                var selectedUI = isRight ? m_RightSelectedHandShapeDebugUI : m_LeftSelectedHandShapeDebugUI;

                if (shapeUI == null) continue;

                // 로컬 변수 캡처 (람다 클로저)
                var g    = gesture;
                var sui  = shapeUI;
                var ssui = selectedUI;

                g.gesturePerformed.AddListener(() =>
                {
                    sui.handShapeOrPose = g.handShapeOrPose;
                    if (ssui != null) ssui.UpdateSelectedHandShapeTextUI(g.handShapeOrPose);
                });

                g.gestureEnded.AddListener(() =>
                {
                    sui.ClearDetectedHandShape();
                    if (ssui != null) ssui.ResetUI();
                });

                if (isRight) rightCount++;
                else         leftCount++;
            }

            Debug.Log($"[GestureDebugUIConnector] 연결 완료 — 오른손: {rightCount}개, 왼손: {leftCount}개 (건너뜀: {skipped}개)");
        }
    }
}
