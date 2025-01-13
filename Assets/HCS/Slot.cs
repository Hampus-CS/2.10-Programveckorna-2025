using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Slot : MonoBehaviour
{
    public GameObject OccupyingSoldier { get; private set; } // Soldaten som tar upp slotten

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
            Debug.Log($"{soldier.name} released from slot at {transform.position}");
            OccupyingSoldier = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OccupyingSoldier == null && (other.CompareTag("FriendlyTroop") || other.CompareTag("HostileTroop")))
        {
            AssignSoldier(other.gameObject);

            // Registrera soldaten till linjen
            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.RegisterSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (OccupyingSoldier == other.gameObject)
        {
            ReleaseSoldier(other.gameObject);

            // Avregistrera soldaten från linjen
            var baseSoldier = other.GetComponent<BaseSoldier>();
            if (baseSoldier != null && baseSoldier.CurrentTargetLine != null)
            {
                baseSoldier.CurrentTargetLine.RemoveSoldier(other.gameObject, baseSoldier.IsPlayer);
            }
        }
    }

    public bool IsFree()
    {
        return OccupyingSoldier == null; // Returnera om slotten är ledig
    }

}