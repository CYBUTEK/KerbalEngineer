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

        /// <summary>
        ///     Fades the canvas group to a specified alpha value with optional callback.
        /// </summary>
        public void FadeTo(float alpha, Action callback = null)
        {
            if (m_CanvasGroup == null || gameObject.activeInHierarchy == false)
            {
                return;
            }

            Fade(m_CanvasGroup.alpha, alpha, callback);
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
            m_CanvasGroup.interactable = !(alpha < 1.0f);
        }

        protected virtual void Awake()
        {
            // cache components
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        ///     Starts a fade from one alpha value to another with callback.
        /// </summary>
        private void Fade(float from, float to, Action callback)
        {
            if (m_FadeCoroutine != null)
            {
                StopCoroutine(m_FadeCoroutine);
            }

            m_FadeCoroutine = FadeCoroutine(from, to, callback);
            StartCoroutine(m_FadeCoroutine);
        }

        /// <summary>
        ///     Coroutine that handles the fading.
        /// </summary>
        private IEnumerator FadeCoroutine(float from, float to, Action callback)
        {
            float progress = 0.0f;

            while (progress <= 1.0f)
            {
                progress += Time.deltaTime;
                SetAlpha(Mathf.Lerp(from, to, progress));
                yield return null;
            }

            callback?.Invoke();
        }
    }
}