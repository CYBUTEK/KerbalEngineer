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
        private Text m_Title = null;

        [SerializeField]
        private Transform m_Content = null;

        private Vector2 m_BeginMousePosition;
        private Vector3 m_BeginWindowPosition;
        private CanvasGroup m_CanvasGroup;
        private RectTransform m_RectTransform;
        private IEnumerator m_ScaleFadeCoroutine;

        /// <summary>
        ///     Gets the content transform.
        /// </summary>
        public Transform Content
        {
            get
            {
                return m_Content;
            }
        }

        /// <summary>
        ///     Gets the rect transform component.
        /// </summary>
        public RectTransform RectTransform
        {
            get
            {
                return m_RectTransform;
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_RectTransform == null)
            {
                return;
            }

            // cache starting positions
            m_BeginMousePosition = eventData.position;
            m_BeginWindowPosition = m_RectTransform.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_RectTransform != null)
            {
                // new position is the starting window position plus the delta of the current and starting mouse positions
                m_RectTransform.position = m_BeginWindowPosition + (Vector3)(eventData.position - m_BeginMousePosition);
            }
        }

        /// <summary>
        ///     Adds a game object as a child of the window content.
        /// </summary>
        public void AddToContent(GameObject childObject)
        {
            if (m_Content != null && childObject != null)
            {
                childObject.transform.SetParent(m_Content, false);
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
            if (m_Title != null)
            {
                m_Title.text = title;
            }
        }

        /// <summary>
        ///     Sets the window size.
        /// </summary>
        public void SetWidth(float width)
        {
            if (m_RectTransform != null)
            {
                Vector2 size = m_RectTransform.sizeDelta;
                size.x = width;
                m_RectTransform.sizeDelta = size;
            }
        }

        protected virtual void Awake()
        {
            // component caching
            m_RectTransform = GetComponent<RectTransform>();
            m_CanvasGroup = GetComponent<CanvasGroup>();
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
            if (m_ScaleFadeCoroutine != null)
            {
                StopCoroutine(m_ScaleFadeCoroutine);
            }

            m_ScaleFadeCoroutine = ScaleFadeCoroutine(from, to, callback);
            StartCoroutine(m_ScaleFadeCoroutine);
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
                if (m_CanvasGroup != null)
                {
                    m_CanvasGroup.alpha = Mathf.Clamp01(value);
                }

                yield return null;
            }

            if (callback != null)
            {
                callback.Invoke();
            }

            m_ScaleFadeCoroutine = null;
        }
    }
}