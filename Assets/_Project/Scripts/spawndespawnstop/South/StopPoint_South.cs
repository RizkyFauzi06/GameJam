using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StopPoint_South : MonoBehaviour
{
    [SerializeField] private TrafficLight trafficLight;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.TryGetComponent(out CarAI car)) return;

        // Jika lampu merah/kuning, cek apakah mobil ini ada mobil lain di depannya.
        // Jika ada mobil di depannya, biarkan CarAI yang mengatur jarak (supaya ngantre rapi).
        // Jika tidak ada mobil di depannya (dia yang paling depan di garis stop), patuhi lampu!
        bool mustStop = trafficLight != null && !trafficLight.BolehJalan();
        
        car.SetStoppedByLight(mustStop);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out CarAI car))
        {
            car.SetStoppedByLight(false);

            // Sistem Near Miss (Disorder) jika nekat menerobos
            if (trafficLight != null && !trafficLight.BolehJalan())
            {
                car.ApplyNearMiss(); 
            }
        }
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        if (col is BoxCollider box)
        {
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}