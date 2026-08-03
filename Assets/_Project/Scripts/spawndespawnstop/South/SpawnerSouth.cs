using UnityEngine;

/// <summary>
/// SpawnerSouth.cs (revisi)
/// Fix untuk Masalah B:
///  - 4.2: maxActiveCars -> "rem darurat", spawn berhenti total kalau udah kepenuhan.
///         Pakai CarAI.ActiveCars (static list dari CarAI.cs) supaya hitungannya akurat
///         dan nggak perlu FindObjectsByType tiap kali cek (lebih murah performa).
///  - 4.1: areaOccupied sekarang dicek di BEBERAPA titik sepanjang lajur (bukan cuma
///         1 titik pas di spawnPoint), jadi spawner bisa "lihat" kalau macet udah
///         merambat jauh ke belakang dari titik spawn, bukan cuma macet PAS di situ.
///  - 4.3: (opsional, lihat AutoDespawnStuckCar.cs terpisah) mobil yang stuck kelamaan
///         di satu tempat bisa di-despawn otomatis -> mencegah numpuk permanen.
/// </summary>
public class SpawnerSouth : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2.5f;
    [SerializeField] private LayerMask carLayer;

    [Header("4.1 - Cek Kepadatan Sepanjang Lajur")]
    [Tooltip("Radius cek di tiap titik sepanjang lajur.")]
    [SerializeField] private float checkRadius = 1.5f;
    [Tooltip("Jarak antar titik cek, diukur mundur dari spawnPoint sepanjang -forward spawner.")]
    [SerializeField] private float checkPointSpacing = 4f;
    [Tooltip("Berapa titik yang dicek ke belakang dari spawn point. " +
             "Makin banyak, makin jauh spawner bisa 'lihat' antrean di belakang.")]
    [SerializeField] private int checkPointCount = 5;

    [Header("4.2 - Batas Maksimum Mobil Aktif")]
    [Tooltip("Kalau jumlah CarAI aktif di scene >= angka ini, spawn di-skip total, apapun kondisi jalan.")]
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
        // 4.2: rem darurat paling atas -> dicek duluan sebelum apapun, paling murah & paling tegas
        if (CarAI.ActiveCars.Count >= maxActiveCars)
        {
            return;
        }

        // 4.1: cek sepanjang lajur, bukan cuma 1 titik
        if (IsLaneOccupiedAhead())
        {
            return;
        }

        Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    /// <summary>
    /// Cek beberapa titik mundur dari spawnPoint sepanjang arah -forward (arah datangnya antrean).
    /// Kalau SALAH SATU titik itu ada mobil, anggap lajur masih padat -> skip spawn.
    /// </summary>
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