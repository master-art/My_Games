using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject cameraHolder;

	private float shootRatetime;

	[SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

	[SerializeField] GunData[] gunitems;

	public LayerMask canBeShot;

	int currentIndex;

	public Transform itemHolder;
	private GameObject currentWeapon;
	private float currentCooldown;

	int itemIndex;

	int previousItemIndex = -1;

	float verticalLookRotation;
	bool grounded;
	Vector3 smoothMoveVelocity;
	Vector3 moveAmount;

	public float checkHealth{ get; set;}

	Rigidbody rb;

	PhotonView PV;

	const float maxHealth = 100f;
	public float currentHealth = maxHealth;

	PlayerManager playerManager;


	private Crosshair m_crosshair;
	private Crosshair crosshair
    {
        get
        {
            if(m_crosshair == null)
			m_crosshair = GetComponentInChildren<Crosshair>();
			return m_crosshair;
        }
    }

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		PV = GetComponent<PhotonView>();
		playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
	}

	void Start()
	{
		if (PV.IsMine)
		{
			photonView.RPC("EquipItem", RpcTarget.All, 0);
			checkHealth = currentHealth;
		}
		else
		{
			Destroy(GetComponentInChildren<Camera>().gameObject);
			gameObject.layer = 8;
			Destroy(rb);
		}
	}

	void Update()
	{
		if (!PV.IsMine)
			return;

		Jump();
		Look();
		Move();


		for (int i = 0; i < gunitems.Length; i++)
		{
			if (Input.GetKeyDown((i + 1).ToString()))
			{
				photonView.RPC("EquipItem", RpcTarget.All, i);
				break;
			}
		}

		if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
		{
			if (itemIndex >= gunitems.Length - 1)
			{
				photonView.RPC("EquipItem", RpcTarget.All, 0);
			}
			else
			{
				photonView.RPC("EquipItem", RpcTarget.All, itemIndex + 1);
			}
		}
		else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
		{
			if (itemIndex <= 0)
			{
				photonView.RPC("EquipItem", RpcTarget.All, gunitems.Length - 1);
			}
			else
			{
				photonView.RPC("EquipItem", RpcTarget.All, itemIndex - 1);
			}
		}
		
		if(currentWeapon != null)
        {
			//Debug.Log("Inside Aim" + Input.GetMouseButton(1));
			Aim(Input.GetMouseButton(1));

			if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
				Debug.Log("Shoot is called");
				PV.RPC("Shoot", RpcTarget.All);
				//if (Time.time > shootRatetime)
				//{
					
				//	shootRatetime = Time.time + gunitems[itemIndex].fireRate;
				//}
            }

			//weapon postion elasticity
			currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

			//cooldown
			if (currentCooldown > 0) currentCooldown -= Time.deltaTime;


		}
		

		if (transform.position.y < -10f) // Die if you fall out of the world
		{
			Die();
		}
	}

	void Look()
	{
		transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

		verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

		cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;


	}

	void Move()
	{
		Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

		moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
	}

	void Jump()
	{
		Debug.Log("not in jump area");
		Debug.Log("Grounded value" + grounded);
		if (Input.GetKeyDown(KeyCode.Space) && grounded)
		{
			Debug.Log("Enter in Jump area");
			rb.AddForce(transform.up * jumpForce);
		}
	}

	[PunRPC]
	void EquipItem(int _index)
	{
		if (_index == previousItemIndex)
			return;

		if (currentWeapon != null) Destroy(currentWeapon);

		itemIndex = _index;
		currentIndex = _index;

		GameObject t_newweapon = Instantiate(gunitems[itemIndex].gunPrefab, itemHolder.position, itemHolder.rotation, itemHolder) as GameObject;
		t_newweapon.transform.localPosition = Vector3.zero;
		t_newweapon.transform.localEulerAngles = Vector3.zero;
		currentWeapon = t_newweapon;

		//if (previousItemIndex != -1)
		//{
		//	Debug.Log("Inside previousItemIndex:" + previousItemIndex);
		//	Destroy(gunitems[previousItemIndex].gunPrefab);
		//}
		
		//Debug.Log("Inside previousItemIndex:" + itemIndex);
		//previousItemIndex = itemIndex;


		if (PV.IsMine)
		{
			Hashtable hash = new Hashtable();
			hash.Add("itemIndex", itemIndex);
			PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		}
	}

	void Aim(bool p_isAiming)
    {
		Transform t_anchor = currentWeapon.transform.Find("Anchor");
		Transform t_gunposition_ads = currentWeapon.transform.Find("GunPosition/ADS");
		Transform t_gunposition_hit = currentWeapon.transform.Find("GunPosition/Hip");
        
		if (p_isAiming)
        {
			t_anchor.position = Vector3.Lerp(t_anchor.position, t_gunposition_ads.position, Time.deltaTime * gunitems[currentIndex].aimSpeed);
			Debug.Log("ADS position" + " " + t_gunposition_ads);
		}
        else
        {
			t_anchor.position = Vector3.Lerp(t_anchor.position, t_gunposition_hit.position, Time.deltaTime * gunitems[currentIndex].aimSpeed);
			Debug.Log("Hit position" + " " + t_gunposition_hit);
		}
    }


	[PunRPC]
	void Shoot()
	{
		Transform bulletTarget = transform.Find("CameraHolder/Normal Camera");

		//bloom
		Vector3 t_bloom = bulletTarget.position + bulletTarget.forward * 1000f;
		t_bloom += Random.Range(-gunitems[currentIndex].gunBloom, gunitems[currentIndex].gunBloom) * bulletTarget.up;
		t_bloom += Random.Range(-gunitems[currentIndex].gunBloom, gunitems[currentIndex].gunBloom) * bulletTarget.right;
		t_bloom -= bulletTarget.position;
		t_bloom.Normalize();

		//Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
		//ray.origin = cam.transform.position;
		RaycastHit t_hit = new RaycastHit();
		if (Physics.Raycast(bulletTarget.position, bulletTarget.forward, out t_hit, 1000f, canBeShot))
		{
			//gunitems[currentIndex].gunParticle.Play();
			//GameObject bulletImpactObj = Instantiate(gunitems[currentIndex].bulletImpactPrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.LookRotation(t_hit.normal, Vector3.up) * gunitems[currentIndex].bulletImpactPrefab.transform.rotation);
			//bulletImpactObj.GetComponent<ShotBehavior>().setTarget(t_hit.point);
			//GameObject.Destroy(bulletImpactObj, 10f);

			Debug.Log("Bullet Target" + " " + bulletTarget.position);
			PV.RPC("RPC_Shoot", RpcTarget.All, t_hit.point, t_hit.normal);
			if (PV.IsMine)
            {
				Debug.Log("Inside Take Damage! ");
				if (t_hit.collider.gameObject.layer == 8)
				{
					Debug.Log("Checking Damage" + " " + gunitems[currentIndex].damage);
					t_hit.collider.gameObject.GetPhotonView().RPC("RPC_TakeDamage", RpcTarget.All, gunitems[currentIndex].damage);
					
				}
			}
			
		}

		//gun fx
		currentWeapon.transform.Rotate(-gunitems[currentIndex].gunRecoil, 0, 0);
		currentWeapon.transform.position -= currentWeapon.transform.forward * gunitems[currentIndex].gunKickback;


		//cooldown
		currentCooldown = gunitems[currentIndex].fireRate;

	}

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
    	Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
    	if (colliders.Length != 0)
    	{
			gunitems[currentIndex].gunParticle.Play();	
			GameObject bulletImpactObj = Instantiate(gunitems[currentIndex].bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * gunitems[currentIndex].bulletImpactPrefab.transform.rotation);
    		bulletImpactObj.GetComponent<ShotBehavior>().setTarget(hitPosition);
            Destroy(bulletImpactObj, 10f);
    		bulletImpactObj.transform.SetParent(colliders[0].transform);
    	}
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
		if (!PV.IsMine && targetPlayer == PV.Owner)
		{
			photonView.RPC("EquipItem", RpcTarget.All, (int)changedProps["itemIndex"]);

		}
	}

	public void SetGroundedState(bool _grounded)
	{
		Debug.Log("Enter in SetGround Jump area");
		grounded = _grounded;
	}

	void FixedUpdate()
	{
		if (!PV.IsMine)
			return;

		rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
	}

	//public void TakeDamage(float damage)
	//{
	//	PV.RPC("RPC_TakeDamage", RpcTarget.All, damage);
	//}

	[PunRPC]
	void RPC_TakeDamage(float damage)
	{
		Debug.Log("Inside Take Damage! ");
		if (!PV.IsMine)
			return;

		currentHealth -= damage;

		checkHealth = currentHealth;

		Debug.Log("current Health" + " " + currentHealth);
		Debug.Log("Check Health" + " " + checkHealth);

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	void Die()
	{
		playerManager.Die();
	}



}