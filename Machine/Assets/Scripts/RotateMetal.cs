using UnityEngine;

public class RotateMetal : MonoBehaviour
{
    /// <summary>
    /// Rotation speed.
    /// </summary>
    [Range(-10f, 10f)] public float rotationsPerSecond = 1f;

    private void Update()
    {
        if (rotationsPerSecond == 0f) return;
        // Rotate object.
        transform.Rotate(Vector3.up, Time.deltaTime * 360f * rotationsPerSecond);
    }
}
