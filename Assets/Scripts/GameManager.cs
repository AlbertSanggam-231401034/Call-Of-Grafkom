using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int enemyCount = 0;

    void Awake()
    {
        instance = this;
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(20, 30, 200, 30 ),
            "Enemies Left: " + enemyCount, style);
    }

    // Dipanggil musuh saat hidup
    public void RegisterEnemy()
    {
        enemyCount++;
        Debug.Log("Enemy registered. Total: " + enemyCount);
    }

    // Dipanggil musuh saat mati
    public void EnemyKilled()
    {
        enemyCount--;
        Debug.Log("Enemy killed. Remaining: " + enemyCount);
    }

    public bool AllEnemiesDead()
    {
        return enemyCount <= 0;
    }

    public void GameOver()
    {
        Invoke("Restart", 2f);
    }

    public void WinGame()
    {
        Debug.Log("YOU WIN!");
        Invoke("Restart", 2f);
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
