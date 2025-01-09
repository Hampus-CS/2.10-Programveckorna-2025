using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

// Manages the progression of time and adjusts lighting to simulate day-night cycles.
public class TimeManager : MonoBehaviour
{
    // Time Properties
    protected float dayDuration = 60f; // Duration of a full day in seconds
    protected float currentTime;
    protected float dayStart = 6f; // Start of the day in hours
    protected float dayEnd = 24f; // End of the day in hours
    public bool isTimeFrozen = false; // Flag to freeze time progression
    public TMP_Text timeText;

    // Lighting
    public Light2D globalLight;

    protected void Start()
    {
        if (dayDuration <= 0f) dayDuration = 60f;
        currentTime = dayStart;
        Debug.Log($"Time initialized to {currentTime} hours with duration {dayDuration} seconds.");
    }

    public void Update()
    {
        timeText.text = $"Time: {currentTime}";
    }

    // Advances the current time and checks if the day has ended.
    public bool AdvanceTime()
    {
        if (isTimeFrozen) return false;

        float timeIncrement = (dayEnd - dayStart) / dayDuration * Time.deltaTime;
        currentTime += timeIncrement;

        if (currentTime >= dayEnd)
        {
            currentTime = dayEnd;
            return true; // Day has ended
        }

        return false; // Day is still ongoing
    }

    // Updates the lighting intensity and color based on the time of day.
    public void UpdateLighting()
    {
        float normalizedTime = (currentTime - dayStart) / (dayEnd - dayStart);

        // Adjust light intensity
        globalLight.intensity = Mathf.Lerp(0.2f, 1f, Mathf.Sin(normalizedTime * Mathf.PI));

        // Adjust light color
        if (normalizedTime < 0.5f)
        {
            globalLight.color = Color.Lerp(new Color(1f, 0.5f, 0.5f), Color.white, normalizedTime * 2f);
        }
        else
        {
            globalLight.color = Color.Lerp(Color.white, new Color(1f, 0.5f, 0.5f), (normalizedTime - 0.5f) * 2f);
        }
    }

    // Resets the time to the start of the day.
    public void ResetTime()
    {
        currentTime = dayStart;
        Debug.Log($"Time reset to {currentTime} hours.");
    }

    // Gets the current time.
    public float GetCurrentTime()
    {
        return currentTime;
    }

    // Sets the current time (used during loading or debugging).
    public void SetCurrentTime(float time)
    {
        currentTime = time;
        Debug.Log($"Current time set to {currentTime} hours.");
    }
}