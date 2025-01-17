using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider; // Reference to the UI Slider
    public Transform target; // Target to follow (e.g., soldier's position)

    [Header("Offset Settings")]
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset to position the health bar above the target

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (healthSlider == null)
        {
            Debug.LogError("HealthSlider is not assigned!");
        }

        if (target == null)
        {
            Debug.LogError("Target is not assigned for HealthBarUI!");
        }
    }

    private void LateUpdate()
    {
        // Keep the health bar above the target
        if (target != null)
        {
            transform.position = target.position + offset;

            // Always face the camera
            if (mainCamera != null)
            {
                transform.LookAt(mainCamera.transform);
                transform.Rotate(0, 180, 0); // Flip the bar to face the camera properly
            }
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}

// How to Use
//      Prefab Setup - To use the health bar system:
//
//          Create a Health Bar Prefab:
//              - Create a new Canvas in Unity and set it to World Space.
//              - Add a Slider to the Canvas and style it as a health bar (e.g., red for health).
//              - Set the Slider’s range to 0–1 and assign a child Image for the fill area.
//              - Save the setup as a prefab (e.g., HealthBarPrefab).
//
//          Assign the Prefab:
//              - In the BaseSoldier.cs script, there is already included logic to instantiate a health bar prefab. Assign your prefab to the HealthBarPrefab field in the inspector.