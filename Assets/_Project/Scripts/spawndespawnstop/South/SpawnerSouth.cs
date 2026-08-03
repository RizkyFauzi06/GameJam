using UnityEngine;

public class SpawnerSouth : MonoBehaviour
{
    [Header("Spawn Setup")]
    // UPDATE: Ubah jadi Array agar bisa masukin banyak warna/tipe mobil Kenney
    [SerializeField] private GameObject[] carPrefabs; 
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2.5f;
    [SerializeField] private LayerMask carLayer;

    [Header("4.1 - Cek Kepadatan Sepanjang Lajur")]
    [SerializeField] private float checkRadius = 1.5f;
    [SerializeField] private float checkPointSpacing = 4f;
    [SerializeField] private int checkPointCount = 5;

    [Header("4.2 - Batas Maksimum Mobil Aktif")]
    [SerializeField] private int maxActiveCars = 40;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < spawnInterval) return;
        _timer = 0f;

        TrySpawn();
    }

    private void TrySpawn()
    {
        if (CarAI.ActiveCars.Count >= maxActiveCars) return;
        if (IsLaneOccupiedAhead()) return;
        if (carPrefabs.Length == 0) return; // Keamanan tambahan

        // UPDATE: Pilih mobil acak dari array
        int randomIndex = Random.Range(0, carPrefabs.Length);
        Instantiate(carPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
    }

    private bool IsLaneOccupiedAhead()
    {
        for (int i = 0; i < checkPointCount; i++)
        {
            Vector3 point = spawnPoint.position - spawnPoint.forward * (checkPointSpacing * i);
            if (Physics.CheckSphere(point, checkRadius, carLayer))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || spawnPoint == null) return;
        Gizmos.color = Color.cyan;
        for (int i = 0; i < checkPointCount; i++)
        {
            Vector3 point = spawnPoint.position - spawnPoint.forward * (checkPointSpacing * i);
            Gizmos.DrawWireSphere(point, checkRadius);
        }
    }
}