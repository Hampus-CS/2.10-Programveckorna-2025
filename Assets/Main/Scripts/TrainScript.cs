using UnityEngine;

public class TrainScript : MonoBehaviour
{
    public float health = 200;
    public GameObject loseScreen;
    public GameObject winScreen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, 0.003f);

        if (health <= 0)
        {
            loseScreen.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finnish"))
        {
            winScreen.SetActive(true);
        }
    }
}
