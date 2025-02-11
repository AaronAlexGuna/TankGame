using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public int damage = 1;
    public float bulletSpeed = 20f;
    public float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        PTController tank = collision.gameObject.GetComponent<PTController>();
        if (tank != null)
        {
            tank.TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   

}
