using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>();
        }
        if (cam == null)
        {
            return;
        }
        DetermineAim();
    }

    public override void Use()
    {
        Shoot();
        DetermineRecoil();
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if(Physics.Raycast(ray,out RaycastHit hit,((GunInfo)itemInfo).range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if(colliders.Length != 0)
        {
           GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 8f);
           bulletImpactObj.transform.SetParent(colliders[0].transform);
        } 
    }

    void DetermineAim()
    {
        if (PV.IsMine)
        {
            Vector3 target = (((GunInfo)itemInfo).normalLocalPosition);
            if (Input.GetMouseButton(1))
            {
                target = ((GunInfo)itemInfo).aimingLocalPosition;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, (((GunInfo)itemInfo).aimFieldOfView), (((GunInfo)itemInfo).aimSmoothing) * Time.deltaTime);
            }
            else
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, (((GunInfo)itemInfo).aimSmoothing * Time.deltaTime));
            }

            Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * (((GunInfo)itemInfo).aimSmoothing));

            transform.localPosition = desiredPosition;
        }
    }

    void DetermineRecoil()
    {
        transform.localPosition -= Vector3.forward * (((GunInfo)itemInfo).recoil);
    }
}
