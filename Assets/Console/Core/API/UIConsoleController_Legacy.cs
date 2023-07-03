using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Console.Utils;

namespace Console
{
    /// <summary>
    /// The built-in implimentation of <see cref="UIConsoleControllerCore{TDisplayText, TInputField}"/> using the legacy text system
    /// </summary>
    [AddComponentMenu("UI/UI Console Controller (Legacy)")]
    [ExecuteAlways]
    public class UIConsoleController_Legacy : UIConsoleControllerCore<Text, InputField> 
    {

        protected override bool InputFieldFocused => InputField.isFocused;
        protected override void OnInputHistoryPositionChanged(string inputText)
        {
            // When the user scrolls through their input history, update the input field's text
            InputField.text = inputText;
        }
        protected override void OnInputFieldChanged(InputField from, InputField to)
        {
            // When the input field changes, subscribe SendCommand(string) to the new input field's onSubmit, and unsubscribe the previously set input field if applicable
            from?.onSubmit.RemoveListener(base.SendCommand);
            to?.onSubmit.AddListener(base.SendCommand);
        }
        protected override void OnConsoleLogChanged(string text)
        {
            // Whenever the contents of the console log change, update the displayed text
            if (DisplayText != null)
                DisplayText.text = text;
        }
        protected override void AfterCommandSent(string command)
        {
            // After sending a command, clear the input field
            InputField.text = null;
        }
    }
}