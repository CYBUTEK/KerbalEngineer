// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Unity
{
    using System;
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFader : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        private IEnumerator fadeCoroutine;

        public bool IsFading
        {
            get
            {
                return fadeCoroutine != null;
            }
        }

        /// <summary>
        ///     Fades the canvas group to a specified alpha using the supplied blocking state during fade with optional callback.
        /// </summary>
        public void FadeTo(float alpha, float duration, Action callback = null)
        {
            if (canvasGroup == null)
            {
                return;
            }

            Fade(canvasGroup.alpha, alpha, duration, callback);
        }

        /// <summary>
        ///     Sets the alpha value of the canvas group.
        /// </summary>
        public void SetAlpha(float alpha)
        {
            if (canvasGroup == null)
            {
                return;
            }

            alpha = Mathf.Clamp01(alpha);
            canvasGroup.alpha = alpha;
        }

        protected virtual void Awake()
        {
            // cache components
            canvasGroup = GetComponent<CanvasGroup>();
        }

        /// <summary>
        ///     Starts a fade from one alpha value to another with callback.
        /// </summary>
        private void Fade(float from, float to, float duration, Action callback)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }

            fadeCoroutine = FadeCoroutine(from, to, duration, callback);
            StartCoroutine(fadeCoroutine);
        }

        /// <summary>
        ///     Coroutine that handles the fading.
        /// </summary>
        private IEnumerator FadeCoroutine(float from, float to, float duration, Action callback)
        {
            // wait for end of frame so that only the last call to fade that frame is honoured.
            yield return new WaitForEndOfFrame();

            float progress = 0.0f;

            while (progress <= 1.0f)
            {
                progress += Time.deltaTime / duration;
                SetAlpha(Mathf.Lerp(from, to, progress));
                yield return null;
            }

            if (callback != null)
            {
                callback.Invoke();
            }

            fadeCoroutine = null;
        }
    }
}