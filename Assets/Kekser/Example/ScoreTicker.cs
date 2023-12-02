using Kekser.PowerSingleton;
using UnityEngine;

namespace Kekser.Example
{
    public class ScoreTicker : MonoBehaviour
    {
        private void Start()
        {
            Single<IGameManager>.Instance.AddScore(10);
        }
    }
}