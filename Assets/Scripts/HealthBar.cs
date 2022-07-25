using UniRx;
using UnityEngine;

public class HealthBar : UIBar
{
    [SerializeField] private CharacterData characterData;

    private void Awake()
    {
        ChangeMaxValue(characterData.Life.Value);
        characterData.Life.Subscribe(ChangeValue).AddTo(this);
    }
}
