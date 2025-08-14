using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay.LevelsData;
using UI;
using UnityEngine;

public partial class ResourceManager : Singleton<ResourceManager>
{
  
    public static event Action<int> CoinsChanged; 
    
    public static bool EnableAds
    {
        get => PrefManager.GetBool(nameof(EnableAds), true);
        set => PrefManager.SetBool(nameof(EnableAds), value);
    }

    public static int Coins
    {
        get => PrefManager.GetInt(nameof(Coins));
        set
        {
            PrefManager.SetInt(nameof(Coins),value);
            CoinsChanged?.Invoke(value);
        }
    }

    protected override void OnInit()
    {
        base.OnInit();
        InitLevels();
    }
    
}


public partial class ResourceManager
{
    [SerializeField]private List<TextAsset> _modeLvlAssets = new List<TextAsset>();

    private readonly Dictionary<Difficulty,List<Level>> _modeAndLevels = new Dictionary<Difficulty, List<Level>>();

    private void InitLevels()
    {
        for (var i = 0; i < _modeLvlAssets.Count; i++)
        {
            _modeAndLevels.Add((Difficulty)i, JsonUtility.FromJson<LevelGroup>(_modeLvlAssets[i].text).ToList());
        }
    }

    public static IEnumerable<Level> GetLevels(Difficulty mode)
    {
        return Instance._modeAndLevels[mode];
    }

    public static Level GetLevel(Difficulty difficulty,int no)
    {
        if(no>=Instance._modeAndLevels[difficulty].Count)
            return new Level();
        return Instance._modeAndLevels[difficulty][no-1];
    }

    public static bool IsLevelLocked(Difficulty difficulty, int no)
    {
        var completedLevel = GetCompletedLevel(difficulty);

        return no > completedLevel + 1;
    }

    public static int GetCompletedLevel(Difficulty difficulty)
    {
       return PrefManager.GetInt($"{difficulty}_Level_Complete");
    }

    public static void CompleteLevel(Difficulty difficulty, int lvl)
    {
        if (GetLevel(difficulty).no>lvl)
        {
            return;
        }

        PrefManager.SetInt($"{difficulty}_Level_Complete",lvl);
    }

    public static bool HasLevel(Difficulty difficulty, int lvl) => GetLevels(difficulty).Count() >= lvl;

    public static Level GetLevel(Difficulty difficulty)
    {
       return GetLevel(difficulty,PrefManager.GetInt($"{difficulty}_Level_Complete")+1);
    }
}