using UnityEngine;
using UnityEngine.UI;

public class CircleIndicator : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    /// <summary>
    /// Update ui status
    /// </summary>
    /// <param name="value">value from 0 to 1</param>
    public void UpdateStatus(float value)
    {
        _fillImage.fillAmount = value;
    }
}
