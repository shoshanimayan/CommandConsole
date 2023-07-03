using UnityEngine;
using TMPro;

namespace Console.TMPro
{
    [AddComponentMenu("UI/UI Console Controller (TMPro)")]
    [ExecuteAlways]
    public class UIConsoleController_TMPro : UIConsoleControllerCore<TextMeshProUGUI, TMP_InputField>
    {
        protected override bool InputFieldFocused => InputField.isFocused;
        protected override void OnInputHistoryPositionChanged(string inputText)
        {
            // When the user scrolls through their input history, update the input field's text
            InputField.text = inputText;
        }
        protected override void OnInputFieldChanged(TMP_InputField from, TMP_InputField to)
        {
            from?.onSubmit.RemoveListener(base.SendCommand);
            to?.onSubmit.AddListener(base.SendCommand);
        }
        protected override void OnConsoleLogChanged(string text)
        {
            if (DisplayText != null)
                DisplayText.text = text;
        }
        protected override void AfterCommandSent(string command)
        {
            InputField.text = null;
        }
    }
}