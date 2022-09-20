using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Gun : Item
{
    public GameObject bulletImpactPrefab;
    public GameObject hitMarker;
    public TMP_Text ammoText;

}
