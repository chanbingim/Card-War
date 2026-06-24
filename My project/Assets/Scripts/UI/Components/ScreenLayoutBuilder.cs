using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TurnCardGame.UI.Components
{
    public static class ScreenLayoutBuilder
    {
        private static readonly Color PageBackground = new Color(0.94f, 0.96f, 0.96f);
        private static readonly Color TextPrimary = new Color(0.05f, 0.08f, 0.1f);
        private static readonly Color TextSecondary = new Color(0.2f, 0.24f, 0.27f);

        public static RectTransform CreateScreen(string name)
        {
            EnsureEventSystem();

            var canvasObject = new GameObject(name + " Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var backgroundObject = new GameObject("Background", typeof(RectTransform), typeof(Image));
            backgroundObject.transform.SetParent(canvasObject.transform, false);
            RectTransform background = backgroundObject.GetComponent<RectTransform>();
            background.anchorMin = Vector2.zero;
            background.anchorMax = Vector2.one;
            background.offsetMin = Vector2.zero;
            background.offsetMax = Vector2.zero;
            backgroundObject.GetComponent<Image>().color = PageBackground;

            var rootObject = new GameObject("Root", typeof(RectTransform), typeof(VerticalLayoutGroup));
            rootObject.transform.SetParent(canvasObject.transform, false);
            RectTransform root = rootObject.GetComponent<RectTransform>();
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.sizeDelta = new Vector2(1120f, 860f);

            VerticalLayoutGroup layout = rootObject.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(36, 36, 36, 36);
            layout.spacing = 14f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            return root;
        }

        public static Text AddTitle(Transform parent, string text)
        {
            Text label = AddText(parent, text, 46, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = TextPrimary;
            label.GetComponent<LayoutElement>().preferredHeight = 68f;
            return label;
        }

        public static Text AddSection(Transform parent, string text)
        {
            Text label = AddText(parent, text, 26, FontStyle.Bold, TextAnchor.MiddleCenter);
            label.color = TextPrimary;
            label.GetComponent<LayoutElement>().preferredHeight = 42f;
            return label;
        }

        public static Text AddBody(Transform parent, string text, TextAnchor alignment = TextAnchor.MiddleCenter)
        {
            Text label = AddText(parent, text, 20, FontStyle.Normal, alignment);
            label.color = TextSecondary;
            label.GetComponent<LayoutElement>().preferredHeight = 34f;
            return label;
        }

        public static RectTransform AddRow(Transform parent, string name, float height = 64f)
        {
            var rowObject = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            rowObject.transform.SetParent(parent, false);
            HorizontalLayoutGroup layout = rowObject.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 12f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            rowObject.GetComponent<LayoutElement>().preferredHeight = height;
            return rowObject.GetComponent<RectTransform>();
        }

        public static RectTransform AddColumn(Transform parent, string name, float width)
        {
            var columnObject = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            columnObject.transform.SetParent(parent, false);
            VerticalLayoutGroup layout = columnObject.GetComponent<VerticalLayoutGroup>();
            layout.spacing = 10f;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            LayoutElement element = columnObject.GetComponent<LayoutElement>();
            element.preferredWidth = width;
            element.flexibleWidth = 0f;
            return columnObject.GetComponent<RectTransform>();
        }

        private static Text AddText(Transform parent, string text, int size, FontStyle style, TextAnchor alignment)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            labelObject.transform.SetParent(parent, false);
            Text label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = size;
            label.fontStyle = style;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 14;
            label.resizeTextMaxSize = size;
            labelObject.GetComponent<LayoutElement>().preferredWidth = 980f;
            return label;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }
}
