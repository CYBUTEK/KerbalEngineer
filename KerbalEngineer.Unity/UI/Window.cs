namespace KerbalEngineer.Unity.UI
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class Window : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        [SerializeField]
        private Text title = null;

        [SerializeField]
        private Transform content = null;

        private Vector2 beginMousePosition;
        private Vector3 beginWindowPosition;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private IEnumerator scaleFadeCoroutine;

        /// <summary>
        ///     Gets the content transform.
        /// </summary>
        public Transform Content
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        ///     Gets the rect transform component.
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                return rectTransform;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (rectTransform == null)
            {
                return;
            }

            // cache starting positions
            beginMousePosition = eventData.position;
            beginWindowPosition = rectTransform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (rectTransform != null)
            {
                // new position is the starting window position plus the delta of the current and starting mouse positions
                rectTransform.position = beginWindowPosition + (Vector3)(eventData.position - beginMousePosition);
            }
        }

        /// <summary>
        ///     Adds a game object as a child of the window content.
        /// </summary>
        public void AddToContent(GameObject childObject)
        {
            if (content != null && childObject != null)
            {
                childObject.transform.SetParent(content, false);
            }
        }

        /// <summary>
        ///     Closes the window.
        /// </summary>
        public void Close()
        {
            ScaleFade(1.0f, 0.0f, () => Destroy(gameObject));
        }

        /// <summary>
        ///     Sets the window title.
        /// </summary>
        public void SetTitle(string title)
        {
            if (this.title != null)
            {
                this.title.text = title;
            }
        }

        /// <summary>
        ///     Sets the window size.
        /// </summary>
        public void SetWidth(float width)
        {
            if (rectTransform != null)
            {
                Vector2 size = rectTransform.sizeDelta;
                size.x = width;
                rectTransform.sizeDelta = size;
            }
        }

        protected virtual void Awake()
        {
            // component caching
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected virtual void OnEnable()
        {
            // scales and fades the window into view
            ScaleFade(0.0f, 1.0f, null);
        }

        /// <summary>
        ///     Scales and fades from a value to another with callback.
        /// </summary>
        private void ScaleFade(float from, float to, Action callback)
        {
            if (scaleFadeCoroutine != null)
            {
                StopCoroutine(scaleFadeCoroutine);
            }

            scaleFadeCoroutine = ScaleFadeCoroutine(from, to, callback);
            StartCoroutine(scaleFadeCoroutine);
        }

        /// <summary>
        ///     Coroutine to handle the scale and fading of the window.
        /// </summary>
        private IEnumerator ScaleFadeCoroutine(float from, float to, Action callback)
        {
            float progress = 0.0f;
            float value;

            while (progress <= 1.0f)
            {
                progress += (Time.deltaTime / 0.2f);
                value = Mathf.Lerp(from, to, progress);

                // scale
                transform.localScale = Vector3.one * value;

                // fade if a canvas group is attached
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = Mathf.Clamp01(value);
                }

                yield return null;
            }

            if (callback != null)
            {
                callback.Invoke();
            }

            scaleFadeCoroutine = null;
        }
    }
}