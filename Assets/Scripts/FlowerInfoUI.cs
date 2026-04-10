using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple runtime UI that shows flower water and growth stage in real time.
/// If no UI references are assigned, a Canvas + Text is created automatically.
/// </summary>
public class FlowerInfoUI : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("Optional explicit interaction manager reference. If null, one is searched in scene.")]
    public FlowerInteraction interaction;

    [Tooltip("Fallback flower shown when no flower is selected.")]
    public FlowerController fallbackFlower;

    [Header("UI")]
    [Tooltip("Legacy UI Text used to display info. Auto-created if left empty.")]
    public Text infoText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (FindObjectOfType<FlowerInfoUI>() != null)
            return;

        GameObject go = new GameObject("FlowerInfoUI");
        go.AddComponent<FlowerInfoUI>();
    }

    private void Awake()
    {
        if (interaction == null)
        {
            interaction = FindObjectOfType<FlowerInteraction>();
        }

        if (fallbackFlower == null)
        {
            fallbackFlower = FindObjectOfType<FlowerController>();
        }

        if (infoText == null)
        {
            CreateDefaultUI();
        }
    }

    private void Update()
    {
        FlowerController target = GetTargetFlower();

        if (target == null)
        {
            infoText.text = "Water: -\nGrowth Stage: -";
            return;
        }

        infoText.text = $"Water: {target.water:F1}\nGrowth Stage: {target.growthStage}";
    }

    private FlowerController GetTargetFlower()
    {
        if (interaction != null && interaction.SelectedFlower != null)
        {
            return interaction.SelectedFlower;
        }

        return fallbackFlower;
    }

    private void CreateDefaultUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("FlowerInfoCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject textGO = new GameObject("FlowerInfoText");
        textGO.transform.SetParent(canvas.transform, false);

        infoText = textGO.AddComponent<Text>();
        infoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        infoText.fontSize = 28;
        infoText.alignment = TextAnchor.UpperLeft;
        infoText.color = Color.white;

        RectTransform rect = infoText.rectTransform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(20f, -20f);
        rect.sizeDelta = new Vector2(420f, 120f);
    }
}
