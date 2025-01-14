using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessing : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            PostProcessVolume ppVolume = Camera.main.gameObject.GetComponent<PostProcessVolume>();
            ppVolume.enabled = !ppVolume.enabled;
            print("ppEnabled");
        }
    }
}
