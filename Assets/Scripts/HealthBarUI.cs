using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Position & Size")]
    public Vector2 position = new Vector2(10, 3); 
    public Vector2 size = new Vector2(200, 30);

    [Header("Animation")]
    public float animationSpeed = 5f; 

    [Header("Colors")]
    public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f); 
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    private Canvas canvas;
    private Image backgroundImage;
    private Image fillImage;
    private float targetScale = 1f; 
    private float currentScale = 1f; 

    void Start()
    {
        CreateHealthBar();
        currentHealth = maxHealth;
        targetScale = 1f;
        currentScale = 1f;
        UpdateHealthBar();
    }

    void Update()
    {
 
        if (Mathf.Abs(currentScale - targetScale) > 0.001f)
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * animationSpeed);

            if (fillImage != null)
            {
                fillImage.rectTransform.localScale = new Vector3(currentScale, 1, 1);
            }
        }
    }

    void CreateHealthBar()
    {
        // 1. Buat Canvas
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Buat Background (kotak hitam)
        GameObject bgObj = new GameObject("HealthBarBackground");
        bgObj.transform.SetParent(canvas.transform);
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;

        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 1); // Kiri-atas
        bgRect.anchorMax = new Vector2(0, 1);
        bgRect.pivot = new Vector2(0, 1);
        bgRect.anchoredPosition = position;
        bgRect.sizeDelta = size;

        // 3. Buat Fill (kotak hijau yang mengecil)
        GameObject fillObj = new GameObject("HealthBarFill");
        fillObj.transform.SetParent(bgObj.transform);
        fillImage = fillObj.AddComponent<Image>();
        fillImage.color = fullHealthColor;

        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(1, 1);
        fillRect.pivot = new Vector2(0, 0.5f); // Pivot kiri tengah (PENTING!)
        fillRect.offsetMin = new Vector2(5, 5); // Padding kiri-bawah
        fillRect.offsetMax = new Vector2(-5, -5); // Padding kanan-atas
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        SetHealth(currentHealth - damage);
    }

    void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            // Hitung persentase HP
            float healthPercent = (float)currentHealth / maxHealth;

            // Set target scale (akan di-animate smooth di Update)
            targetScale = healthPercent;

            // Ubah warna dari hijau ke merah
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
    }
}