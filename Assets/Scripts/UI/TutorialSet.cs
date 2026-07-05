using System;
using System.Collections.Generic;
using UnityEngine;

namespace RCCom.UI
{
    /// <summary>
    /// 튜토리얼 1페이지 — 주제/설명/삽화 한 세트. ScriptableObject가 아니라 그냥 직렬화되는
    /// 데이터 묶음(OperatorLineSet과 같은 패턴) — TutorialSet 하나가 이 페이지를 순서대로 여러 개 가진다.
    /// </summary>
    [Serializable]
    public class TutorialPage
    {
        public string topic;

        [TextArea(2, 5)]
        public string description;

        public Sprite image;
    }

    /// <summary>
    /// 튜토리얼 전체 페이지를 모아두는 프로젝트 창 에셋. OperatorDialogueSet과 같은 이유로
    /// SO화 — 페이지마다 삽화(Unity 에셋 참조)가 필요해서 JSON 등 외부 파일로 분리하면
    /// 결국 절반은 여기로 다시 와야 함.
    /// </summary>
    [CreateAssetMenu(menuName = "RCCom/UI/Tutorial Set")]
    public class TutorialSet : ScriptableObject
    {
        public List<TutorialPage> pages = new();
    }
}
