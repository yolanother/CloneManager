using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace Doubtech.CloneManager
{
    [Serializable]
    public class Clone
    {
        public bool isMaster;
        public string path;
        [FormerlySerializedAs("clonedProjectSettings")] public bool symlinkProjectSettings;
    }

    [Serializable]
    public class CloneData
    {
        public Clone master;
        public List<Clone> clones = new List<Clone>();
        public int Count => clones?.Count ?? 0;

        public void Add(Clone clone)
        {
            clones.Add(clone);
        }

        public void Remove(Clone clone)
        {
            clones.Remove(clone);
        }
    }
}