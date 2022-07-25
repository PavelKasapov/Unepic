using System;
using UniRx;
using UnityEngine;

public abstract class CharacterData : MonoBehaviour
{
    public ReactiveProperty<int> Life;
}
