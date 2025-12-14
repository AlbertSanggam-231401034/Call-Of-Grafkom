using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;
    private float nextFire = 0f;

    [Header("Ammo System")]
    public int maxAmmo = 30; // Peluru per magazine
    public int currentAmmo;
    public int reserveAmmo = 90; // Peluru cadangan
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("UI")]
    private AmmoUI ammoUI;

    // Referensi ke Kamera Utama untuk membidik
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        currentAmmo = maxAmmo;

        // Setup Ammo UI (otomatis dibuat!)
        GameObject ammoUIObj = new GameObject("PlayerAmmoUI");
        ammoUI = ammoUIObj.AddComponent<AmmoUI>();
        ammoUI.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);
    }

    void Update()
    {
        // DEBUG: Print status
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("KLIK! Ammo: " + currentAmmo + " | Reloading: " + isReloading);
        }

        // Reload dengan tombol R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartReload();
        }

        // Auto reload jika peluru habis
        if (currentAmmo <= 0 && !isReloading && reserveAmmo > 0)
        {
            StartReload();
        }

        // Klik Kiri Mouse
        if (Input.GetButton("Fire1") && Time.time > nextFire && !isReloading)
        {
            if (currentAmmo > 0)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
            else
            {
                // Bunyi "klik" kosong (opsional)
                Debug.Log("Out of Ammo! Press R to Reload");
            }
        }
    }

    void Shoot()
    {
        if (firePoint == null) return;

        // Kurangi ammo
        currentAmmo--;
        ammoUI.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);

        // 1. Cari titik tengah layar
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        // 2. Cek apakah crosshair menabrak sesuatu (Enemy/Tembok)
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point; // Tembak ke titik tabrakan
        }
        else
        {
            targetPoint = ray.GetPoint(75); // Jika membidik langit, tembak jauh ke depan (jarak 75)
        }

        // 3. Hitung arah dari moncong senjata ke titik target
        Vector3 directionWithoutSpread = targetPoint - firePoint.position;

        // 4. Hitung rotasi peluru agar menghadap target
        Quaternion rotation = Quaternion.LookRotation(directionWithoutSpread);

        // 5. Spawn Peluru dengan rotasi yang sudah dikoreksi
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, rotation);

        // --- Opsional: Agar tidak menabrak player sendiri ---
        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider playerCollider = GetComponent<Collider>();
        if (bulletCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(bulletCollider, playerCollider);
        }
        // ----------------------------------------------------

        // Setup Peluru Player
        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.shooterTag = "Player";
        }
    }

    void StartReload()
    {
        if (reserveAmmo <= 0)
        {
            Debug.Log("No Reserve Ammo!");
            return;
        }

        isReloading = true;
        ammoUI.ShowReloadText(true);
        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        // Hitung berapa peluru yang dibutuhkan
        int ammoNeeded = maxAmmo - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, reserveAmmo);

        // Isi magazine
        currentAmmo += ammoToReload;
        reserveAmmo -= ammoToReload;

        isReloading = false;
        ammoUI.ShowReloadText(false);
        ammoUI.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);

        Debug.Log("Reload Complete!");
    }

    // Fungsi untuk menambah ammo (misal dari pickup)
    public void AddAmmo(int amount)
    {
        reserveAmmo += amount;
        ammoUI.UpdateAmmo(currentAmmo, maxAmmo, reserveAmmo);
    }
}