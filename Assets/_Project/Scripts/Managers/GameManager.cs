using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Pola Singleton agar bisa dipanggil dari mana saja menggunakan GameManager.Instance
    public static GameManager Instance { get; private set; }

    [Header("UI Settings")]
    public GameObject gameOverPanel; // Panel yang muncul saat Tawuran

    private void Awake()
    {
        // Setup Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Dipanggil oleh CarAI saat stress mencapai 100
    /// </summary>
    public void TriggerTawuran(CarAI triggerCar)
    {
        Debug.Log($"[GAME OVER] Tawuran pecah! Dipicu oleh mobil: {triggerCar.gameObject.name}");

        // Hentikan waktu permainan (mobil berhenti semua)
        Time.timeScale = 0f;

        // Tampilkan layar Game Over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Fungsi untuk tombol "Restart" di layar Game Over
    /// </summary>
    public void RestartGame()
    {
        // Kembalikan waktu berjalan normal
        Time.timeScale = 1f; 
        
        // Muat ulang scene yang sedang aktif
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}