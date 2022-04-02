using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveFile
{
    int state = 0;

    string gravitySource;
    float3 position = new float3(-450000);
    float3 localRot = new float3();
    float3 parentRot = new float3();

    float3 shipPos = new float3(-450000);
    float3 shipRot = new float3();
    string shipSource;
    List<bool> weaponStates;

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

    public void CopyFromPlayer(PlayerController player)
    {
        gravitySource = player.GetNearestSource().Key;
        position.SetData(player.transform.position - player.GetNearestSource().transform.position);
        parentRot.SetData(player.GetParentRotation());
        localRot.SetData(player.GetLocalRotation());

        SetWeaponStates(player.GetWeaponManager().GetWeaponStates());
    }

    public void CopyFromShip(ShipController ship)
    {
        GravitySource gravitySource = GravityController.FindClosest(ship.GetComponent<GravityReceiver>(), true);
        shipPos.SetData(ship.transform.position - gravitySource.transform.position);
        shipRot.SetData(ship.transform.localEulerAngles);
        shipSource = gravitySource.Key;
    }

    public Vector3 GetPosition()
    {
        return position.GetVector();
    }

    public Vector3 GetLocalRot()
    {
        return localRot.GetVector();
    }

    public Vector3 GetParentRot()
    {
        return parentRot.GetVector();
    }

    public Vector3 GetShipPos()
    {
        return shipPos.GetVector();
    }

    public Vector3 GetShipRot()
    {
        return shipRot.GetVector();
    }

    public string GetShipSource()
    {
        return shipSource;
    }

    public void SetWeaponStates(List<bool> states)
    {
        weaponStates = states;
    }

    public bool GetWeaponState(int index)
    {
        if (weaponStates == null || index >= weaponStates.Count) return false;

        return weaponStates[index];
    }

    public List<bool> GetWeaponStates()
    {
        return weaponStates;
    }

    [System.Serializable]
    struct float3
    {
        float x;
        float y;
        float z;

        public float3(float x)
        {
            this.x = x;
            y = 0;
            z = 0;
        }

        public void SetData(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public void SetData(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 GetVector()
        {
            return new Vector3(x, y, z);
        }
    }
}
