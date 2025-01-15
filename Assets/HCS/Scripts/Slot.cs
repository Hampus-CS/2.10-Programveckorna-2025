using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject OccupyingSoldier { get; private set; }

    [Header("Debug Settings")]
    public bool ShowGizmos = true; // Toggle Gizmos visibility in the Inspector

    public bool IsFree()
    {
        return OccupyingSoldier == null;
    }

    public void AssignSoldier(GameObject soldier)
    {
        if (IsFree())
        {
            OccupyingSoldier = soldier;
            Debug.Log($"{soldier.name} assigned to slot at {transform.position}");
        }
    }

    public void ReleaseSoldier()
    {
        if (OccupyingSoldier != null)
        {
            Debug.Log($"{OccupyingSoldier.name} released from slot at {transform.position}");
            OccupyingSoldier = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos) return;

        Gizmos.color = IsFree() ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsFree() && (other.CompareTag("FriendlyTroop") || other.CompareTag("HostileTroop")))
        {
            AssignSoldier(other.gameObject);

            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.AddSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (OccupyingSoldier == other.gameObject)
        {
            ReleaseSoldier();

            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.RemoveSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
    }
}
