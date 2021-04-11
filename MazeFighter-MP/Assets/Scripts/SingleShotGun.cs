//using Photon.Pun;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SingleShotGun : Gun
//{
//	//[SerializeField] Camera cam;
//	private float shootRatetime;
//	public float shootRate;
//	public ParticleSystem ps;
//	PhotonView PV;

//	void Awake()
//	{
//		PV = GetComponent<PhotonView>();
//	}

//	public override void Use()
//	{
//		if(Time.time > shootRatetime)
//        {
//			Shoot();
//			shootRatetime = Time.time + shootRate;
//		}
		
//	}

//	void Shoot()
//	{
//		ps.Play();
//		Transform bulletTarget = transform.Find("PlayerController/CameraHolder/Main Camera");
//		//Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
//		//ray.origin = cam.transform.position;
//		if (Physics.Raycast(bulletTarget.position, bulletTarget.forward, out RaycastHit hit, 1000f))
//		{

//			hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
//			PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
//		}
//	}

//	[PunRPC]
//	void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
//	{
//		Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
//		if (colliders.Length != 0)
//		{
//			GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
//			bulletImpactObj.GetComponent<ShotBehavior>().setTarget(hitPosition);
//			GameObject.Destroy(bulletImpactObj, 10f);
//			bulletImpactObj.transform.SetParent(colliders[0].transform);
//		}
//	}
//}
