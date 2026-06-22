using UnityEngine;
using UnityEngine.Events;

public class VRButtonTouch : MonoBehaviour
{
    [Tooltip("터치 시 실행할 함수를 연결하세요.")]
    public UnityEvent onTouched;

    // 컨트롤러(또는 손)가 버튼의 콜라이더에 닿았을 때 실행
    private void OnTriggerEnter(Collider other)
    {
        // 디버그 로그 추가: 무엇이든 닿으면 로그를 남깁니다.

        // 컨트롤러 오브젝트인지 확인 (이름에 Anchor나 Controller, Hand가 포함된 경우)
        if (other.name.Contains("Anchor") || other.name.Contains("Controller") || other.name.Contains("Hand"))
        {
            onTouched?.Invoke(); // 연결된 함수 실행
        }
    }
}