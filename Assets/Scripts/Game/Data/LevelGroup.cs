using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.LevelsData
{
    [Serializable]
    public struct LevelGroup:IEnumerable<Level>
    {
        public List<Level> levels;
        public IEnumerator<Level> GetEnumerator()
        {
            return levels?.GetEnumerator() ?? Enumerable.Empty<Level>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}