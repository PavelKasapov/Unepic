using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    [SerializeField] private Slider bar;

    protected void ChangeValue(int value)
    {
        bar.value = value;
    }

    protected void ChangeMaxValue(int value)
    {
        bar.maxValue = value;
    }
}
