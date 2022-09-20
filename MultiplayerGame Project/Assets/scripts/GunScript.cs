using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunScript : Gun
{
    PhotonView pv;
    [SerializeField] Camera cam;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    public void Start()
    {
        ((GunInfo)itemInfo).bulletsleft = ((GunInfo)itemInfo).magazineSize;
        ((GunInfo)itemInfo).readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
        DetermineAim();
    }

    private void MyInput()
    {
        if (((GunInfo)itemInfo).allowButtonHold) ((GunInfo)itemInfo).shooting = Input.GetKey(KeyCode.Mouse0);
        else ((GunInfo)itemInfo).shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKey(KeyCode.R) && ((GunInfo)itemInfo).bulletsleft < ((GunInfo)itemInfo).magazineSize && ((GunInfo)itemInfo).reloading == false) Reload();


        //shoot
        if (((GunInfo)itemInfo).readyToShoot && ((GunInfo)itemInfo).shooting && ((GunInfo)itemInfo).reloading == false && ((GunInfo)itemInfo).bulletsleft > 0)
        {
            ((GunInfo)itemInfo).bulletsShot = ((GunInfo)itemInfo).bulletsPerTap;
            Shoot();
            ((GunInfo)itemInfo).readyToShoot = false;
            DetermineRecoil();
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

        //Raycast
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit, ((GunInfo)itemInfo).range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            //pv.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }

        ((GunInfo)itemInfo).bulletsleft--;
        ((GunInfo)itemInfo).bulletsShot--;
        ((GunInfo)itemInfo).shotsFired++;

        Invoke("ResetShot",((GunInfo)itemInfo).timeBetweenShooting);

        if(((GunInfo)itemInfo).bulletsShot > 0 && ((GunInfo)itemInfo).bulletsleft > 0)
        {
            Invoke("Shoot", ((GunInfo)itemInfo).timeBetweenShooting);
        }
    }

    void ResetShot()
    {
        ((GunInfo)itemInfo).readyToShoot = true;
    }

    void Reload()
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

        if(((GunInfo)itemInfo).ammoTotal <= 0)
        {
            ((GunInfo)itemInfo).ammoTotal = 0;
            ((GunInfo)itemInfo).shotsFired = 0;
            ((GunInfo)itemInfo).magazineSize = 0;
        }
    }

    void DetermineAim()
    {
        if (pv.IsMine)
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
