using UnityEngine;

public partial class GameSettings : ScriptableObject
{
    public const string DEFAULT_NAME = nameof(GameSettings);
}

public partial class GameSettings
{
    public static GameSettings Default => Resources.Load<GameSettings>(nameof(GameSettings));

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/GameSettings")]
    public static void OpenGameSettings()
    {
        GameplayEditorManager.OpenScriptableAtDefault<GameSettings>();
    }
#endif
}


[System.Serializable]
public struct LeadersboardSetting
{
    public bool enable;
    public string leadersboardId;
}
