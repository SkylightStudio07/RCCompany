using UnityEngine;

namespace RCCom.Runtime
{
    /// <summary>
    /// 아트 임포트 해상도/PPU가 에셋마다 제각각이어도 항상 지정한 월드 크기로 보이도록 균일
    /// 스케일을 계산하는 공용 유틸리티. TowerInstance(실제 건설)와 TowerBuildPreview(프리뷰)
    /// 둘 다 같은 기준으로 맞춰야 프리뷰와 실제 건설 결과 크기가 어긋나지 않는다.
    /// </summary>
    public static class SpriteFit
    {
        /// <summary>스프라이트의 가장 긴 축이 targetSize가 되도록 하는 균일 스케일 값.</summary>
        public static float CalculateUniformScale(Sprite sprite, float targetSize)
        {
            if (sprite == null)
            {
                return 1f;
            }

            Vector2 nativeSize = sprite.bounds.size;
            float largestAxis = Mathf.Max(nativeSize.x, nativeSize.y);

            return largestAxis > 0f ? targetSize / largestAxis : 1f;
        }
    }
}
