using UnityEngine;

/// <summary>
/// Core data component for a flower entity in the FlowerWhisper simulation.
/// Attach to any flower GameObject to give it identity, growth, and environmental stats.
/// All fields are serialized so they appear in the Unity Inspector.
///
/// Growth system: Every <see cref="updateInterval"/> seconds the flower is evaluated.
/// If water > 50 AND sunlight > 50, there is a random chance to advance growthStage.
/// Otherwise, health decreases over time.
/// </summary>
public class FlowerController : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Unique identifier for this flower instance.")]
    public string id = "";

    [Tooltip("The botanical species of this flower (e.g. Rose, Sunflower, Tulip).")]
    public string species = "Unknown";

    [Header("Growth")]
    [Tooltip("Current growth stage from 0 (seed) to 5 (fully bloomed).")]
    [Range(0, 5)]
    public int growthStage = 0;

    [Header("Vitals")]
    [Tooltip("Overall health of the flower. 0 = dead, 100 = perfect health.")]
    [Range(0f, 100f)]
    public float health = 100f;

    [Tooltip("Current water level. 0 = completely dry, 100 = fully watered.")]
    [Range(0f, 100f)]
    public float water = 50f;

    [Tooltip("Current sunlight exposure. 0 = no light, 100 = full sun.")]
    [Range(0f, 100f)]
    public float sunlight = 50f;

    [Header("Growth System Settings")]
    [Tooltip("Seconds between each growth evaluation tick.")]
    public float updateInterval = 10f;

    [Tooltip("Probability (0-1) of advancing a growth stage when conditions are met.")]
    [Range(0f, 1f)]
    public float growthChance = 0.4f;

    [Tooltip("Health points lost per tick when conditions are NOT met.")]
    public float healthDecayRate = 5f;

    /// <summary>
    /// Accumulated time since the last growth tick.
    /// Not serialized — resets each play session.
    /// </summary>
    private float timer;

    private void Awake()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer -= updateInterval;
            EvaluateGrowth();
        }
    }

    /// <summary>
    /// Runs once every <see cref="updateInterval"/> seconds.
    /// Checks water and sunlight conditions to decide whether the
    /// flower grows or loses health, then clamps all values.
    /// </summary>
    private void EvaluateGrowth()
    {
        if (water > 50f && sunlight > 50f)
        {
            TryGrow();
        }
        else
        {
            DecayHealth();
        }

        ClampValues();
    }

    private void TryGrow()
    {
        if (growthStage >= 5)
        {
            Debug.Log($"[FlowerWhisper] {species} ({id}) is fully bloomed — no further growth.");
            return;
        }

        float roll = Random.value;
        if (roll <= growthChance)
        {
            growthStage++;
            Debug.Log($"[FlowerWhisper] {species} ({id}) grew to stage {growthStage}! (roll: {roll:F2})");
        }
        else
        {
            Debug.Log($"[FlowerWhisper] {species} ({id}) conditions are good but did not grow this tick. (roll: {roll:F2})");
        }
    }

    private void DecayHealth()
    {
        health -= healthDecayRate;
        Debug.Log($"[FlowerWhisper] {species} ({id}) health decreased to {health:F1} (water: {water:F1}, sunlight: {sunlight:F1})");
    }

    private void ClampValues()
    {
        growthStage = Mathf.Clamp(growthStage, 0, 5);
        health = Mathf.Clamp(health, 0f, 100f);
        water = Mathf.Clamp(water, 0f, 100f);
        sunlight = Mathf.Clamp(sunlight, 0f, 100f);
    }
}
