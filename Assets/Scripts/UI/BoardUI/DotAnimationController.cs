using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace UI.BoardUI
{
    public class DotAnimationController : MonoBehaviour
    {
        [Header("Settings")] public float growthTime = 0.1f;
        public float shrinkTime = 0.1f;
        public float growthDelayStaggerTime = 0.4f;
        public float shrinkDelayStaggerTime = 0.4f;

        private Vector3 m_StartScale;

        // Start is called before the first frame update
        private void Start()
        {
            var dotTransform = transform;
            m_StartScale = dotTransform.localScale;
            dotTransform.localScale = Vector3.zero;
        }

        public void DoDelayedGrowth(int staggerMult)
        {
            StartCoroutine(DelayedGrowth((staggerMult - 1f) * growthDelayStaggerTime));
        }

        private IEnumerator DelayedGrowth(float delay)
        {
            yield return new WaitForSeconds(delay);
            DoGrowth();
        }

        private void DoGrowth()
        {
            transform.DOScale(m_StartScale, growthTime);
        }

        public void DoDelayedShrink(int staggerMult)
        {
            StartCoroutine(DelayedShrink((staggerMult - 1f) * shrinkDelayStaggerTime));
        }

        private IEnumerator DelayedShrink(float delay)
        {
            yield return new WaitForSeconds(delay);
            DoShrink();
        }

        private void DoShrink()
        {
            transform.DOScale(Vector3.zero, shrinkTime);
        }
    }
}