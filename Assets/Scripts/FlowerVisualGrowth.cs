using UnityEngine;

/// <summary>
/// Smoothly updates the flower's visual appearance (scale and bloom color)
/// based on the current <see cref="FlowerController.growthStage"/>.
/// Attach to each flower root GameObject alongside FlowerController.
///
/// Stage-to-visual mapping:
///   Stage 0 (Seed)       → scale 0.3x, pale green bloom
///   Stage 1 (Sprout)     → scale 0.5x, light green bloom
///   Stage 2 (Young)      → scale 0.7x, yellow-green bloom
///   Stage 3 (Growing)    → scale 0.85x, yellow bloom
///   Stage 4 (Mature)     → scale 1.0x, orange bloom
///   Stage 5 (Full Bloom) → scale 1.2x, vibrant red/pink bloom
///
/// Transitions are smoothly interpolated using Lerp so changes are gradual,
/// not instant. The script reads growthStage each frame and drives toward the
/// target scale and color for that stage.
/// </summary>
public class FlowerVisualGrowth : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("Scale multipliers for each growth stage (0 through 5). Applied to the initial scale.")]
    public float[] stageScales = new float[] { 0.3f, 0.5f, 0.7f, 0.85f, 1.0f, 1.2f };

    [Header("Color Settings")]
    [Tooltip("Bloom colors for each growth stage (0 through 5).")]
    public Color[] stageColors = new Color[]
    {
        new Color(0.6f, 0.8f, 0.5f, 1f),   // Stage 0: pale green
        new Color(0.5f, 0.85f, 0.4f, 1f),   // Stage 1: light green
        new Color(0.7f, 0.85f, 0.3f, 1f),   // Stage 2: yellow-green
        new Color(0.95f, 0.85f, 0.2f, 1f),  // Stage 3: yellow
        new Color(1.0f, 0.6f, 0.2f, 1f),    // Stage 4: orange
        new Color(1.0f, 0.3f, 0.4f, 1f)     // Stage 5: vibrant pink/red
    };

    [Header("Transition")]
    [Tooltip("How fast the visual transitions to the target (higher = faster).")]
    [Min(0.1f)]
    public float transitionSpeed = 3f;

    [Tooltip("Name of the child object whose Renderer color will be changed (the bloom).")]
    public string bloomChildName = "Bloom";

    private FlowerController flower;
    private Vector3 initialScale;
    private Renderer bloomRenderer;
    private MaterialPropertyBlock propertyBlock;
    private int lastStage = -1;
    private Vector3 targetScale;
    private Color targetColor;

    private void Start()
    {
        flower = GetComponent<FlowerController>();

        if (flower == null)
        {
            Debug.LogError($"[FlowerWhisper] FlowerVisualGrowth on {gameObject.name} requires a FlowerController component.");
            enabled = false;
            return;
        }

        initialScale = transform.localScale;

        Transform bloomTransform = transform.Find(bloomChildName);
        if (bloomTransform != null)
        {
            bloomRenderer = bloomTransform.GetComponent<Renderer>();
        }

        if (bloomRenderer == null)
        {
            Debug.LogWarning($"[FlowerWhisper] FlowerVisualGrowth on {gameObject.name}: no Renderer found on child '{bloomChildName}'. Color changes will be skipped.");
        }

        propertyBlock = new MaterialPropertyBlock();

        ApplyStageVisuals(flower.growthStage, immediate: true);
    }

    private void Update()
    {
        if (flower == null)
            return;

        int stage = Mathf.Clamp(flower.growthStage, 0, 5);

        if (stage != lastStage)
        {
            ApplyStageVisuals(stage, immediate: false);
        }

        SmoothTransition();
    }

    /// <summary>
    /// Sets the target scale and color for the given growth stage.
    /// If immediate is true, snaps to the target instantly (used on Start).
    /// </summary>
    private void ApplyStageVisuals(int stage, bool immediate)
    {
        lastStage = stage;

        float scaleMult = (stage >= 0 && stage < stageScales.Length) ? stageScales[stage] : 1f;
        targetScale = initialScale * scaleMult;

        targetColor = (stage >= 0 && stage < stageColors.Length) ? stageColors[stage] : Color.white;

        if (immediate)
        {
            transform.localScale = targetScale;
            ApplyColor(targetColor);
        }

        Debug.Log($"[FlowerWhisper] {flower.species} ({flower.id}) visual updated to stage {stage} — scale {scaleMult:F2}x");
    }

    /// <summary>
    /// Smoothly interpolates current scale and color toward targets each frame.
    /// </summary>
    private void SmoothTransition()
    {
        float lerpFactor = 1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);

        // Smooth scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpFactor);

        // Smooth color
        if (bloomRenderer != null)
        {
            bloomRenderer.GetPropertyBlock(propertyBlock);
            Color currentColor = propertyBlock.GetColor("_Color");

            if (currentColor == default)
                currentColor = targetColor;

            Color smoothedColor = Color.Lerp(currentColor, targetColor, lerpFactor);
            ApplyColor(smoothedColor);
        }
    }

    /// <summary>
    /// Applies a color to the bloom renderer via MaterialPropertyBlock
    /// to avoid creating material instances.
    /// </summary>
    private void ApplyColor(Color color)
    {
        if (bloomRenderer == null)
            return;

        bloomRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        propertyBlock.SetColor("_BaseColor", color);
        bloomRenderer.SetPropertyBlock(propertyBlock);
    }
}
