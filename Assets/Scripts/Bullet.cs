using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public string shooterTag; // Diisi "Player" atau "Enemy" saat spawn

    //private Rigidbody rb;

    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        //rb.linearVelocity = transform.forward * speed; // Bergerak lurus ke depan
        Destroy(gameObject, 5f); // Hapus otomatis setelah 5 detik agar tidak menumpuk
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Jangan hancur jika mengenai penembak sendiri
        if (collision.gameObject.CompareTag(shooterTag)) return;

        // Cek jika mengenai Player atau Enemy
        if (collision.gameObject.CompareTag("Player"))
        {
            // Panggil fungsi damage di script player (nanti dibuat)
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            // Panggil fungsi damage di script enemy
            EnemyController enemyController = collision.gameObject.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);
            }
        }

        // Hancur saat menabrak tembok atau target
        Destroy(gameObject);
    }
}