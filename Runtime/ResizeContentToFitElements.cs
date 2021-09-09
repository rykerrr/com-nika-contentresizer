using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ContentResizer.Extensions;

namespace ContentResizer
{
    public class ResizeContentToFitElements : MonoBehaviour
    {
        [SerializeField] private RectTransform contentToResize = default;

        private List<RectTransform> elements;

        private RectTransform[] contentCorners = new RectTransform[4];

        // Call InitElements and pass the elements that will effectively function as "children" of the content
        // Afterwards call TryResizeStretchedContent
        
        // Returns true if succeeded, false if failed (InitElements was not called prior)
        public bool TryResizeStretchedContent()
        {
            if (elements == null || elements.Count == 0) return false;
            
            InitContentCorners();
            ResizeStretchedContent();

            return true;
        }
        
        public void InitElements(IEnumerable<RectTransform> elems)
        {
            elements = new List<RectTransform>(elems);
        }

        private void InitContentCorners()
        {
            // max x
            contentCorners[0] = (RectTransform) elements
                .OrderByDescending(x => ((RectTransform) x.transform).position.x).First().transform;
            // max y
            contentCorners[1] = (RectTransform) elements
                .OrderByDescending(x => ((RectTransform) x.transform).position.y).First().transform;
            // min x
            contentCorners[2] = (RectTransform) elements
                .OrderBy(x => ((RectTransform) x.transform).position.x).First().transform;
            // min y
            contentCorners[3] = (RectTransform) elements
                .OrderBy(x => ((RectTransform) x.transform).position.y).First().transform;
        }

        // This method is required to resize the stretched content based on the position of the corner nodes,
        // so that they're all encompassed in the scroll view's content
        private void ResizeStretchedContent()
        {
            var rightAnchor = new Vector2(1f, 0.5f);
            var topAnchor = new Vector2(0.5f, 1f);
            var leftAnchor = new Vector2(0f, 0.5f);
            var botAnchor = new Vector2(0.5f, 0f);

            SetContentStretchedValue(contentCorners[1], topAnchor, botAnchor, false, true);
            SetContentStretchedValue(contentCorners[3], botAnchor, topAnchor, true, true);
            SetContentStretchedValue(contentCorners[0], rightAnchor, leftAnchor, false, false);
            SetContentStretchedValue(contentCorners[2], leftAnchor, rightAnchor, true, false);
        }

        // Used for setting Top/Bot/Right/Left of a stretched UI obj respectively based on call
        private void SetContentStretchedValue(RectTransform corner, Vector2 anchor, Vector2 invAnchor, bool isMin,
            bool isY)
        {
            corner.SetAnchorsAndPivotSingleValue(anchor);

            var cornAnchorPos = corner.anchoredPosition;
            var xVal = isY ? 0f : cornAnchorPos.x;
            var yVal = isY ? cornAnchorPos.y : 0f;

            SetAnchorAndPivotOfTalentElements(invAnchor);

            if (isMin)
            {
                var offMin = contentToResize.offsetMin;

                contentToResize.offsetMin =
                    new Vector2(offMin.x + xVal, offMin.y + yVal);
            }
            else
            {
                var offMax = contentToResize.offsetMax;

                contentToResize.offsetMax =
                    new Vector2(offMax.x + xVal, offMax.y + yVal);
            }
        }

        private void SetAnchorAndPivotOfTalentElements(Vector2 value)
        {
            foreach (var link in elements)
            {
                ((RectTransform) link.transform).SetAnchorsAndPivotSingleValue(value);
            }
        }
    }
}