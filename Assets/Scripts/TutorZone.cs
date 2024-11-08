using UnityEngine;

public class TutorZone : MonoBehaviour
{
    [SerializeField] private Color _color;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TutorController.Instance.NextStep();
            gameObject.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = _color;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
#endif
}
