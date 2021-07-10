using UnityEngine;

class FieldColliderInBot : MonoBehaviour
{
    public LayerMask leadMask;
    private void OnTriggerEnter(Collider other)
    {
        if (((1<<other.gameObject.layer)&leadMask) != 0)
        {
            LeadController controller = other.gameObject.GetComponent<LeadController>();
            if (controller != null)
            {
                controller.LerpToDeath();
            }
        }
    }
}