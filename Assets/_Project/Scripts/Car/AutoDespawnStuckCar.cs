using UnityEngine;

/// <summary>
/// AutoDespawnStuckCar.cs (opsional - checklist 5, poin "mobil stuck > X detik -> auto-despawn")
///
/// Tempel di prefab mobil yang sama (sebelah CarAI). Ini "jaring pengaman" TERAKHIR
/// buat kasus ekstrem: kalau karena bug lain mobil somehow stuck permanen dan nggak
/// pernah nyampe ExitSouth, komponen ini yang bakal beresin biar nggak numpuk selamanya.
///
/// Catatan: ini BUKAN pengganti fix di 4.1/4.2, cuma lapisan pengaman tambahan.
/// Kalau nggak mau efek "mobil ngilang tiba-tiba" dari sudut pandang pemain,
/// jangan pasang komponen ini dulu.
/// </summary>
public class AutoDespawnStuckCar : MonoBehaviour
{
    [Tooltip("Berapa detik mobil boleh diam total sebelum dianggap 'stuck' dan di-despawn.")]
    [SerializeField] private float stuckThresholdSeconds = 30f;
    [Tooltip("Toleransi pergerakan minimum per detik biar nggak salah anggap 'diam' gara-gara jitter kecil.")]
    [SerializeField] private float minMoveDistance = 0.05f;

    private Vector3 _lastPosition;
    private float _stuckTimer;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        float movedThisFrame = Vector3.Distance(transform.position, _lastPosition);

        if (movedThisFrame < minMoveDistance)
        {
            _stuckTimer += Time.deltaTime;
        }
        else
        {
            _stuckTimer = 0f;
        }

        _lastPosition = transform.position;

        if (_stuckTimer >= stuckThresholdSeconds)
        {
            Destroy(gameObject);
        }
    }
}