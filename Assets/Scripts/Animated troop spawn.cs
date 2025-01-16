using UnityEngine;

public class TroopSpawner : MonoBehaviour
{
    public GameObject[] troopPrefabs; // Array of troop prefab GameObjects
    public RuntimeAnimatorController[] troopControllers; // Array of AnimatorControllers matching troop types

    void SpawnTroop()
    {
        // Randomly select a troop type
        int randomIndex = Random.Range(0, troopPrefabs.Length);

        // Instantiate the troop
        GameObject spawnedTroop = Instantiate(troopPrefabs[randomIndex], transform.position, Quaternion.identity);

        // Get the Animator component of the spawned troop
        Animator animator = spawnedTroop.GetComponentInChildren<Animator>();

        // Assign the correct AnimatorController
        if (animator != null && randomIndex < troopControllers.Length)
        {
            animator.runtimeAnimatorController = troopControllers[randomIndex];
        }
    }
}