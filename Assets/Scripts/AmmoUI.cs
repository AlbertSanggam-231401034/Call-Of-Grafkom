using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [Header("Settings")]
    public Vector2 position = new Vector2(-20, 0); // Kanan bawah layar
    public int fontSize = 36;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color lowAmmoColor = Color.red;
    public Color reloadColor = Color.yellow;

    private Canvas canvas;
    private Text ammoText;
    private Text reloadText;

    void Start()
    {
        CreateAmmoUI();
    }

    void CreateAmmoUI()
    {
        // 1. Buat Canvas (atau gunakan yang sudah ada)
        Canvas existingCanvas = FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            canvas = existingCanvas;
        }
        else
        {
            GameObject canvasObj = new GameObject("AmmoCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 2. Buat Ammo Text (30 / 90)
        GameObject ammoObj = new GameObject("AmmoText");
        ammoObj.transform.SetParent(canvas.transform);
        ammoText = ammoObj.AddComponent<Text>();
        ammoText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ammoText.fontSize = fontSize;
        ammoText.color = normalColor;
        ammoText.alignment = TextAnchor.LowerRight;
        ammoText.text = "30 / 90";

        RectTransform ammoRect = ammoObj.GetComponent<RectTransform>();
        ammoRect.anchorMin = new Vector2(1, 0); // Kanan bawah
        ammoRect.anchorMax = new Vector2(1, 0);
        ammoRect.pivot = new Vector2(1, 0);
        ammoRect.anchoredPosition = position;
        ammoRect.sizeDelta = new Vector2(200, 50);

        // 3. Buat Reload Text (RELOADING...)
        GameObject reloadObj = new GameObject("ReloadText");
        reloadObj.transform.SetParent(canvas.transform);
        reloadText = reloadObj.AddComponent<Text>();
        reloadText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        reloadText.fontSize = fontSize - 8;
        reloadText.color = reloadColor;
        reloadText.alignment = TextAnchor.LowerRight;
        reloadText.text = "RELOADING...";

        RectTransform reloadRect = reloadObj.GetComponent<RectTransform>();
        reloadRect.anchorMin = new Vector2(1, 0);
        reloadRect.anchorMax = new Vector2(1, 0);
        reloadRect.pivot = new Vector2(1, 0);
        reloadRect.anchoredPosition = new Vector2(position.x, position.y + 60);
        reloadRect.sizeDelta = new Vector2(250, 40);

        reloadText.gameObject.SetActive(false); // Hidden by default
    }

    public void UpdateAmmo(int current, int max, int reserve)
    {
        if (ammoText != null)
        {
            ammoText.text = current + " / " + reserve;

            // Ubah warna jadi merah jika ammo tinggal sedikit
            float ammoPercent = (float)current / max;
            if (ammoPercent <= 0.3f) // 30% atau kurang
            {
                ammoText.color = lowAmmoColor;
            }
            else
            {
                ammoText.color = normalColor;
            }
        }
    }

    public void ShowReloadText(bool show)
    {
        if (reloadText != null)
        {
            reloadText.gameObject.SetActive(show);
        }
    }
}