using UnityEngine;

public class LevelGenerator : ScriptableObject
{
    public const string TARGET_SWAP_RANGE = nameof(_targetSwapRange);
    public const string TARGET_GROUP_RANGE = nameof(_targetGroupRange);
    public const string EXTRA_HOLDER_RANGE = nameof(_extraHolderRange);
    public const string TARGET_LEVEL_COUNT = nameof(_targetLevelCount);
    
    [SerializeField] private Vector2Int _targetSwapRange;
    [SerializeField] private Vector2Int _targetGroupRange;
    [SerializeField] private Vector2Int _extraHolderRange;
    [SerializeField] private int _targetLevelCount;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("MyGames/Level Generator")]
    public static void Open()
    {
        GameplayEditorManager.OpenScriptableAtDefault<LevelGenerator>();
    }
#endif
}