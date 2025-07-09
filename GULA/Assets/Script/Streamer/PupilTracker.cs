using UnityEngine;

public class PupilTrackerUI : MonoBehaviour
{
    public RectTransform pupil;
    public RectTransform eyeCenter;
    public float maxDistance = 20f;
    public Canvas canvas;

    void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out mousePos
        );

        Vector2 center = eyeCenter.anchoredPosition;
        Vector2 direction = mousePos - center;

        if (direction.magnitude > maxDistance)
            direction = direction.normalized * maxDistance;

        pupil.anchoredPosition = center + direction;
    }
}