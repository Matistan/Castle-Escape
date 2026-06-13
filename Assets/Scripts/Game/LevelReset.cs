using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelReset
{
    private static bool isResetting;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ClearResetFlag()
    {
        isResetting = false;
    }

    public static void RequestReset()
    {
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
