using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    public PlayerData player = new PlayerData(-450000);
    int state = 0;
    string gravitySource;


    public int GetState()
    {
        return state;
    }

    public void SetState(int newState)
    {
        if (newState > state)
        {
            state = newState;
        }
    }

    public string GetGravitySource()
    {
        return gravitySource;
    }

    public void SetSource(string source)
    {
        gravitySource = source;
    }

    [System.Serializable]
    public struct PlayerData{
        float posX;
        float posY;
        float posZ;

        float lRotX;
        float lRotY;
        float lRotZ;

        float pRotX;
        float pRotY;
        float pRotZ;

        public PlayerData(float x)
        {
            posX = x;
            posY = 0;
            posZ = 0;

            lRotX = 0;
            lRotY = 0;
            lRotZ = 0;

            pRotX = 0;
            pRotY = 0;
            pRotZ = 0;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(posX, posY, posZ);
        }

        public Vector3 GetLocalRot()
        {
            return new Vector3(lRotX, lRotY, lRotZ);
        }

        public Vector3 GetParentRot()
        {
            return new Vector3(pRotX, pRotY, pRotZ);
        }

        public void SetPosition(Vector3 playerPos)
        {
            posX = playerPos.x;
            posY = playerPos.y;
            posZ = playerPos.z;
        }

        public void SetLocalRot(Vector3 localRot)
        {
            lRotX = localRot.x;
            lRotY = localRot.y;
            lRotZ = localRot.z;
        }

        public void SetParentRot(Vector3 parentRot)
        {
            pRotX = parentRot.x;
            pRotY = parentRot.y;
            pRotZ = parentRot.z;
        }

        public void CopyFromPlayer(PlayerController player)
        {
            SetPosition(player.transform.position - player.GetNearestSource().transform.position);
            SetParentRot(player.GetParentRotation());
            SetLocalRot(player.GetLocalRotation());
        }

    }
}
