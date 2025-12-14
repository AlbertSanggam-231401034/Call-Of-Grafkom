using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 5;
    private int currentHP;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    [Header("Components")]
    public Transform cameraTransform;
    private Rigidbody rb;

    [Header("UI")]
    private HealthBarUI healthBarUI;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentHP = maxHP;
        Cursor.lockState = CursorLockMode.Locked;

        // --- TAMBAHAN BARU ---
        // Otomatis cari kamera utama di scene
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            cameraTransform = mainCam.transform;
            // Beritahu kamera untuk mengikuti saya (Player ini)
            mainCam.GetComponent<ThirdPersonCamera>().player = this.transform;
        }
        // ---------------------

        // Setup health bar UI (OTOMATIS DIBUAT!)
        GameObject healthBarObj = new GameObject("PlayerHealthBar");
        healthBarUI = healthBarObj.AddComponent<HealthBarUI>();
        healthBarUI.maxHealth = maxHP;
    }

    void Update()
    {
        Move();
        Crouch();

        // DEBUG: Tekan T untuk test damage
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(1);
            Debug.Log("TEST DAMAGE! HP sekarang: " + currentHP);
        }
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Arah relatif kamera
        Vector3 camFwd = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camFwd.y = 0;
        camRight.y = 0;
        Vector3 direction = (camFwd * v + camRight * h).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // 1. ROTASI MANUAL (Ganti Quaternion.Slerp bawaan jika ingin 100% manual, tapi Slerp biasanya dimaafkan)
            // Syarat minta manipulasi nilai transform. Kita manipulasi rotation langsung.
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // 2. TRANSLASI MANUAL (INI YANG PALING PENTING)
            // Rumus: Posisi Baru = Posisi Lama + (Arah * Kecepatan * Waktu)
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void Crouch()
    {
        // FIX BUG: Ganti dari LeftControl ke C untuk menghindari konflik dengan tombol shooting
        if (Input.GetKeyDown(KeyCode.C))
            transform.localScale = new Vector3(1, 0.5f, 1); // Jadi pendek

        if (Input.GetKeyUp(KeyCode.C))
            transform.localScale = new Vector3(1, 1, 1); // Kembali normal
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0); // Pastikan HP tidak negatif

        Debug.Log("Player HP: " + currentHP);

        // Update health bar UI
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHP);
        }

        if (currentHP <= 0)
        {
            Debug.Log("GAME OVER");
            // Panggil Game Over logic di GameManager nanti
            FindFirstObjectByType<GameManager>().GameOver();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            if (GameManager.instance.AllEnemiesDead())
            {
                GameManager.instance.WinGame();
            }
            else
            {
                Debug.Log("Masih ada musuh! Kalahkan semua musuh dulu!");
            }
        }
    }

}