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
    /// Enums for knife movement style.
    /// </summary>
    public enum MoveStyle { Dynamic, Stable };
    /// <summary>
    /// Chosen knife movement style.
    /// </summary>
    public MoveStyle moveStyle = MoveStyle.Dynamic;
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
    /// Variable used to control the frequency of checking the shape.
    /// Setting to "true" means that the blade is in the rest position.
    /// </summary>
    public static bool needToCheckShape = false;
    /// <summary>
    /// Variable used to control the frequency of checking the shape.
    /// Setting to "true" means that the blade is in the rest position and the shape of the mesh has been checked.
    /// </summary>
    public static bool alreadyChecked = false;
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
        // Moving the knife.
        Move(moveStyle);
    }

    /// <summary>
    /// Check boundaries on Y axis.
    /// </summary>
    private void CheckPosition()
    {
        if (!goingUp && transform.position.y < minY) goingUp = true;
        else if (goingUp && transform.position.y > maxY) goingUp = false;
    }

    /// <summary>
    /// Choosing a good knife movement style based on input data.
    /// </summary>
    /// <param name="moveStyle"></param>
    private void Move(MoveStyle moveStyle)
    {
        switch(moveStyle)
        {
            case MoveStyle.Dynamic:
                DynamicMove();
                break;
            case MoveStyle.Stable:
                StableMove();
                break;
            default: break;
        }
    }

    /// <summary>
    /// Dynamic move of knife based on actual touch input.
    /// </summary>
    private void DynamicMove()
    {
        // Setting movement on X axis.
        downward = Input.GetMouseButton(0) ? -1f : 1f;
        // Calculation of the new knife position
        CalculateNewPosition();
    }

    /// <summary>
    /// Stable move of knife based on actual and previous touch inputs.
    /// </summary>
    private void StableMove()
    {
        // Setting movement on X axis.
        if (Input.GetMouseButton(0)) downward = (Input.mousePosition.x / Screen.width <= 0.5f) ? -1f : 1f;
        else downward = 0;
        // Calculating new position of knife.
        CalculateNewPosition();
    }

    /// <summary>
    /// Calculation of a new knife position based on the set value of the "downward" variable.
    /// </summary>
    private void CalculateNewPosition()
    {
        // Calculation of the new knife position
        Vector3 pos = transform.position + (new Vector3(downward * downSpeed, moveSpeed * (goingUp ? 1f : -1f), 0f) * Time.deltaTime);
        // Clamping new position with X axis boundaries.
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        transform.position = pos;
    }
}
