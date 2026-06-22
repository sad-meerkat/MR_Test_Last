using UnityEngine;
using UnityEngine.XR.Templates.MRTTabletopAssets;

/// <summary>
/// 투사체가 캐릭터(FighterHealth)에 닿았을 때 데미지를 입히는 스크립트입니다.
/// </summary>
public class ProjectileDamage : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("적용할 데미지 양")]
    public float damageAmount = 10f;

    [Tooltip("피격 시 생성할 이펙트 프리팹")]
    public GameObject hitEffectPrefab;

    // [핵심 패치] 이 투사체를 발사한 주인 캐릭터 오브젝트 (자신과 자신의 무기는 타격 대상에서 제외)
    [HideInInspector]
    public GameObject owner;

    private void OnTriggerEnter(Collider other)
    {
        // 데미지 계산은 서버에서만 수행합니다.
        if (Unity.Netcode.NetworkManager.Singleton != null && !Unity.Netcode.NetworkManager.Singleton.IsServer) return;

        // [핵심 패치] 충돌한 대상이 시전자 본인이거나 시전자의 자식(손, 무기 등)이라면 완전히 무시합니다.
        if (owner != null)
        {
            if (other.gameObject == owner || other.transform.IsChildOf(owner.transform) || other.transform.root == owner.transform.root)
            {
                return; // 나 자신은 때리지 않고 패스!
            }
        }

        // 부모 오브젝트를 포함하여 FighterHealth 컴포넌트를 찾습니다.
        var health = other.GetComponentInParent<FighterHealth>();

        if (health != null)
        {
            // 데미지 적용
            health.TakeDamage(damageAmount);

            // 피격 이펙트 생성
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // 투사체 제거 (적을 맞춘 경우에만 소멸)
            Destroy(gameObject);
        }
    }
}