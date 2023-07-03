using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Console.Utils
{
    public static class UIUtils
    {
        public static void StretchToCorners (this RectTransform rectTransform)
        {
            rectTransform.SetAnchorsAndMargins
                (
                new Vector4(0, 0, 1, 1),
                Vector4.zero
                );
        }
        public static void SetAnchorsAndMargins (this RectTransform rectTransform, Vector4 anchors, Vector4 margins)
        {
            rectTransform.anchorMin = new Vector2(anchors.x, anchors.y);
            rectTransform.anchorMax = new Vector2(anchors.z, anchors.w);
            rectTransform.offsetMin = new Vector2(margins.x, margins.y);
            rectTransform.offsetMax = new Vector2(-margins.z, -margins.w);
        }
        public static Canvas GetOrCreateCanvasInContext (GameObject context)
        {
            Canvas canvas;
            if (context == null)
            {
                canvas = GameObject.FindObjectOfType<Canvas>();
            }
            else
            {
                canvas = context.GetComponentInParent<Transform>()?.GetComponentInChildren<Canvas>();
            }
            if (canvas == null) canvas = UIUtils.SetupUI();
            if (context != null) canvas.transform.SetParent(context.transform);
            return canvas;
        }
        public static Canvas SetupUI()
        {
            Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.gameObject.AddComponent<CanvasScaler>();
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            EnsureEventSystem();
            return canvas;
        }
        public static void EnsureEventSystem()
        {
            EventSystem activeEventSystem = GameObject.FindObjectOfType<EventSystem>();
            if (activeEventSystem == null)
            {
                GameObject newEventSystem = new GameObject("EventSystem");
                newEventSystem.AddComponent<EventSystem>();
                newEventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        public static bool IsUnderCanvas(GameObject gameObject)
        {
            return gameObject.transform.GetComponentsInParent<Canvas>().Length > 0;
        }
    }
}