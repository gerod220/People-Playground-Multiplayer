using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

namespace CheatMenu.GUI
{
    public class UIBuilder : MonoBehaviour
    {

        /// <summary>
        /// Create text
        /// </summary>
        public static TextMeshProUGUI CreateText(string content, int fontSize, Color color, Vector3 position, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
        {
            GameObject textObj = new GameObject("New Text");
            textObj.transform.SetParent(UIElements.GetCanvas.transform);
            textObj.transform.position = position;
            RectTransform rectText = textObj.AddComponent<RectTransform>();
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            textObj.AddComponent<ContentSizeFitter>();
            return text;
        }
        public static TextMeshProUGUI CreateText(string content, int fontSize, Color color, Transform parent, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
        {
            GameObject textObj = new GameObject("New Text");
            textObj.transform.SetParent(UIElements.GetCanvas.transform);
            textObj.transform.SetParent(parent);
            RectTransform rectText = textObj.AddComponent<RectTransform>();
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            textObj.AddComponent<ContentSizeFitter>();
            return text;
        }
        /// <summary>
        /// Create button
        /// </summary>
        public static Button CreateButton(Vector3 position, Vector2 sizeDelta)
        {
            GameObject buttonObj = new GameObject("New Button");
            buttonObj.transform.SetParent(UIElements.GetCanvas.transform);
            buttonObj.transform.position = position;
            RectTransform rectText = buttonObj.AddComponent<RectTransform>();
            rectText.sizeDelta = sizeDelta;
            buttonObj.AddComponent<Image>();
            Button button = buttonObj.AddComponent<Button>();
            return button;
        }
        public static Button CreateButton(Sprite sprite, Vector3 position, Vector2 sizeDelta)
        {
            GameObject buttonObj = new GameObject("New Button");
            buttonObj.transform.SetParent(UIElements.GetCanvas.transform);
            buttonObj.transform.position = position;
            RectTransform rectText = buttonObj.AddComponent<RectTransform>();
            rectText.sizeDelta = sizeDelta;
            buttonObj.AddComponent<Image>().sprite = sprite;
            Button button = buttonObj.AddComponent<Button>();
            return button;
        }
        /// <summary>
        /// Create image
        /// </summary>
        public static Image CreateImage(Sprite sprite, Vector3 position, Vector2 sizeDelta)
        {
            GameObject imageObj = new GameObject("New Image");
            imageObj.transform.SetParent(UIElements.GetCanvas.transform);
            imageObj.transform.position = position;
            RectTransform rectImage = imageObj.AddComponent<RectTransform>();
            rectImage.sizeDelta = sizeDelta;
            Image image = imageObj.AddComponent<Image>();
            image.sprite = sprite;
            return image;
        }
        public static Image CreateImage(Vector3 position, Vector2 sizeDelta)
        {
            GameObject imageObj = new GameObject("New Image");
            imageObj.transform.SetParent(UIElements.GetCanvas.transform);
            imageObj.transform.position = position;
            RectTransform rectImage = imageObj.AddComponent<RectTransform>();
            rectImage.sizeDelta = sizeDelta;
            Image image = imageObj.AddComponent<Image>();
            return image;
        }
        /// <summary>
        /// Create vertical layout group
        /// </summary>
        public static VerticalLayoutGroup CreateVerticalLayoutGroup(Vector3 position, Vector2 sizeDelta)
        {
            GameObject groupObj = new GameObject("New LayoutGroup");
            groupObj.transform.SetParent(UIElements.GetCanvas.transform);
            groupObj.transform.position = position;
            RectTransform rectGroup = groupObj.AddComponent<RectTransform>();
            rectGroup.sizeDelta = sizeDelta;
            VerticalLayoutGroup group = groupObj.AddComponent<VerticalLayoutGroup>();
            group.childControlHeight = false;
            group.childControlWidth = false;
            group.childForceExpandHeight = false;
            group.childAlignment = TextAnchor.UpperCenter;
            return group;
        }
    }

    public class UIElements : MonoBehaviour
    {
        public static Canvas GetCanvas => FindObjectOfType<Canvas>();

        public static Vector3 CANVAS_SIZE => GetCanvas.pixelRect.size;

        public static Vector3 CANVAS_CENTER => new Vector3(CANVAS_SIZE.x / 2, CANVAS_SIZE.y / 2);

        public static Vector3 CANVAS_TOP_RIGHT => new Vector3(CANVAS_SIZE.x, CANVAS_SIZE.y);

        public static Vector3 CANVAS_TOP_LEFT => new Vector3(0, CANVAS_SIZE.y);

        public static Vector3 CANVAS_TOP_CENTER => new Vector3(CANVAS_SIZE.x / 2, CANVAS_SIZE.y);

        public static Vector3 CANVAS_BOTTOM_RIGHT => new Vector3(CANVAS_SIZE.x, 0);

        public static Vector3 CANVAS_BOTTOM_LEFT => new Vector3(0, 0);

        public static Vector3 CANVAS_BOTTOM_CENTER => new Vector3(CANVAS_SIZE.x / 2, 0);

        public static float CANVAS_SIZE_X => CANVAS_SIZE.x;

        public static float CANVAS_SIZE_Y => CANVAS_SIZE.y;
    }
}
