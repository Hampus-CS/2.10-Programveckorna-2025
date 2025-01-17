using UnityEngine;

[ExecuteAlways]
public class DebugVisualizer : MonoBehaviour
{
    /*[Header("Ownership Colors")]
    public Color PlayerOwnedColor = Color.green;
    public Color EnemyOwnedColor = Color.red;
    public Color ContestedColor = Color.yellow;
    public Color NeutralColor = Color.gray;

    [Header("Slot Colors")]
    public Color FreeSlotColor = Color.cyan;
    public Color OccupiedSlotColor = Color.magenta;

    [Header("Visualization Settings")]
    public bool ShowLineOwnership = true;
    public bool ShowSlotStatus = true;

    private LineManager lineManager;

    private void Awake()
    {
        lineManager = GetComponent<LineManager>();
        if (lineManager == null)
        {
            Debug.LogError($"DebugVisualizer requires a LineManager on {name}!");
        }
    }

    private void OnDrawGizmos()
    {
        if (lineManager == null) return;

        // Draw line ownership state
        if (ShowLineOwnership)
        {
            DrawLineOwnership();
        }

        // Draw slot statuses
        if (ShowSlotStatus)
        {
            DrawSlotStatuses();
        }
    }

    private void DrawLineOwnership()
    {
        // Determine color based on ownership state
        Gizmos.color = lineManager.CurrentState switch
        {
            LineManager.LineState.PlayerOwned => PlayerOwnedColor,
            LineManager.LineState.EnemyOwned => EnemyOwnedColor,
            LineManager.LineState.Contested => ContestedColor,
            _ => NeutralColor
        };

        // Draw a wire cube to represent the line
        Gizmos.DrawWireCube(transform.position, new Vector3(10, 1, 1)); // Adjust dimensions as needed
    }

    private void DrawSlotStatuses()
    {
        if (lineManager.slots == null || lineManager.slots.Length == 0) return;

        foreach (var slot in lineManager.slots)
        {
            if (slot == null) continue;

            // Get slot status from SlotManager
            var slotManager = slot.GetComponent<SlotManager>();
            if (slotManager == null) continue;

            Gizmos.color = slotManager.IsFree() ? FreeSlotColor : OccupiedSlotColor;

            // Draw a sphere to represent the slot
            Gizmos.DrawSphere(slot.position, 0.3f);
        }
    }*/
}

/// <summary>
/// Key Features
///   
///     Line Ownership Visualization:
///         - Displays a color-coded wireframe box around each line based on ownership state.
///         - Ownership states: PlayerOwned, EnemyOwned, Contested, Neutral.
///         
///     Slot Status Visualization:
///         - Highlights each slot with a color:
///             - Cyan: Slot is free.
///             - Magenta: Slot is occupied.
///         
///     Toggle Debug Options:
///         - Enable or disable line ownership and slot status visualization independently via Inspector toggles.
///         
///     Dynamic Feedback:
///         - Automatically updates visuals in both play and edit modes (ExecuteAlways).
/// </summary>

// How to Use
// 1. Attach DebugVisualizer.cs to each LineManager GameObject in your scene.
// 2. Customize the colors in the Inspector as needed.
// 3. Toggle ShowLineOwnership and ShowSlotStatus to enable or disable specific visual aids.