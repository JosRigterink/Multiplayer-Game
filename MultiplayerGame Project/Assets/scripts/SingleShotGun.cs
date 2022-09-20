using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;

    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        ((GunInfo)itemInfo).bulletsleft = ((GunInfo)itemInfo).magazineSize;
        ((GunInfo)itemInfo).shotsFired = 0;
        ((GunInfo)itemInfo).readyToShoot = true;
    }

    void Update()
    {
        MyInput();
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

    void MyInput()
    {
        if (PV.IsMine)
        {
            if (((GunInfo)itemInfo).allowButtonHold) ((GunInfo)itemInfo).shooting = Input.GetKey(KeyCode.Mouse0);
            else ((GunInfo)itemInfo).shooting = Input.GetKeyDown(KeyCode.Mouse0);


            if (Input.GetKey(KeyCode.R) && ((GunInfo)itemInfo).bulletsleft < ((GunInfo)itemInfo).magazineSize && ((GunInfo)itemInfo).reloading == false) Reload();

            if (((GunInfo)itemInfo).readyToShoot && ((GunInfo)itemInfo).shooting && ((GunInfo)itemInfo).reloading == false && ((GunInfo)itemInfo).bulletsleft > 0)
            {
                ((GunInfo)itemInfo).bulletsShot = ((GunInfo)itemInfo).bulletsPerTap;
                Shoot();
                ((GunInfo)itemInfo).readyToShoot = false;
                DetermineRecoil();
            }
        }
    }

    void Shoot()
    {
        //spread
        float x = Random.Range(-((GunInfo)itemInfo).spread, ((GunInfo)itemInfo).spread);
        float y = Random.Range(-((GunInfo)itemInfo).spread, ((GunInfo)itemInfo).spread);
        float z = Random.Range(-((GunInfo)itemInfo).spread, ((GunInfo)itemInfo).spread);

        //calculate Direction with spread
        Vector3 direction = cam.transform.forward + new Vector3(x, y, z);

        //raycast
        //Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        //ray.origin = cam.transform.position;
        if(Physics.Raycast(cam.transform.position, direction, out RaycastHit hit,((GunInfo)itemInfo).range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
            Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.tag == "Player")
            {
                hitMarker.SetActive(true);
                Invoke("HitmarkerAway", 0.3f);
            }
            DetermineRecoil();
        }

        ((GunInfo)itemInfo).bulletsleft--;
        ((GunInfo)itemInfo).bulletsShot--;
        ((GunInfo)itemInfo).shotsFired++;

        Invoke("ResetShot", ((GunInfo)itemInfo).timeBetweenShooting);

        if (((GunInfo)itemInfo).bulletsShot > 0 && ((GunInfo)itemInfo).bulletsleft > 0)
        {
         Invoke("Shoot", ((GunInfo)itemInfo).timeBetweenShots);
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
    void ResetShot()
    {
        ((GunInfo)itemInfo).readyToShoot = true;
    }
    private void Reload()
    {
        ((GunInfo)itemInfo).reloading = true;
        Invoke("ReloadFinished", ((GunInfo)itemInfo).reloadTime);
    }
    private void ReloadFinished()
    {
        if (((GunInfo)itemInfo).ammoTotal <= 0 && ((GunInfo)itemInfo).bulletsleft <= 0)
        {
            ((GunInfo)itemInfo).magazineSize = 0;
            ((GunInfo)itemInfo).ammoTotal = 0;
            ((GunInfo)itemInfo).shotsFired = 0;
        }

       ((GunInfo)itemInfo).ammoTotal -= ((GunInfo)itemInfo).shotsFired;
        ((GunInfo)itemInfo).bulletsleft = ((GunInfo)itemInfo).magazineSize;
        ((GunInfo)itemInfo).reloading = false;
        ((GunInfo)itemInfo).shotsFired -= ((GunInfo)itemInfo).shotsFired;

        if (((GunInfo)itemInfo).ammoTotal <= 0)
        {
            ((GunInfo)itemInfo).ammoTotal = 0;
            ((GunInfo)itemInfo).shotsFired = 0;
            ((GunInfo)itemInfo).magazineSize = 0;
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
        if (PV.IsMine)
        {
            transform.localPosition -= Vector3.forward * (((GunInfo)itemInfo).recoil);
        }
    }

    void HitmarkerAway()
    {
        hitMarker.SetActive(false);
    }
}
