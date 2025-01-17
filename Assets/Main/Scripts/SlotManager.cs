using UnityEngine;

public class SlotManager : MonoBehaviour
{
    /*[Header("Slot Properties")]
    public GameObject OccupyingSoldier { get; private set; }
    public string OccupiedBy => OccupyingSoldier == null ? "none" : OccupyingSoldier.tag;

    [Header("Debug Settings")]
    public bool ShowGizmos = true; // Toggle Gizmos visibility
    public Color FreeColor = Color.green; // Slot is free
    public Color OccupiedColor = Color.red; // Slot is occupied

    [Header("Cover Settings")]
    public float OccupationTimeThreshold = 1f; // Time needed to mark as "fully occupied"
    private float currentOccupationTime = 0f;

    private void Update()
    {
        // If the slot is occupied, track the time the soldier has spent in it
        if (OccupyingSoldier != null)
        {
            currentOccupationTime += Time.deltaTime;

            // Ensure the occupying soldier is valid
            if (!OccupyingSoldier.activeSelf)
            {
                ReleaseSoldier();
            }
        }
        else
        {
            currentOccupationTime = 0f; // Reset if the slot is empty
        }
    }

    public bool IsFree()
    {
        return OccupyingSoldier == null;
    }

    public void AssignSoldier(GameObject soldier)
    {
        if (IsFree())
        {
            OccupyingSoldier = soldier;
            currentOccupationTime = 0f;
            Debug.Log($"{soldier.name} assigned to slot at {transform.position}");
        }
    }

    public void ReleaseSoldier()
    {
        if (OccupyingSoldier != null)
        {
            Debug.Log($"{OccupyingSoldier.name} released from slot at {transform.position}");
            OccupyingSoldier = null;
            currentOccupationTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only accept soldiers (either player or enemy) into the slot
        if (IsFree() && (other.CompareTag("FriendlyTroop") || other.CompareTag("HostileTroop")))
        {
            AssignSoldier(other.gameObject);

            // Notify the line about this soldier entering
            var soldier = other.GetComponent<BaseSoldier>();
            var lineManager = GetComponentInParent<LineManager>();
            if (soldier != null && lineManager != null)
            {
                lineManager.AddSoldier(other.gameObject, soldier.IsPlayer);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // When the soldier leaves the slot, release it
        if (OccupyingSoldier == other.gameObject)
        {
            var soldier = other.GetComponent<BaseSoldier>();
            var lineManager = GetComponentInParent<LineManager>();

            if (lineManager != null && soldier != null)
            {
                lineManager.RemoveSoldier(other.gameObject, soldier.IsPlayer);
            }

            ReleaseSoldier();
        }
    }

    private void OnDrawGizmos()
    {
        if (!ShowGizmos) return;

        Gizmos.color = IsFree() ? FreeColor : OccupiedColor;

        // Only log in play mode
        if (Application.isPlaying)
        {
            Debug.Log($"{name} is currently {(IsFree() ? "Free" : "Occupied")}");
        }

        Gizmos.DrawSphere(transform.position, 0.5f);
    }
    */

}

/// <summary>
/// Key Features
/// 
///     Slot Occupancy:
///         - Soldiers can occupy or vacate slots dynamically.
///         - Tracks which soldier is in the slot and how long they’ve been occupying it.
/// 
///     Integration with LineManager.cs:
///         - Automatically informs the parent LineManager when a soldier enters or leaves a slot.
/// 
///     Debugging Tools:
///         - Uses Gizmos to visualize free and occupied slots.
/// 
///     Smooth Soldier Management:
///         - Ensures soldiers leave and occupy slots correctly without stacking or conflicts.
/// </summary> 

// How to Use
// 1. Attach the SlotManager.cs script to all Slot GameObjects in your scene.
// 2. Ensure all Slot GameObjects are children of their respective Line GameObjects, so they correctly report to the parent LineManager.cs.