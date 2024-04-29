using UnityEngine;

namespace eBMasterData
{
    [System.Serializable]
    public class Core
    {
        private int count;

        public void Log()
        {
            count++;
            Debug.Log($"Log: {count}");
        }
    }
}
