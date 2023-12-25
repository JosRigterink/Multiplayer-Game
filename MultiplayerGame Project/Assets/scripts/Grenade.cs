using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Grenade : MonoBehaviour
{
    public float delay = 3f;
    public float radius = 5f;
    public float force = 700f;
    public float damage = 80f;

    float countdown;
    bool hasExploded = false;
    PhotonView pv;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        if (!pv.IsMine)
        {
            Destroy(rb);
        }
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    void Explode()
    {
        //showing some explotion effect
        //gets nearby objects
        Collider[] colliders =  Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
           Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                //addsforce
                rb.AddExplosionForce(force, transform.position, radius);
            }
            //damage code here

            if (nearbyObject.GetComponent<IDamageable>() != null)
            {
                nearbyObject.GetComponent<IDamageable>().TakeDamage(damage);
            }
        }
        //remove grenade
        Destroy(gameObject);
    }
}
