using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Monitors runtime FPS and applies simple quality fallbacks when performance drops.
/// </summary>
public class PerformanceOptimizer : MonoBehaviour
{
    [Header("FPS Monitor")]
    [Tooltip("Target minimum FPS. Optimizations are applied if measured FPS is below this value.")]
    public float minFps = 50f;

    [Tooltip("How often (seconds) to re-evaluate FPS.")]
    [Min(0.1f)]
    public float sampleInterval = 0.5f;

    [Header("Adaptive Quality")]
    [Tooltip("Seconds to wait between two quality reduction steps.")]
    [Min(0.1f)]
    public float optimizeCooldown = 2f;

    [Header("UI (Optional)")]
    [Tooltip("If enabled, create a small FPS text in the top-right corner.")]
    public bool showFpsText = true;

    public Text fpsText;

    private float elapsed;
    private int frameCount;
    private float measuredFps;
    private float cooldownTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (FindObjectOfType<PerformanceOptimizer>() != null)
            return;

        GameObject go = new GameObject("PerformanceOptimizer");
        go.AddComponent<PerformanceOptimizer>();
    }

    private void Start()
    {
        EnableGpuInstancingIfPossible();

        if (showFpsText && fpsText == null)
        {
            CreateFpsUI();
        }
    }

    private void Update()
    {
        frameCount++;
        elapsed += Time.unscaledDeltaTime;
        cooldownTimer += Time.unscaledDeltaTime;

        if (elapsed < sampleInterval)
            return;

        measuredFps = frameCount / elapsed;
        frameCount = 0;
        elapsed = 0f;

        UpdateFpsText();

        if (measuredFps < minFps)
        {
            ApplyPerformanceFallbacks();
        }
    }

    private void ApplyPerformanceFallbacks()
    {
        // Apply heavy switch immediately once: disable realtime shadows.
        if (QualitySettings.shadows != ShadowQuality.Disable)
        {
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0f;
            Debug.Log("[FlowerWhisper] PerformanceOptimizer: FPS low, shadows disabled.");
        }

        // Then step down quality slowly to avoid over-correction in one frame.
        if (cooldownTimer >= optimizeCooldown && QualitySettings.GetQualityLevel() > 0)
        {
            cooldownTimer = 0f;
            QualitySettings.DecreaseLevel(true);
            Debug.Log($"[FlowerWhisper] PerformanceOptimizer: quality reduced to '{QualitySettings.names[QualitySettings.GetQualityLevel()]}' (FPS {measuredFps:F1}).");
        }
    }

    private void EnableGpuInstancingIfPossible()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>(true);
        int enabledCount = 0;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].sharedMaterials;
            for (int j = 0; j < mats.Length; j++)
            {
                Material mat = mats[j];
                if (mat == null || mat.enableInstancing)
                    continue;

                mat.enableInstancing = true;
                enabledCount++;
            }
        }

        Debug.Log($"[FlowerWhisper] PerformanceOptimizer: GPU instancing enabled on {enabledCount} material(s) when supported.");
    }

    private void UpdateFpsText()
    {
        if (fpsText == null)
            return;

        fpsText.text = $"FPS: {measuredFps:F1}\nQuality: {QualitySettings.names[QualitySettings.GetQualityLevel()]}\nShadows: {QualitySettings.shadows}";
        fpsText.color = measuredFps < minFps ? Color.yellow : Color.white;
    }

    private void CreateFpsUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("PerformanceCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject textGO = new GameObject("FpsText");
        textGO.transform.SetParent(canvas.transform, false);

        fpsText = textGO.AddComponent<Text>();
        fpsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        fpsText.fontSize = 20;
        fpsText.alignment = TextAnchor.UpperRight;
        fpsText.color = Color.white;

        RectTransform rect = fpsText.rectTransform;
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(360f, 120f);
    }
}
