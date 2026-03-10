using UnityEngine;

[RequireComponent (typeof(Collider))]
public class RespawnVolume : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController controller))
        {
            controller.Respawn();
        }
    }
}
