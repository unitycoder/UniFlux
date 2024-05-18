using UnityEditor;
using UnityEngine;

namespace UniFlux.Editor
{
    internal static class UniFluxMenuItems
    {
        [MenuItem("Tools/UniFlux/Open Debugger", priority = 0)] private static void Openw_Window_UniFluxDebuggerWindow()
        {
            EditorWindow.GetWindow<UniFluxDebuggerWindow>(false, "UniFlux Debugger", true);
        }
        [MenuItem("Tools/UniFlux/Open Generator Key", priority = 1)] public static void GenerateExtensionType()
        {
            Rect centerRect = new Rect(
                Screen.width / 2 - 100, 
                Screen.height / 2 - 100, 
                400,
                150
            );
            UniFluxGeneratorKeyWindow window = (UniFluxGeneratorKeyWindow)EditorWindow.GetWindowWithRect(typeof(UniFluxGeneratorKeyWindow), centerRect, true, "Uniflux Generator Key");
            window.ShowPopup();
        }
        // [MenuItem("Tools/UniFlux/Open Packages", priority = 0)] private static void Openw_Window_UniFluxPackagesWindow()
        // {
        //     EditorWindow.GetWindow<UniFluxPackagesWindow>(false, "UniFlux Packages", true);
        // }
        [MenuItem("Tools/UniFlux/📚 Documentation", priority = 100)] private static void OpenDocumentation()
        {
            Application.OpenURL("https://xavierarpa.gitbook.io/uniflux");
        }
        [MenuItem("Tools/UniFlux/📦 Github", priority = 100)] private static void OpenGithub()
        {
            Application.OpenURL("https://github.com/xavierarpa/UniFlux");
        }
        [MenuItem("Tools/UniFlux/👋 Contact", priority = 200)] private static void OpenMail()
        {
            Application.OpenURL("mailto:"+"arpaxavier@gmail.com");
        }
    }
}
