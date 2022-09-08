using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    [Header("Gun stats")]
    public float damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    public int bulletsleft;
    public int bulletsShot;
    public int ammoTotal;
    public int shotsFired;
   
    [Header("Bools")]
    public bool shooting, readyToShoot, reloading;
    public bool holdingWeapon;
    public bool pistol, AR, shotgun;

    [Header("aiming")]
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;
    public float recoil;
    public float aimSmoothing;
    public float aimFieldOfView;
}
