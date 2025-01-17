using UnityEngine;

public class MissileBehavoiur : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float cooldown = 100f;
    private int damage = 100;
    public SphereCollider SC;
    private float distance;
    private void Start()
    {
        SC = GetComponent<SphereCollider>();
        SC.radius = 35f;
    }
    void OnCollisionEnter(Collision collision) //when bullet hits something
    {

        BaseSoldier soldier = collision.gameObject.GetComponent<BaseSoldier>();
        if (soldier != null)
        {
            soldier.TakeDamage(damage);
        }
        
    }
    public void impactZone()
    {
        if (explosionPrefab != null) //Spawn explosion at missiles position
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Animator animator = explosionInstance.GetComponent<Animator>();
            if(animator != null)
            {
                float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
                Destroy(explosionInstance, animationLength);//Destroy if animation has ended

            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, SC.radius); //get collider within the sphere collider

        foreach (Collider nearbyCollider in colliders) 
        { 
            BaseSoldier nearbySoldier = nearbyCollider.GetComponent<BaseSoldier>();
            if (nearbySoldier != null) 
            { 
            nearbySoldier.TakeDamage(damage);
            
            }
        }
        Debug.Log("Impact zone triggered");
    }

}