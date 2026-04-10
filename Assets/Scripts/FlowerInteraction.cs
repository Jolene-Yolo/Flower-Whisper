using UnityEngine;

/// <summary>
/// Handles player interaction with flowers via mouse raycasting.
/// Attach to a single manager GameObject in the scene (e.g. "InteractionManager").
///
/// Raycast logic:
/// 1. On left-click, a ray is cast from the camera through the mouse position into the scene.
/// 2. If the ray hits a collider, we check the hit object and its parents for a FlowerController.
/// 3. If found, the flower is "selected" — a scale-pulse animation plays on it.
/// 4. While a flower is selected, pressing W adds water to that flower.
///
/// The animation is a simple ping-pong scale effect driven in Update().
/// </summary>
public class FlowerInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [Tooltip("Maximum distance for the interaction raycast.")]
    public float maxRayDistance = 100f;

    [Header("Water Settings")]
    [Tooltip("Amount of water added per W key press.")]
    public float waterAmount = 10f;

    [Header("Animation Settings")]
    [Tooltip("How much the flower scales up when clicked (multiplier).")]
    public float pulseScale = 1.3f;

    [Tooltip("Duration of the full pulse animation in seconds.")]
    [Min(0.01f)]
    public float pulseDuration = 0.3f;

    /// <summary>Currently selected flower, or null if none.</summary>
    private FlowerController selectedFlower;
    public FlowerController SelectedFlower => selectedFlower;

    /// <summary>Original scale of the selected flower before animation.</summary>
    private Vector3 originalScale;

    /// <summary>Timer tracking the pulse animation progress.</summary>
    private float pulseTimer;

    /// <summary>Whether a pulse animation is currently playing.</summary>
    private bool isPulsing;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleClick();
        HandleWaterInput();
        UpdatePulseAnimation();
    }

    /// <summary>
    /// On left mouse button down, cast a ray from the camera through the mouse
    /// position. If it hits a GameObject with (or parented to) a FlowerController,
    /// select that flower and start the pulse animation.
    /// </summary>
    private void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            FlowerController flower = hit.collider.GetComponentInParent<FlowerController>();

            if (flower != null)
            {
                SelectFlower(flower);
                Debug.Log($"[FlowerWhisper] Selected {flower.species} ({flower.id})");
            }
            else
            {
                DeselectFlower();
                Debug.Log("[FlowerWhisper] Clicked non-flower object — deselected.");
            }
        }
        else
        {
            DeselectFlower();
            Debug.Log("[FlowerWhisper] Clicked empty space — deselected.");
        }
    }

    /// <summary>
    /// While a flower is selected, pressing W increases its water level.
    /// </summary>
    private void HandleWaterInput()
    {
        if (selectedFlower == null)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedFlower.water = Mathf.Clamp(selectedFlower.water + waterAmount, 0f, 100f);
            Debug.Log($"[FlowerWhisper] Watered {selectedFlower.species} ({selectedFlower.id}) — water is now {selectedFlower.water:F1}");
        }
    }

    private void SelectFlower(FlowerController flower)
    {
        // Restore previous flower's scale and unpause its growth visuals
        if (isPulsing && selectedFlower != null)
        {
            selectedFlower.transform.localScale = originalScale;
            SetScalePaused(selectedFlower, false);
        }

        selectedFlower = flower;

        // Use the growth system's target scale as the base, so the pulse
        // is always relative to the correct growth-stage size.
        FlowerVisualGrowth visualGrowth = flower.GetComponent<FlowerVisualGrowth>();
        originalScale = (visualGrowth != null) ? visualGrowth.TargetScale : flower.transform.localScale;

        // Pause growth-scale writes so they don't fight the pulse animation
        SetScalePaused(flower, true);

        pulseTimer = 0f;
        isPulsing = true;
    }

    private void DeselectFlower()
    {
        if (selectedFlower != null && isPulsing)
        {
            selectedFlower.transform.localScale = originalScale;
            SetScalePaused(selectedFlower, false);
        }

        selectedFlower = null;
        isPulsing = false;
    }

    /// <summary>
    /// Drives a scale pulse: the flower scales up to pulseScale over the first
    /// half of pulseDuration, then back down to original scale over the second half.
    /// </summary>
    private void UpdatePulseAnimation()
    {
        if (!isPulsing || selectedFlower == null)
            return;

        pulseTimer += Time.deltaTime;

        float halfDuration = pulseDuration * 0.5f;

        if (pulseTimer < halfDuration)
        {
            // Scale up
            float t = pulseTimer / halfDuration;
            selectedFlower.transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, t);
        }
        else if (pulseTimer < pulseDuration)
        {
            // Scale back down
            float t = (pulseTimer - halfDuration) / halfDuration;
            selectedFlower.transform.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, t);
        }
        else
        {
            // Animation complete — restore scale and unpause growth visuals
            selectedFlower.transform.localScale = originalScale;
            SetScalePaused(selectedFlower, false);
            isPulsing = false;
        }
    }

    /// <summary>
    /// Sets or clears the scalePaused flag on the flower's FlowerVisualGrowth
    /// component (if present) to prevent scale write conflicts during pulse.
    /// </summary>
    private void SetScalePaused(FlowerController flower, bool paused)
    {
        FlowerVisualGrowth visualGrowth = flower.GetComponent<FlowerVisualGrowth>();
        if (visualGrowth != null)
        {
            visualGrowth.scalePaused = paused;
        }
    }
}
