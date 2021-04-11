using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName ="GunInfo")]
public class GunData : ScriptableObject
{
    public string gunName;
    public float fireRate;
    public float gunBloom;
    public float gunRecoil;
    public float gunKickback;
    public float damage;
    public ParticleSystem gunParticle;
    public float aimSpeed;
    public GameObject gunPrefab;
    public GameObject bulletImpactPrefab;
}
