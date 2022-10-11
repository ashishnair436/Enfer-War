using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SyncPosition : NetworkBehaviour
{

    private GameObject playerBody;
    private Rigidbody physicsRoot;

    void Start()
    {
        playerBody = transform.GetChild(0).gameObject;
        physicsRoot = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            CmdSyncPos(transform.localPosition, transform.localRotation, playerBody.transform.localRotation, physicsRoot.velocity, transform.parent.name);
        }
    }

    // Send position to the server and run the RPC for everyone, including the server. 
    [Command]
    protected void CmdSyncPos(Vector3 localPosition, Quaternion localRotation, Quaternion bodyRotation, Vector3 velocity, string parentName)
    {
        RpcSyncPos(localPosition, localRotation, bodyRotation, velocity, parentName);
    }

    // For each player, transfer the position from the server to the client, and set it as long as it's not the local player. 
    [ClientRpc]
    void RpcSyncPos(Vector3 localPosition, Quaternion localRotation, Quaternion bodyRotation, Vector3 velocity, string parentName)
    {
        if (playerBody == null)
        {
            return;
        }
        if (!isLocalPlayer)
        {
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;
            playerBody.transform.localRotation = bodyRotation;
            physicsRoot.velocity = velocity;

            if (!transform.parent.name.Equals(parentName))
            {
                transform.parent = GameObject.Find(parentName).transform;
            }
        }
    }
}