using Kekser.PowerSingleton;
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Example
{
    [PowerSingleton(typeof(IGameManager), Creation = PowerSingletonCreation.IfNeeded, CreationName = "GameManager", DontDestroyOnLoad = true)]
    public class GameManager : MonoBehaviour, IGameManager
    {
        public void AddScore(int score)
        {
            Debug.Log($"GameManager: AddScore({score})");
            Single<UIManager>.Instance.UpdateScore(score);
        }
    }
}