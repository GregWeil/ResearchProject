using UnityEngine;
using System.Collections;

public class GenericFilters {

    public static bool isTrue(bool value) {
        return value;
    }

    [UnityEditor.Callbacks.DidReloadScripts()]
    public static void InstallLibrary() {
        Debug.Log("Reload");
    }
}
