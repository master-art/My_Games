//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;
//using System.IO;

//public class GameManager : MonoBehaviourPunCallbacks
//{
//    // Start is called before the first frame update
//    public GameObject myprefab;
//    PhotonView PV;

//    public Transform[] SpawnPoints; 
//    void Start()
//    {
//        int spawnpicker = Random.Range(0, SpawnPoints.Length);
       
//            PhotonNetwork.Instantiate(myprefab.name, SpawnPoints[spawnpicker].position, Quaternion.identity);
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
