using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class GrenadeThrower : MonoBehaviour
{
    public float throwForce = 20f;
    public GameObject grenadePrefab;
    public int grenadeAmount;
    PhotonView pv;
    // Start is called before the first frame update
    void Start()
    {
        //pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.gameIsPaused)
        {
            if (grenadeAmount > 0 && Input.GetMouseButtonDown(2))
            {
                grenadeAmount--;
                ThrowGrenade();
                if (grenadeAmount <= 0)
                {
                    grenadeAmount = 0;
                }
            }
        }
    }

    void ThrowGrenade()
    {
        GameObject grenade = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "SkuffedGrenade"), transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
    }
}
