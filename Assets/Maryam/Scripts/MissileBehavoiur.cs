using UnityEngine;

public class MissileBehavoiur : MonoBehaviour
{
    public GameObject Explosion;
    

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(Explosion);
    }
}
