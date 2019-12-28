using UnityEngine;

public class KnifeControll : MonoBehaviour
{
    /// <summary>
    /// Vertical speed of knife.
    /// </summary>
    [Range(0.05f, 10f)] public float moveSpeed = 1f;
    /// <summary>
    /// Horizontal speed of knife.
    /// </summary>
    [Range(0.05f, 10f)] public float downSpeed = 1f;
    /// <summary>
    /// Knife movement enabled.
    /// </summary>
    public static bool run = true;
    /// <summary>
    /// Maximum X position of knife.
    /// </summary>
    public static float maxX = 5f;
    /// <summary>
    /// Minimum X position of knife.
    /// </summary>
    public static float minX = 0.7f;
    /// <summary>
    /// Maximum Y position of knife.
    /// </summary>
    public static float maxY = 10f;
    /// <summary>
    /// Minimum Y position of knife.
    /// </summary>
    public static float minY = -0.5f;
    /// <summary>
    /// Number changed by touch input. Responsible for movement on X axis.
    /// </summary>
    private float downward;
    /// <summary>
    /// Bool responsible for movement on Y axis.
    /// </summary>
    private bool goingUp = true;

    private void Update()
    {
        if (!run) return;
        // Check boundaries on Y axis.
        CheckPosition();
        // Setting movement on X axis.
        downward = Input.GetMouseButton(0) ? -1f : 1f;
        // Calculating new position of knife.
        Vector3 pos = transform.position + (new Vector3(downward * downSpeed, moveSpeed * (goingUp ? 1f : -1f), 0f) * Time.deltaTime);
        // Clamping new position with X axis boundaries.
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }

    /// <summary>
    /// Check boundaries on Y axis.
    /// </summary>
    private void CheckPosition()
    {
        if (!goingUp && transform.position.y < minY) goingUp = true;
        else if (goingUp && transform.position.y > maxY) goingUp = false;
    }
}
