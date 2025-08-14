using System;
using System.Collections.Generic;

namespace Gameplay.LevelsData
{
    [Serializable]
    public struct Level
    {
        public int no;
        public List<LevelColumn> map;
    }
}