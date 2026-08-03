using UnityEngine;

public class CarDespawn : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        CarAI car = other.GetComponent<CarAI>();
        if (car == null) return;

        Destroy(other.gameObject);
    }
}