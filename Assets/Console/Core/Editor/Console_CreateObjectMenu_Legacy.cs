using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Console.Utils;

namespace Console.Editor
{
    public static class Console_CreateObjectMenu_Legacy
    {
        const string legacyConsoleName = "Console (Legacy)";
        [MenuItem("GameObject/UI/Consoles/" + legacyConsoleName)]
        static void CreateLegacyConsole(MenuCommand menuCommand)
        {
            GameObject selectedObject = menuCommand.context as GameObject;
            Canvas canvas = UIUtils.GetOrCreateCanvasInContext(selectedObject);
            RectTransform targetParentTransform = selectedObject?.GetComponent<RectTransform>() != null
                ? selectedObject.transform as RectTransform
                : canvas.transform as RectTransform;
            UIConsoleController_Legacy consoleController = ConsoleFactory_Legacy.Generate(targetParentTransform);
            consoleController.GetComponent<RectTransform>().SetAnchorsAndMargins(new Vector4(0.6f, 0.06f, 0.94f, 0.94f), Vector4.zero);
            Undo.RegisterCreatedObjectUndo(consoleController.gameObject, $"Create {legacyConsoleName}");
        }
    }
}