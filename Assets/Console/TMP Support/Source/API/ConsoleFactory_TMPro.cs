using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Console.Utils;

namespace Console.TMPro
{
    public static class ConsoleFactory_TMPro
    {
        // Generation
        public static UIConsoleController_TMPro Generate(RectTransform parent, float sizeOfInputBox = 40)
        {
            if (parent == null) throw new System.ArgumentException($"Console object's parent cannot be null");
            RectTransform consoleController_Transform = new GameObject("UI Console Controller (TMPro)", typeof(RectTransform), typeof(UIConsoleController_TMPro)).GetComponent<RectTransform>();
            UIConsoleController_TMPro consoleController = consoleController_Transform.GetComponent<UIConsoleController_TMPro>();
            consoleController_Transform.SetParent(parent);
            consoleController_Transform.GetComponent<RectTransform>().SetAnchorsAndMargins(Vector4.zero, Vector4.zero);

            RectTransform consoleWrapper_Transform = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).GetComponent<RectTransform>();
            consoleWrapper_Transform.SetParent(consoleController_Transform);
            consoleWrapper_Transform.StretchToCorners();
            Image consoleWrapperImage = consoleWrapper_Transform.gameObject.GetComponent<Image>();
            consoleWrapperImage.sprite = null;
            consoleWrapperImage.color = new Color(0.1f, 0.1f, 0.1f, 0.65f);
            consoleController.ConsoleTransform = consoleWrapper_Transform;

            RectTransform displayTextMask = new GameObject("Display Text Mask", typeof(RectTransform), typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
            displayTextMask.GetComponent<Mask>().showMaskGraphic = false;
            displayTextMask.SetParent(consoleWrapper_Transform);
            displayTextMask.SetAnchorsAndMargins
                (
                new Vector4(0, 0, 1, 1),
                new Vector4(3, sizeOfInputBox, 3, 0)
                );
            RectTransform displayText = new GameObject("Display Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
            displayText.SetParent(displayTextMask);
            displayText.SetAnchorsAndMargins(new Vector4(0, 0, 1, 0), Vector4.zero);
            TextMeshProUGUI displayText_Text = displayText.GetComponent<TextMeshProUGUI>();
            displayText_Text.color = Color.white;
            displayText_Text.alignment = TextAlignmentOptions.BottomLeft;
            displayText_Text.richText = true;
            displayText_Text.fontSize = 12;
            displayText_Text.overflowMode = TextOverflowModes.Masking;
            consoleController.DisplayText = displayText_Text;

            RectTransform inputField_Transform = new GameObject("Input Field", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(TMP_InputField)).GetComponent<RectTransform>();
            TMP_InputField inputField = inputField_Transform.GetComponent<TMP_InputField>();
            ColorBlock colors = inputField.colors = new ColorBlock();
            inputField_Transform.transform.SetParent(consoleWrapper_Transform);
            inputField_Transform.SetAnchorsAndMargins
                (
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, -sizeOfInputBox)
                );
            colors.colorMultiplier = 1;
            colors.normalColor = Color.black;
            colors.highlightedColor = Color.black;
            colors.pressedColor = Color.black;
            colors.selectedColor = Color.black;
            inputField.colors = colors;


            RectTransform inputFieldTextArea_Transform = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D)).GetComponent<RectTransform>();
            inputFieldTextArea_Transform.SetParent(inputField_Transform);
            inputFieldTextArea_Transform.SetAnchorsAndMargins(new Vector4(0, 0, 1, 1), Vector4.one * 2);

            int inputTextMaxFontSize = Mathf.CeilToInt(sizeOfInputBox * 0.6f);

            RectTransform inputFieldPlaceholder = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
            inputFieldPlaceholder.SetParent(inputFieldTextArea_Transform);
            inputFieldPlaceholder.StretchToCorners();
            TextMeshProUGUI inputFieldPlaceholder_Text = inputFieldPlaceholder.GetComponent<TextMeshProUGUI>();
            inputFieldPlaceholder_Text.richText = false;
            inputFieldPlaceholder_Text.enableAutoSizing = true;
            inputFieldPlaceholder_Text.fontSizeMax = inputTextMaxFontSize;
            inputFieldPlaceholder_Text.fontSizeMin = 0;
            inputFieldPlaceholder_Text.fontStyle = FontStyles.Italic;
            inputFieldPlaceholder_Text.alignment = TextAlignmentOptions.MidlineLeft;
            inputFieldPlaceholder_Text.color = Color.white * 0.35f;
            inputField.placeholder = inputFieldPlaceholder_Text;

            RectTransform inputText = new GameObject("Input Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI)).GetComponent<RectTransform>();
            inputText.SetParent(inputFieldTextArea_Transform);
            inputText.StretchToCorners();
            TextMeshProUGUI inputText_Text = inputText.GetComponent<TextMeshProUGUI>();
            inputText_Text.richText = false;
            inputText_Text.enableAutoSizing = true;
            inputText_Text.fontSizeMax = inputTextMaxFontSize;
            inputText_Text.fontSizeMin = 0;
            inputText_Text.alignment = TextAlignmentOptions.MidlineLeft;
            inputText_Text.color = Color.white;
            inputField.textComponent = inputText_Text;
            consoleController.InputField = inputField;

            consoleController.ConsoleActive = false;

            return consoleController;
        }
    }
}
