
using UnityEngine;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class PermissionsRationaleDialog : MonoBehaviour
{
    const int kDialogWidth = 600;
    const int kDialogHeight = 200;

    private bool windowOpen = true;

    void DoMyWindow(int windowID)
    {
        GUI.Label(new Rect(10, 20, kDialogWidth - 20, kDialogHeight - 10), "This experience records voice messages and shares it with other users. Enable the microhone to continue.");
        GUI.Button(new Rect(10, kDialogHeight - 50, 200, 50), "No");
        if (GUI.Button(new Rect(kDialogWidth - 300, kDialogHeight - 50, 200, 50), "Yes"))

        {
#if PLATFORM_ANDROID     
            Permission.RequestUserPermission(Permission.Microphone);
#endif
            windowOpen = false;
        }
    }

    void OnGUI()
    {
        if (windowOpen)
        {
            Rect rect = new Rect((Screen.width / 2) - (kDialogWidth / 2), (Screen.height / 2) - (kDialogHeight / 2), kDialogWidth, kDialogHeight);
            GUI.ModalWindow(0, rect, DoMyWindow, "Permissions Request Dialog");
        }
    }
}
