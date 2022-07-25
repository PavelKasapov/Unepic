using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Slider bar;

    protected void ChangeValue(int value)
    {
        Debug.Log($"!! ChangeValue {value}");
        bar.value = value;
    }

    protected void ChangeMaxValue(int value)
    {
        Debug.Log($"!! ChangeMaxValue {value}");
        bar.maxValue = value;
    }
}
