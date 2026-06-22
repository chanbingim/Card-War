using System;
using UnityEngine;
using UnityEngine.UI;

namespace TurnCardGame.UI.Components
{
    public sealed class PrimaryButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Text label;

        public static PrimaryButton Create(Transform parent, string text, Action onClick)
        {
            var root = new GameObject(text + " Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(PrimaryButton));
            root.transform.SetParent(parent, false);
            var rect = root.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(220f, 48f);

            var image = root.GetComponent<Image>();
            image.color = new Color(0.14f, 0.29f, 0.42f);

            var primaryButton = root.GetComponent<PrimaryButton>();
            primaryButton.button = root.GetComponent<Button>();
            primaryButton.button.targetGraphic = image;
            primaryButton.button.onClick.AddListener(() => onClick?.Invoke());

            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(root.transform, false);
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(12f, 6f);
            labelRect.offsetMax = new Vector2(-12f, -6f);

            primaryButton.label = labelObject.GetComponent<Text>();
            primaryButton.label.text = text;
            primaryButton.label.alignment = TextAnchor.MiddleCenter;
            primaryButton.label.color = Color.white;
            primaryButton.label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            primaryButton.label.fontSize = 18;

            return primaryButton;
        }
    }
}
