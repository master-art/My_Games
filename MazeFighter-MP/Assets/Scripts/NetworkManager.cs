using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager Instance;

	[SerializeField] TMP_InputField roomNameInputField;
	[SerializeField] TMP_Text errorText;
	[SerializeField] TMP_Text roomNameText;
	[SerializeField] Transform roomListContent;
	[SerializeField] GameObject roomListItemPrefab;
	[SerializeField] Transform playerListContent;
	[SerializeField] GameObject PlayerListItemPrefab;
	[SerializeField] GameObject startGameButton;
	[SerializeField] TMP_InputField playerNameField;

	void Awake()
	{
		Instance = this;
	}
	void Start()
    {
		Debug.Log("Connecting to Master");
        if (!PhotonNetwork.IsConnected)
        {
			Debug.Log("Connected");
			Connect();

        }
		//Debug.Log("Connected");
	}

    public void Connect()
    {
		Debug.Log("Inside Connect()");
		PhotonNetwork.ConnectUsingSettings();
    }


	public override void OnConnectedToMaster()
	{
		Debug.Log("Connected to Master");
		PhotonNetwork.JoinLobby();
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public override void OnJoinedLobby()
	{
		
		MenuManager.Instance.OpenMenu("usermenu");
		Debug.Log(" On Joined Lobby");
		PlayerName();

	}


	public string PlayerName()
    {
		if (string.IsNullOrEmpty(playerNameField.text))
		{
			return PhotonNetwork.NickName = "Player:" + Random.Range(0, 1000).ToString("0000");
		}
        else
        {
			return PhotonNetwork.NickName = "Player:" + " " + playerNameField.text;
		}
		
    }

	public void CreateRoom()
	{
		

		Debug.Log("Create Room");
		if (string.IsNullOrEmpty(roomNameInputField.text))
		{
			return;
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions{ MaxPlayers = 4});
		PlayerName();
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnJoinedRoom()
	{
		MenuManager.Instance.OpenMenu("room");
		roomNameText.text = PhotonNetwork.CurrentRoom.Name;

		Player[] players = PhotonNetwork.PlayerList;
		//PlayerName();

		foreach (Transform child in playerListContent)
		{
			Destroy(child.gameObject);
		}

		for (int i = 0; i < players.Count(); i++)
		{
			Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
		}

		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		Debug.Log("Master Client");
		startGameButton.SetActive(PhotonNetwork.IsMasterClient);
	}


	/// <summary>
	/// When Room with same name is already there will pop up error 
	/// </summary>
	/// <param name="returnCode"></param>
	/// <param name="message"></param>
	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		errorText.text = "Room Creation Failed: " + message;
		Debug.LogError("Room Creation Failed: " + message);
		MenuManager.Instance.OpenMenu("error");
	}
	
	public void StartGame()
	{
		Debug.Log("Level Load");
		PhotonNetwork.LoadLevel(1);
	}

	/// <summary>
	/// Leave Room Function 
	/// </summary>
	public void LeaveRoom()
	{
		Debug.Log("Leave Lobby");
		PhotonNetwork.LeaveRoom();
		MenuManager.Instance.OpenMenu("loading");
	}

	/// <summary>
	/// It is used to get user info and is called in RoomListItem.cs
	/// </summary>
	/// <param name="info"></param>
	public void JoinRoom(RoomInfo info)
	{
		Debug.Log("Join Room");
		PlayerName();
		PhotonNetwork.JoinRoom(info.Name);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnLeftRoom()
	{
		MenuManager.Instance.OpenMenu("title");
	}

	
	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		Debug.Log("Room List Update Lobby");
		foreach (Transform trans in roomListContent)
		{
			Destroy(trans.gameObject);
		}

		for (int i = 0; i < roomList.Count; i++)
		{
			if (roomList[i].RemovedFromList)
				continue;

			Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		//PlayerName();
		Debug.Log("Player Entered Room");
		Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
	}


}
