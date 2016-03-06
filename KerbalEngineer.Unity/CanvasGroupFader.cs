namespace KerbalEngineer.Unity
{
    using System;
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;
        private IEnumerator m_FadeCoroutine;

        public bool fading
        {
            get
            {
                return m_FadeCoroutine != null;
            }
        }

        /// <summary>
        ///     Fades the canvas group to a specified alpha using the supplied blocking state during fade with optional callback.
        /// </summary>
        public void FadeTo(float alpha, float duration, Action callback = null)
        {
            if (m_CanvasGroup == null)
            {
                return;
            }

            Fade(m_CanvasGroup.alpha, alpha, duration, callback);
        }

        /// <summary>
        ///     Sets the alpha value of the canvas group.
        /// </summary>
        public void SetAlpha(float alpha)
        {
            if (m_CanvasGroup == null)
            {
                return;
            }

            alpha = Mathf.Clamp01(alpha);
            m_CanvasGroup.alpha = alpha;
        }

        protected virtual void Awake()
        {
            // cache components
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        ///     Starts a fade from one alpha value to another with callback.
        /// </summary>
        private void Fade(float from, float to, float duration, Action callback)
        {
            if (m_FadeCoroutine != null)
            {
                StopCoroutine(m_FadeCoroutine);
            }

            m_FadeCoroutine = FadeCoroutine(from, to, duration, callback);
            StartCoroutine(m_FadeCoroutine);
        }

        /// <summary>
        ///     Coroutine that handles the fading.
        /// </summary>
        private IEnumerator FadeCoroutine(float from, float to, float duration, Action callback)
        {
            float progress = 0.0f;

            while (progress <= 1.0f)
            {
                progress += Time.deltaTime / duration;
                SetAlpha(Mathf.Lerp(from, to, progress));
                yield return null;
            }

            print(m_CanvasGroup.alpha);
            callback?.Invoke();

            m_FadeCoroutine = null;
        }
    }
}