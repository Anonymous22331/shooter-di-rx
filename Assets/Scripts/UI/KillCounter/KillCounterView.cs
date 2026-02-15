using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace UI
{
    public class KillCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;

        [Inject]
        public void Construct(IKillCounterController service)
        {
            service.KillCount
                .Subscribe(count =>
                    label.text = $"Kills: {count}");
        }
    }
}
