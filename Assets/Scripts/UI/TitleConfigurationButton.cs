using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RCCom.UI
{
    public class TitleConfigurationButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISubmitHandler
    {
        public enum Action
        {
            PreviousScreenMode,
            NextScreenMode,
            PreviousResolution,
            NextResolution,
            PreviousVSync,
            NextVSync,
            Apply,
            Back
        }

        [SerializeField] private TitleConfigurationController configurationController;
        [SerializeField] private Action action;
        [SerializeField] private float hoverScale = 1.08f;
        [SerializeField] private float scaleSpeed = 14f;

        private RectTransform rectTransform;
        private Vector3 baseScale;
        private float hoverAmount;
        private bool isHovering;

        private void Awake()
        {
            rectTransform = transform as RectTransform;
            baseScale = rectTransform != null ? rectTransform.localScale : transform.localScale;

            if (TryGetComponent(out Graphic graphic))
            {
                graphic.raycastTarget = true;
            }
        }

        private void OnDisable()
        {
            isHovering = false;
            hoverAmount = 0f;
            ApplyScale();
        }

        private void Update()
        {
            float target = isHovering ? 1f : 0f;
            hoverAmount = Mathf.MoveTowards(hoverAmount, target, scaleSpeed * Time.unscaledDeltaTime);
            ApplyScale();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            Invoke();
        }

        public void Invoke()
        {
            if (configurationController == null)
            {
                return;
            }

            switch (action)
            {
                case Action.PreviousScreenMode:
                    configurationController.PreviousScreenMode();
                    break;
                case Action.NextScreenMode:
                    configurationController.NextScreenMode();
                    break;
                case Action.PreviousResolution:
                    configurationController.PreviousResolution();
                    break;
                case Action.NextResolution:
                    configurationController.NextResolution();
                    break;
                case Action.PreviousVSync:
                    configurationController.PreviousVSync();
                    break;
                case Action.NextVSync:
                    configurationController.NextVSync();
                    break;
                case Action.Apply:
                    configurationController.Apply();
                    break;
                case Action.Back:
                    configurationController.Close();
                    break;
            }
        }

        private void ApplyScale()
        {
            float scale = Mathf.Lerp(1f, hoverScale, hoverAmount);

            if (rectTransform != null)
            {
                rectTransform.localScale = baseScale * scale;
            }
            else
            {
                transform.localScale = baseScale * scale;
            }
        }
    }
}
