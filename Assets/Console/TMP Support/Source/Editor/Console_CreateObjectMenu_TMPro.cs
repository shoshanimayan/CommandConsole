using UnityEditor;
using UnityEngine;
using Console.TMPro;
using Console.Utils;

namespace Console.Editor.TMPro
{
    public static class Console_CreateObjectMenu_TMPro
    {
        const string TMProConsoleName = "Console (TMPro)";
        [MenuItem("GameObject/UI/Consoles/" + TMProConsoleName)]
        static void CreateLegacyConsole(MenuCommand menuCommand)
        {
            GameObject selectedObject = menuCommand.context as GameObject;
            Canvas canvas = UIUtils.GetOrCreateCanvasInContext(selectedObject);
            RectTransform targetParentTransform = selectedObject?.GetComponent<RectTransform>() != null
                ? selectedObject.transform as RectTransform
                : canvas.transform as RectTransform;
            UIConsoleController_TMPro consoleController = ConsoleFactory_TMPro.Generate(targetParentTransform);
            consoleController.GetComponent<RectTransform>().SetAnchorsAndMargins(new Vector4(0.6f, 0.06f, 0.94f, 0.94f), Vector4.zero);
            Undo.RegisterCreatedObjectUndo(consoleController.gameObject, $"Create {TMProConsoleName}");
        }
    }
}