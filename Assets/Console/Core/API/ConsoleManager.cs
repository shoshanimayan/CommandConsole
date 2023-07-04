using System.Collections.Generic;
using System.Reflection;
using Console.Internal;

namespace Console
{

    public class ConsoleManager
    {
        public const string errorColor = "FF512F";

        private static HashSet<IConsoleController> _registeredConsoles = new HashSet<IConsoleController>();
        private static List<string> _history = new List<string>();
        private static int _maxHistoryLength => Config.maxHistoryLength;
        private static List<string> _inputHistory = new List<string>();
        private static int _maxInputHistoryLength => Config.maxInputHistoryLength;
        private static string _cachedText;

        public static int InputHistoryLength { get => _inputHistory.Count; }

        /// <summary>
        /// Begin scanning the specified assembly/s for console commands on a new thread (if they have not been scanned already).
        /// </summary>
        /// <param name="assemblies">The assembly or assemblies to load</param>
        /// <remarks>Use this method if you load an assembly that is imported at runtime (ex from a dll)</remarks>
        public static void BeginLoadingDataFromAssemblies(params Assembly[] assemblies) => CommandRegistry.LoadDataFromAssemblies(multithread:true, assemblies);
        /// <summary>
        /// Synchronously the specified assembly/s for console commands (if they have not been scanned already). This process can take some time depending on the size of the assemblies and the number of commands they contain - if synchronicity is not important, consider using <see cref="BeginLoadingDataFromAssemblies(Assembly[])"/> to load commands in the background instead
        /// </summary>
        /// <param name="assemblies">The assembly or assemblies to load</param>
        /// <remarks>Use this method if you load an assembly that is imported at runtime (ex from a dll)</remarks>
        public static void LoadDataFromAssemblies(params Assembly[] assemblies) => CommandRegistry.LoadDataFromAssemblies(multithread:false, assemblies);
        private static void AppendHistory(string line)
        {
            _history.Insert(0, line);
            if (_history.Count > _maxHistoryLength) _history.RemoveRange(_maxHistoryLength, _history.Count - _maxHistoryLength);
        }
        private static void AppendInputHistory (string input)
        {
            _inputHistory.RemoveAll(previousInput => previousInput == input); // If the new input exists anywhere in the history, remove the old occurence and move it to the front
            _inputHistory.Insert(0, input);
            if (_inputHistory.Count > _maxInputHistoryLength) _inputHistory.RemoveRange(_maxInputHistoryLength, _inputHistory.Count - _maxInputHistoryLength);
        }
        private static void UpdateConsoleTexts()
        {
            string text = "";
            if (_history.Count > 0)
            {
                text = _history[0];
                for (int i = 1; i < _history.Count; i++)
                {
                    string line = _history[i];
                    text = line + '\n' + text;
                }
            }

            _cachedText = text;

            foreach (IConsoleController console in _registeredConsoles)
            {
                console.OnConsoleLogChanged(text);
            }
        }
        /// <summary>
        /// Gets the nth user input, a value of 0 being the most recent input. If an input is repeated, only the most recent occurence is stored in the history
        /// </summary>
        /// <param name="n">How many inputs ago to retrieve</param>
        /// <returns>The <paramref name="n">th</paramref> oldest input</returns>
        public static string GetInputHistoryAt(int n)
        {
            if (n >= 0 && n < _inputHistory.Count) return _inputHistory[n];
            else return null;
        }
        /// <summary>
        /// Get's the current console log
        /// </summary>
        public static string GetLog()
        {
            return _cachedText;
        }
        /// <summary>
        /// Logs a message to the console
        /// </summary>
        /// <param name="text"></param>
        public static void Log(string text, string color = null)
        {
            if (text == null) text = "null";
            if (color != null)
            {
                if (color.Length > 0 && color[0] != '#')
                    color = '#' + color;
                text = $"<color={color}>{text}</color>";
            }
            lock (_history)
            {
                foreach (string line in text.Split('\n')) AppendHistory(line);
            }

            UpdateConsoleTexts();
        }
        /// <summary>
        /// Executes the command at the specified <paramref name="commandPath"/>
        /// </summary>
        /// <param name="commandPath">The full path of the command to execute</param>
        public static void TryExecuteCommand(string commandPath)
        {
            if (commandPath == null) throw new System.ArgumentException($"Command cannot be null");
            if (commandPath.Length == 0) throw new System.ArgumentException($"Command cannot be empty");
            Log($">> {commandPath}");
            AppendInputHistory(commandPath);

            object result = CommandRegistry.TryExecuteCommand(commandPath);

            if (result != null)
                Log(result.ToString());
        }
        /// <summary>
        /// Returns true if any console is active
        /// </summary>
        public static bool IsAnyConsoleActive()
        {
            foreach (IConsoleController consoleController in _registeredConsoles)
                if (consoleController.IsActive) return true;
            return false;
        }
        /// <summary>
        /// Returns true if any console is under the mouse
        /// </summary>
        public static bool IsAnyConsoleFocused ()
        {
            foreach (IConsoleController consoleController in _registeredConsoles)
                if (consoleController.IsFocused) return true;
            return false;
        }
        /// <summary>
        /// Registers a console controller to be notified when the console log is updated
        /// </summary>
        /// <seealso cref="UnregisterConsole(IConsoleController)"/>
        public static void RegisterConsole(IConsoleController console)
        {
            _registeredConsoles.Add(console);
        }
        /// <summary>
        /// Unregisters a console controller so that it is no longer notified when the console log is updated
        /// </summary>
        /// <seealso cref="RegisterConsole(IConsoleController)(IConsoleController)"/>
        public static void UnregisterConsole(IConsoleController console)
        {
            _registeredConsoles.Remove(console);
        }
    }
}