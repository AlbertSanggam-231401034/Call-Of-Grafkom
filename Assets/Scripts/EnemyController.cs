using UnityEngine;
using System.Collections; // PENTING: Tambah ini buat IEnumerator

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int hp = 3;
    public float rotationSpeed = 50f;
    public float detectionRange = 10f;

    [Header("Visuals")]
    public Renderer enemyRenderer; // DRAG BADAN MUSUH KE SINI DI INSPECTOR
    private Material enemyMat;
    private Color originalHitColor;

    [Header("Status")]
    public bool isLockedOnPlayer = false;
    public Transform playerTarget;

    private EnemyShooting shootingScript;

    void Start()
    {
        shootingScript = GetComponent<EnemyShooting>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        GameManager.instance.RegisterEnemy();

        // --- SETUP SHADER MUSUH ---
        if (enemyRenderer != null)
        {
            enemyMat = enemyRenderer.material;
            // Ambil warna default shader (biasanya hitam/transparan)
            if (enemyMat.HasProperty("_HitColor"))
            {
                originalHitColor = enemyMat.GetColor("_HitColor");
            }
        }
    }

    void Update()
    {
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // 1. Cek Jarak
        if (distance <= detectionRange)
        {
            CheckLineOfSight();
        }
        else
        {
            isLockedOnPlayer = false;
        }

        // 2. Perilaku
        if (isLockedOnPlayer)
        {
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            shootingScript.canShoot = true;
        }
        else
        {
            // Idle rotation manual (Euler manipulation)
            Vector3 currentRot = transform.eulerAngles;
            currentRot.y += rotationSpeed * Time.deltaTime;
            transform.eulerAngles = currentRot;

            shootingScript.canShoot = false;
        }
    }

    void CheckLineOfSight()
    {
        RaycastHit hit;
        Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player")) isLockedOnPlayer = true;
            else isLockedOnPlayer = false;
        }
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;

        // --- TRIGGER ANIMASI SHADER (Merah 0.2 Detik) ---
        StartCoroutine(FlashRed());

        if (hp <= 0)
        {
            // Skalasi Musuh Mengecil (Syarat Tugas)
            StartCoroutine(DieSequence());
        }
    }

    IEnumerator FlashRed()
    {
        if (enemyMat != null)
        {
            enemyMat.SetColor("_HitColor", Color.red); // Jadi Merah
            yield return new WaitForSeconds(0.2f); // Tunggu 0.2 detik
            enemyMat.SetColor("_HitColor", originalHitColor); // Balik Normal
        }
    }

    IEnumerator DieSequence()
    {
        // Matikan collider biar ga bisa ditembak lagi saat animasi mati
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;
        shootingScript.canShoot = false;

        // Animasi Mengecil (Skalasi Manual)
        float shrinkDuration = 0.5f;
        float timer = 0;
        Vector3 initialScale = transform.localScale;

        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timer / shrinkDuration);
            yield return null;
        }

        GameManager.instance.EnemyKilled();
        Destroy(gameObject);
    }
}