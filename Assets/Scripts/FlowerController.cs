using UnityEngine;

/// <summary>
/// Core data component for a flower entity in the FlowerWhisper simulation.
/// Attach to any flower GameObject to give it identity, growth, and environmental stats.
/// All fields are serialized so they appear in the Unity Inspector.
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

    private void Awake()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }
}
