using UnityEngine;

public static class Serializer
{
    public static float Normalize(float value, float min, float max)
    {
        // Ограничиваем значение в пределах минимального и максимального значений
        value = Mathf.Clamp(value, min, max);

        // Нормализуем значение в диапазоне [0, 1]
        return (value - min) / (max - min);
    }
}
