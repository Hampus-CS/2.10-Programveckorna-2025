using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Slot : MonoBehaviour
{
    public GameObject OccupyingSoldier { get; set; } // Soldaten som tar upp slotten

    public void AssignSoldier(GameObject soldier)
    {
        if (OccupyingSoldier == null)
        {
            OccupyingSoldier = soldier;
            Debug.Log($"{soldier.name} assigned to slot at {transform.position}");
        }
        else
        {
            Debug.LogWarning($"Slot at {transform.position} is already occupied by {OccupyingSoldier.name}. {soldier.name} cannot take it.");
        }
    }

    public void ReleaseSoldier(GameObject soldier)
    {
        if (OccupyingSoldier == soldier)
        {
            OccupyingSoldier = null;
            Debug.Log($"{soldier.name} released from slot at {transform.position}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OccupyingSoldier == null && (other.CompareTag("FriendlyTroop") || other.CompareTag("HostileTroop")))
        {
            AssignSoldier(other.gameObject);
            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.RegisterSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
        else if (OccupyingSoldier != null)
        {
            Debug.LogWarning($"{other.name} tried to occupy an already occupied slot at {transform.position}");
            // Signalera till soldaten att omdirigera
            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null)
            {
                baseSoldier.FindNewTargetSlotOrLine();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (OccupyingSoldier == other.gameObject)
        {
            ReleaseSoldier(other.gameObject);

            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.RemoveSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
    }

    public void VacateSlot()
    {
        if (OccupyingSoldier != null)
        {
            Debug.Log($"{OccupyingSoldier.name} vacated slot at {transform.position}");
            OccupyingSoldier = null;
        }
    }

    public bool IsFree()
    {
        return OccupyingSoldier == null; // Returnera om slotten är ledig
    }
}