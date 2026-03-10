using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GravityChanger : MonoBehaviour
{
    [SerializeField] private PlayerController.EDirection newGravityDirection = PlayerController.EDirection.down;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController controller))
        {
            controller.ChangeGravityDirection(newGravityDirection);
        }
    }
}
