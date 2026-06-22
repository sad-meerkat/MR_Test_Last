/*
2026-06-14 AI-Tag
This was created with the help of Assistant, a Unity Artificial Intelligence product.
*/
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.XR.Templates.MRTTabletopAssets
{
    /// <summary>
    /// 등록된 360 배경 오브젝트들 중 하나만 랜덤으로 활성화하고 
    /// 나머지는 모두 비활성화합니다.
    /// </summary>
    public class Random360ObjectActivator : MonoBehaviour
    {
        [Header("360 Background Objects")]
        [SerializeField]
        [Tooltip("복사해서 배치한 여러 개의 BackgroundLobby 오브젝트들을 여기에 드래그해서 넣으세요.")]
        private GameObject[] m_BackgroundObjects;

        void Start()
        {
            if (m_BackgroundObjects == null || m_BackgroundObjects.Length == 0)
            {
                Debug.LogWarning($"[{nameof(Random360ObjectActivator)}] 활성화할 배경 오브젝트가 등록되지 않았습니다!");
                return;
            }

            // 1. 활성화할 랜덤 인덱스 하나 선택
            int randomIndex = Random.Range(0, m_BackgroundObjects.Length);

            // 2. 전체 오브젝트를 돌면서 선택된 인덱스만 켜고, 나머지는 전부 끕니다.
            for (int i = 0; i < m_BackgroundObjects.Length; i++)
            {
                if (m_BackgroundObjects[i] != null)
                {
                    // i가 randomIndex와 같을 때만 true가 되어 켜집니다.
                    m_BackgroundObjects[i].SetActive(i == randomIndex);
                }
            }

            Debug.Log($"[{nameof(Random360ObjectActivator)}] {randomIndex}번째 배경 오브젝트가 활성화되었습니다. (이름: {m_BackgroundObjects[randomIndex].name})");
        }
    }
}
