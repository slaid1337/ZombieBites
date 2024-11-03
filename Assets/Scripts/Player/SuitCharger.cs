using UnityEngine;

public class SuitCharger : MonoBehaviour
{
    [SerializeField] private float _chargeValue;

    public float GetCharge()
    {
        Destroy(gameObject);

        return _chargeValue;
    }
}
