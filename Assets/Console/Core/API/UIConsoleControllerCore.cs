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

        [SerializeField] bool _active = true;
        [SerializeField] List<KeyCode> _keysToToggle = new List<KeyCode>(new KeyCode[] { KeyCode.Tilde, KeyCode.BackQuote });
        [SerializeField] TDisplayText _displayText = null;
        [SerializeField] TInputField _inputField = null;
        [SerializeField] RectTransform _consoleTransform = null;
        [SerializeField] bool _allowScrolling = true;
        [SerializeField] float _scrollSensitivity = 6;
        [SerializeField] float _scrollJumpAmount = 40;
        [SerializeField] bool _hideIfNotInEditor;
        TInputField _lastInputField;
        int _historyPosition = -1;
        float _scrollValue = 0;
        /// <summary>
        /// The unity component responsible for displaying the console log
        /// </summary>
        public TDisplayText DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                UpdateDisplayTextScrollPosition();
            }
        }
        /// <summary>
        /// The unity component responsible for handling console input
        /// </summary>
        public TInputField InputField
        {
            get => _inputField;
            set
            {
                _inputField = value;
                OnInputFieldChanged_Internal();
            }
        }
        /// <summary>
        /// Whether the console is active and displayed
        /// </summary>
        public bool ConsoleActive
        {
            get => _active;
            set
            {
                _active = value;
                _consoleTransform.gameObject.SetActive(_active);
            }
        }
        bool Focused
        {
            get
            {
                return
                    _active &&
                    Input.mousePosition.x > _consoleTransform.rect.xMin + _consoleTransform.position.x &&
                    Input.mousePosition.x < _consoleTransform.rect.xMax + _consoleTransform.position.x &&
                    Input.mousePosition.y > _consoleTransform.rect.yMin + _consoleTransform.position.y &&
                    Input.mousePosition.y < _consoleTransform.rect.yMax + _consoleTransform.position.y;
            }
        }
        public RectTransform ConsoleTransform
        {
            get => _consoleTransform;
            set
            {
                _consoleTransform = value;
                _consoleTransform?.gameObject.SetActive(_active);
            }
        }
        public float ScrollValue
        {
            get => _scrollValue;
            set
            {
                if (_allowScrolling)
                {
                    _scrollValue = System.Math.Clamp(value, float.NegativeInfinity, 0);
                    UpdateDisplayTextScrollPosition();
                }
            }
        }
        protected abstract bool InputFieldFocused { get; }


        void Start()
        {
            OnInputFieldChanged_Internal();
            if(Application.isEditor && _hideIfNotInEditor){
                Destroy(gameObject);
            }
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
            foreach (KeyCode key in _keysToToggle)
                if (Input.GetKeyDown(key) &&(!Application.isEditor&&!_hideIfNotInEditor))
                {
                    ConsoleActive = !ConsoleActive;
                }

            if (InputFieldFocused)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    _historyPosition = System.Math.Clamp(_historyPosition + 1, -1, ConsoleManager.InputHistoryLength - 1);
                    OnInputHistoryPositionChanged(ConsoleManager.GetInputHistoryAt(_historyPosition));
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    _historyPosition = System.Math.Clamp(_historyPosition - 1, -1, ConsoleManager.InputHistoryLength - 1);
                    OnInputHistoryPositionChanged(ConsoleManager.GetInputHistoryAt(_historyPosition));
                }
            }
            
            if (Focused)
            {
                if (Input.mouseScrollDelta.y != 0)
                {
                    ScrollValue -= Input.mouseScrollDelta.y * _scrollSensitivity;
                }
            }
        }
        private void OnValidate()
        {
            UpdateDisplayTextScrollPosition();
            if (_inputField != _lastInputField) OnInputFieldChanged_Internal();
            if (_consoleTransform != null)
                ConsoleTransform?.gameObject.SetActive(_active);
        }
        void UpdateDisplayTextScrollPosition ()
        {
            if (_displayText == null) return;
            RectTransform displayTextRect = _displayText.GetComponent<RectTransform>();
            if (displayTextRect == null) return;
            float scrollJumpPosition = ScrollValue - (ScrollValue % _scrollJumpAmount);
            displayTextRect.anchoredPosition = new Vector2(displayTextRect.anchoredPosition.x, scrollJumpPosition);
        }
        bool IConsoleController.IsActive { get => _active; }
        bool IConsoleController.IsFocused { get => Focused; }
        void IConsoleController.OnConsoleLogChanged(string text)
        {
            this.OnConsoleLogChanged(text);
        }
        void OnInputFieldChanged_Internal ()
        {
            OnInputFieldChanged(_lastInputField, _inputField);
            _lastInputField = _inputField;
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
            _historyPosition = -1;
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
