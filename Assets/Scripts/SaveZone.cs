using UnityEngine;

public class SaveZone : MonoBehaviour
{
    private BoxCollider _area;

    private Vector3 _minBounds;
    private Vector3 _maxBounds;

    private void Start()
    {
        _area = GetComponent<BoxCollider>();

        Bounds bounds = _area.bounds;

        // Записываем минимальные и максимальные границы в виде Vector3
        _minBounds = bounds.min;
        _maxBounds = bounds.max;
    }

    public Vector3 GetRandomPoint()
    {
        // Генерируем случайные координаты в пределах заданных границ
        Vector3 randomPosition = new Vector3(
            Random.Range(_minBounds.x, _maxBounds.x),
            Random.Range(_minBounds.y, _maxBounds.y),
            Random.Range(_minBounds.z, _maxBounds.z)
        );

        return randomPosition; // Возвращаем случайную позицию
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Color color = Color.yellow;
        color.a = 0.8f;
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
#endif
}
