//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//public class LazerGun : MonoBehaviourPunCallbacks
//{
//    public Transform laserguntransform;
//    public ParticleSystem ps;
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (photonView.IsMine)
//        {
//            if (Input.GetMouseButtonDown(0))
//            {
//                photonView.RPC("RPC_Shoot", RpcTarget.All);
//            }
//        }
//    }

//    [PunRPC]
//    void RPC_Shoot()
//    {
//        ps.Play();
//        Ray ray = new Ray(laserguntransform.position, laserguntransform.forward);
//        if(Physics.Raycast(ray, out RaycastHit hit, 100f))
//        {
//            Debug.Log("Health_Reduce");
//            var enemyPlayerHealth = hit.collider.GetComponent<IDamageable>();
//           // if(enemyPlayerHealth)
//           // {
//           //     enemyPlayerHealth.DoDamage(20);
//           //}
//        }
//    }
//}
