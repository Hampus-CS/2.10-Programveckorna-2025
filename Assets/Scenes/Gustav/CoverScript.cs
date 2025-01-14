using UnityEngine.AI;
using UnityEngine;

public class CoverScript : MonoBehaviour
{
    public string occupiedBy = "none";
    private int troopsInCollider = 0;
    private float presenceTime = 0f;
    private float requiredPresenceTime = 1f; // Time needed to count as occupied

    private BoxCollider collider;

    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, collider.size / 2, Quaternion.identity);
        troopsInCollider = 0;

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<NavMeshAgent>())
            {
                troopsInCollider++;
                if (occupiedBy == "none" || occupiedBy == col.name)
                {
                    presenceTime += Time.deltaTime;

                    if (presenceTime >= requiredPresenceTime)
                    {
                        occupiedBy = col.name; // Mark as occupied
                    }
                }
            }
        }

        if (troopsInCollider == 0)
        {
            presenceTime = 0f; // Reset timer
            occupiedBy = "none"; // No troop present
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Troop"))
        {
            troopsInCollider++;
            occupiedBy = other.gameObject.tag; // Adjust based on troop ownership logic
            Debug.Log($"{other.name} entered {gameObject.name}, now occupied by {occupiedBy}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Troop"))
        {
            troopsInCollider--;
            if (troopsInCollider <= 0)
            {
                occupiedBy = "none";
            }
            Debug.Log($"{other.name} exited {gameObject.name}, now occupied by {occupiedBy}");
        }
    }
}