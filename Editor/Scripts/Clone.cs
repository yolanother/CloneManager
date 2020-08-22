using System;
using System.Linq;

namespace Doubtech.CloneManager
{
    [Serializable]
    public class Clone
    {
        public bool isMaster;
        public string path;
        public bool clonedProjectSettings;
    }

    [Serializable]
    public class CloneData
    {
        public Clone master;
        public Clone[] clones = new Clone[0];
        public int Count => null != clones ?  clones.Length : 0;

        public void Add(Clone clone)
        {
            clones = clones.Append(clone).ToArray();
        }

        public void Remove(Clone clone)
        {
            clones = clones.Where(c => c != clone).ToArray();
        }
    }
}