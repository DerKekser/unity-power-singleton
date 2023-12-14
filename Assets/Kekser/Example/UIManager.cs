using Kekser.PowerSingleton.Attributes;
using UnityEngine;

namespace Kekser.Example
{
    [PowerSingleton]
    public class UIManager : MonoBehaviour
    {
        public void UpdateScore(int score)
        {
            Debug.Log($"UIManager: UpdateScore({score})");
        }
    }
}