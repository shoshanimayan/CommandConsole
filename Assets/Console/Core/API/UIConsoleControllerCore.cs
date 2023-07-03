using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Console.Utils;
using Console;

namespace Console
{
    /// <summary>
    /// This is a base class for creating custom console controller components. For an implimentation example, see <see cref="UIConsoleController_Legacy"/>
    /// </summary>
    /// <typeparam name="TDisplayText">The component type used for the main text display</typeparam>
    /// <typeparam name="TInputField">The component type used for the console's input field</typeparam>
    [ExecuteAlways]
    public abstract class UIConsoleControllerCore<TDisplayText, TInputField> : MonoBehaviour, IConsoleController
        where TDisplayText : Component where TInputField : Component
    {

        [SerializeField] bool active = true;
        [SerializeField] List<KeyCode> keysToToggle = new List<KeyCode>(new KeyCode[] { KeyCode.Tilde, KeyCode.BackQuote });
        [SerializeField] TDisplayText displayText = null;
        [SerializeField] TInputField inputField = null;
        [SerializeField] RectTransform consoleTransform = null;
        [SerializeField] bool allowScrolling = true;
        [SerializeField] float scrollSensitivity = 6;
        [SerializeField] float scrollJumpAmount = 40;
        TInputField lastInputField;
        int historyPosition = -1;
        float scrollValue = 0;
        /// <summary>
        /// The unity component responsible for displaying the console log
        /// </summary>
        public TDisplayText DisplayText
        {
            get => displayText;
            set
            {
                displayText = value;
                UpdateDisplayTextScrollPosition();
            }
        }
        /// <summary>
        /// The unity component responsible for handling console input
        /// </summary>
        public TInputField InputField
        {
            get => inputField;
            set
            {
                inputField = value;
                OnInputFieldChanged_Internal();
            }
        }
        /// <summary>
        /// Whether the console is active and displayed
        /// </summary>
        public bool ConsoleActive
        {
            get => active;
            set
            {
                active = value;
                consoleTransform.gameObject.SetActive(active);
            }
        }
        bool Focused
        {
            get
            {
                return
                    active &&
                    Input.mousePosition.x > consoleTransform.rect.xMin + consoleTransform.position.x &&
                    Input.mousePosition.x < consoleTransform.rect.xMax + consoleTransform.position.x &&
                    Input.mousePosition.y > consoleTransform.rect.yMin + consoleTransform.position.y &&
                    Input.mousePosition.y < consoleTransform.rect.yMax + consoleTransform.position.y;
            }
        }
        public RectTransform ConsoleTransform
        {
            get => consoleTransform;
            set
            {
                consoleTransform = value;
                consoleTransform?.gameObject.SetActive(active);
            }
        }
        public float ScrollValue
        {
            get => scrollValue;
            set
            {
                if (allowScrolling)
                {
                    scrollValue = System.Math.Clamp(value, float.NegativeInfinity, 0);
                    UpdateDisplayTextScrollPosition();
                }
            }
        }
        protected abstract bool InputFieldFocused { get; }


        void Start()
        {
            OnInputFieldChanged_Internal();
        }
        void OnEnable()
        {
            ((IConsoleController)this).Register();
            OnConsoleLogChanged(((IConsoleController)this).GetLog());
        }
        private void OnDisable()
        {
            ((IConsoleController)this).Unregister();
        }
        private void OnDestroy()
        {
            ((IConsoleController)this).Unregister();
        }
        protected virtual void Update()
        {
            foreach (KeyCode key in keysToToggle)
                if (Input.GetKeyDown(key))
                {
                    ConsoleActive = !ConsoleActive;
                }

            if (InputFieldFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    historyPosition = System.Math.Clamp(historyPosition + 1, -1, ConsoleManager.InputHistoryLength - 1);
                    OnInputHistoryPositionChanged(ConsoleManager.GetInputHistoryAt(historyPosition));
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    historyPosition = System.Math.Clamp(historyPosition - 1, -1, ConsoleManager.InputHistoryLength - 1);
                    OnInputHistoryPositionChanged(ConsoleManager.GetInputHistoryAt(historyPosition));
                }
            }
            
            if (Focused)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    ScrollValue -= Input.mouseScrollDelta.y * scrollSensitivity;
                }
            }
        }
        private void OnValidate()
        {
            UpdateDisplayTextScrollPosition();
            if (inputField != lastInputField) OnInputFieldChanged_Internal();
            if (consoleTransform != null)
                ConsoleTransform?.gameObject.SetActive(active);
        }
        void UpdateDisplayTextScrollPosition ()
        {
            if (displayText == null) return;
            RectTransform displayTextRect = displayText.GetComponent<RectTransform>();
            if (displayTextRect == null) return;
            float scrollJumpPosition = ScrollValue - (ScrollValue % scrollJumpAmount);
            displayTextRect.anchoredPosition = new Vector2(displayTextRect.anchoredPosition.x, scrollJumpPosition);
        }
        bool IConsoleController.IsActive { get => active; }
        bool IConsoleController.IsFocused { get => Focused; }
        void IConsoleController.OnConsoleLogChanged(string text)
        {
            this.OnConsoleLogChanged(text);
        }
        void OnInputFieldChanged_Internal ()
        {
            OnInputFieldChanged(lastInputField, inputField);
            lastInputField = inputField;
        }
        /// <summary>
        /// Attempts to execute the specified string as a console command
        /// </summary>
        /// <param name="formattedCommand"></param>
        /// <seealso cref="ConsoleManager"/>
        protected void SendCommand(string formattedCommand)
        {
            ((IConsoleController)this).SendCommand(formattedCommand);
            AfterCommandSent(formattedCommand);
            historyPosition = -1;
            ScrollValue = 0;
        }
        /// <summary>
        /// Triggered when the <see cref="InputField"/> is set to a new value. Subscribe and unsubscribe to input field events in here
        /// </summary>
        /// <param name="previousInputField">The previous input field</param>
        /// <param name="newInputField">The new input field</param>
        /// <remarks><paramref name="previousInputField"/> and/or <paramref name="newInputField"/> may be null</remarks>
        protected abstract void OnInputFieldChanged(TInputField previousInputField, TInputField newInputField);
        /// <summary>
        /// Triggered when the console log has changed. Update the displayed text in here.
        /// </summary>
        /// <seealso cref="DisplayText"/>
        protected abstract void OnConsoleLogChanged(string text);
        protected abstract void OnInputHistoryPositionChanged(string inputText);
        /// <summary>
        /// Triggered after calling <see cref="SendCommand(string)"/>. Useful for clearing the input field's text.
        /// </summary>
        protected virtual void AfterCommandSent(string command) { }

    }
}
