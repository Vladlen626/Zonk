using UnityEngine;
using System.Collections;
using System; // Для использования Action

public class BezierMover : MonoBehaviour
{
    // Типы easing функций
    public enum EaseType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    // Метод для запуска движения объекта по кривой Безье
    public static IEnumerator MoveAlongCurve(Transform objectToMove, Vector3[] path, float duration, EaseType easeType = EaseType.Linear, Action onComplete = null)
    {
        if (path == null || path.Length < 2)
        {
            Debug.LogError("Path must contain at least 2 points.");
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            // Применяем easing функцию
            t = ApplyEasing(t, easeType);

            // Вычисляем позицию на кривой Безье
            objectToMove.position = CalculateBezierPoint(t, path);
            yield return null; // Ждем следующего кадра
        }

        // Завершаем движение, устанавливая объект в конечную точку
        objectToMove.position = CalculateBezierPoint(1f, path);

        // Вызываем callback, если он был передан
        onComplete?.Invoke();
    }

    // Метод для вычисления точки на кривой Безье
    private static Vector3 CalculateBezierPoint(float t, Vector3[] points)
    {
        Vector3[] tempPoints = (Vector3[])points.Clone();
        int n = tempPoints.Length - 1;

        for (int r = 1; r <= n; r++)
        {
            for (int i = 0; i <= n - r; i++)
            {
                tempPoints[i] = Vector3.Lerp(tempPoints[i], tempPoints[i + 1], t);
            }
        }

        return tempPoints[0];
    }

    // Применение easing функции
    private static float ApplyEasing(float t, EaseType easeType)
    {
        switch (easeType)
        {
            case EaseType.Linear:
                return t; // Без изменений
            case EaseType.EaseIn:
                return t * t; // Плавный старт
            case EaseType.EaseOut:
                return 1 - (1 - t) * (1 - t); // Плавная остановка
            case EaseType.EaseInOut:
                return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2; // Плавный старт и остановка
            default:
                return t;
        }
    }

    // Визуализация кривой Безье в редакторе (опционально)
    public static void DrawGizmos(Vector3[] path)
    {
        if (path == null || path.Length < 2)
            return;

        Gizmos.color = Color.green;
        const float step = 0.02f;

        Vector3 prevPoint = path[0];
        for (float t = 0; t <= 1; t += step)
        {
            Vector3 nextPoint = CalculateBezierPoint(t, path);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        Gizmos.color = Color.red;
        foreach (var point in path)
        {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
