using UnityEngine;

public class RotateMetal : MonoBehaviour
{
    [Range(0.01f,10f)] public float rotationsPerSecond = 1f;

    private void Update()
    {
        transform.Rotate(Vector3.up, Time.deltaTime * 360f * rotationsPerSecond);
    }
}
