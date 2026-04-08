using UnityEngine;

/// <summary>
/// Applies a smooth sine-based wind sway to the flower's transform.
/// Attach to each flower root GameObject.
///
/// How the animation works:
/// 1. On Start(), the flower's world X position is captured as a phase offset
///    so flowers at different positions sway out of sync — avoiding a uniform look.
/// 2. Each frame, a rotation offset is calculated:
///      angle = sin(Time.time * speed + positionOffset) * intensity
/// 3. This angle is applied as a Z-axis rotation around the flower's base,
///    creating a natural side-to-side tilting motion.
/// 4. A secondary, smaller X-axis sway adds depth so the motion isn't purely 2D.
/// 5. The original rotation is preserved and the sway is layered on top,
///    so the animation is purely additive and non-destructive.
/// </summary>
public class WindSway : MonoBehaviour
{
    [Header("Wind Settings")]
    [Tooltip("How far the flower tilts in degrees.")]
    public float intensity = 5f;

    [Tooltip("How fast the sway oscillates (cycles per second multiplier).")]
    public float speed = 1.5f;

    [Tooltip("Secondary sway intensity on the X axis for depth.")]
    public float secondaryIntensity = 2f;

    [Tooltip("Speed multiplier for the secondary sway (slightly offset for organic feel).")]
    public float secondarySpeed = 1.1f;

    /// <summary>Phase offset derived from world position so each flower sways differently.</summary>
    private float phaseOffset;

    /// <summary>The flower's initial local rotation, preserved so sway is additive.</summary>
    private Quaternion baseRotation;

    private void Start()
    {
        phaseOffset = transform.position.x + transform.position.z * 0.7f;
        baseRotation = transform.localRotation;
    }

    private void Update()
    {
        float primaryAngle = Mathf.Sin(Time.time * speed + phaseOffset) * intensity;
        float secondaryAngle = Mathf.Sin(Time.time * secondarySpeed + phaseOffset * 1.3f) * secondaryIntensity;

        Quaternion swayRotation = Quaternion.Euler(secondaryAngle, 0f, primaryAngle);
        transform.localRotation = baseRotation * swayRotation;
    }
}
