using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public Vector3 offset;   // Смещение камеры относительно игрока
    public float smoothing = 5f; // Параметр для сглаживания движения камеры

    private void Awake()
    {
        //offset = player.InverseTransformPoint(transform.position);
    }

    private void FixedUpdate()
    {
        // Проверка, что ссылка на игрока установлена
        if (player != null)
        {
            // Вычисляем новую позицию камеры
            Vector3 targetCamPos = player.position + offset;
            // Плавно перемещаем камеру к цели
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }

    [ContextMenu("CalculateOffset")]
    private void CalculateOffset()
    {
        offset = player.InverseTransformPoint(transform.position);
    }
}