using UnityEngine;
using UnityEngine.UI;
using Console.Utils;

namespace Console
{
    public static class ConsoleFactory_Legacy
    {
        public static UIConsoleController_Legacy Generate(RectTransform parent, float sizeOfInputBox = 40)
        {
            if (parent == null) throw new System.ArgumentException($"Console object's parent cannot be null");
            RectTransform consoleController_Transform = new GameObject("UI Console Controller (Legacy Text)", typeof(RectTransform), typeof(UIConsoleController_Legacy)).GetComponent<RectTransform>();
            UIConsoleController_Legacy consoleController = consoleController_Transform.GetComponent<UIConsoleController_Legacy>();
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
                new Vector4(0, sizeOfInputBox, 0, 0)
                );

            RectTransform displayText = new GameObject("Display Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<RectTransform>();
            displayText.SetParent(displayTextMask);
            displayText.SetAnchorsAndMargins(new Vector4(0, 0, 1, 0), Vector4.zero);
            Text displayText_Text = displayText.GetComponent<Text>();
            consoleController.DisplayText = displayText_Text;
            displayText_Text.alignment = TextAnchor.LowerLeft;
            displayText_Text.verticalOverflow = VerticalWrapMode.Overflow;

            RectTransform inputField_Transform = new GameObject("Input Field", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(InputField)).GetComponent<RectTransform>();
            InputField inputField = inputField_Transform.GetComponent<InputField>();
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

            RectTransform inputFieldPlaceholder = new GameObject("Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<RectTransform>();
            inputFieldPlaceholder.SetParent(inputFieldTextArea_Transform);
            inputFieldPlaceholder.StretchToCorners();
            Text inputFieldPlaceholder_Text = inputFieldPlaceholder.GetComponent<Text>();
            inputFieldPlaceholder_Text.supportRichText = false;
            inputFieldPlaceholder_Text.resizeTextForBestFit = true;
            inputFieldPlaceholder_Text.resizeTextMaxSize = inputTextMaxFontSize;
            inputFieldPlaceholder_Text.resizeTextMinSize = 0;
            inputFieldPlaceholder_Text.fontStyle = FontStyle.Italic;
            inputFieldPlaceholder_Text.alignment = TextAnchor.MiddleLeft;
            inputFieldPlaceholder_Text.color = Color.white * 0.35f;
            inputField.placeholder = inputFieldPlaceholder_Text;

            RectTransform inputText = new GameObject("Input Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text)).GetComponent<RectTransform>();
            inputText.SetParent(inputFieldTextArea_Transform);
            inputText.StretchToCorners();
            Text inputText_Text = inputText.GetComponent<Text>();
            inputText_Text.supportRichText = false;
            inputText_Text.resizeTextForBestFit = true;
            inputText_Text.resizeTextMaxSize = inputTextMaxFontSize;
            inputText_Text.resizeTextMinSize = 0;
            inputFieldPlaceholder_Text.alignment = TextAnchor.MiddleLeft;
            inputText_Text.color = Color.white;
            inputField.textComponent = inputText_Text;
            consoleController.InputField = inputField;

            consoleController.ConsoleActive = false;

            return consoleController;
        }
    }
}
