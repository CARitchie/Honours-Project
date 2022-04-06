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

    RelativeTransform shipTransform;
    List<bool> weaponStates;

    float currentHealth = 100;
    float currentEnergy = 200;
    int numberOfCells = 0;

    Dictionary<string, bool> combatAreas = new Dictionary<string, bool>();
    Dictionary<string, Pod> cryoPods = new Dictionary<string, Pod>();
    Dictionary<string, int> upgrades = new Dictionary<string, int>();

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

        PlayerDetails details = player.GetDetails();
        if (details == null) return;
        currentHealth = details.GetHealth();
        currentEnergy = details.GetEnergy();
        numberOfCells = details.NumberOfCells();
    }

    public void CopyFromShip(ShipController ship)
    {
        shipTransform = new RelativeTransform();
        shipTransform.CopyFromReceiver(ship.GetComponent<GravityReceiver>());
    }

    public void CopyCryoPods(List<CryoPod> cryoPods)
    {
        foreach(CryoPod pod in cryoPods)
        {
            SetPodTransform(pod.Key, pod.Receiver);
        }
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetEnergy()
    {
        return currentEnergy;
    }

    public int NumberOfCells()
    {
        return numberOfCells;
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

    public RelativeTransform GetShipData()
    {
        return shipTransform;
    }

    public void SetWeaponStates(List<bool> states)
    {
        weaponStates = states;
    }

    public void UnlockWeapon(int index)
    {
        if (weaponStates == null) weaponStates = new List<bool>();
        while(index >= weaponStates.Count)
        {
            weaponStates.Add(false);
        }

        weaponStates[index] = true;
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

    public void CompleteCombatArea(string key)
    {
        if (!combatAreas.ContainsKey(key))
        {
            combatAreas.Add(key, true);
        }
        else
        {
            combatAreas[key] = true;
        }
    }

    public bool IsCombatAreaComplete(string key)
    {
        if (!combatAreas.ContainsKey(key))
        {
            combatAreas.Add(key, false);
            return false;
        }

        return combatAreas[key];
    }

    public Pod GetPod(string key)
    {
        if (cryoPods.ContainsKey(key))
        {
            return cryoPods[key];
        }
        return null;
    }

    public void SetPodState(string key, int state)
    {
        if (!cryoPods.ContainsKey(key))
        {
            cryoPods.Add(key, new Pod());
        }
        cryoPods[key].SetState(state);
    }

    public void SetPodTransform(string key, GravityReceiver receiver)
    {
        if (receiver == null) return;

        if (!cryoPods.ContainsKey(key))
        {
            cryoPods.Add(key, new Pod());
        }

        cryoPods[key].transform.CopyFromReceiver(receiver);
    }

    public int GetUpgradeState(string key)
    {
        if (!upgrades.ContainsKey(key)) return -1;
        return upgrades[key];
    }

    public void SetUpgradeState(string key, int state)
    {
        if (!upgrades.ContainsKey(key)) upgrades.Add(key, state);
        else upgrades[key] = state;
    }

    [System.Serializable]
    public struct float3
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

        public float3(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
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


    [System.Serializable]
    public class Pod
    {
        public RelativeTransform transform = new RelativeTransform();
        int state = -1;

        public void SetState(int value)
        {
            if (value > state) state = value;
        }

        public int GetState()
        {
            return state;
        }
    }
}

[System.Serializable]
public class RelativeTransform
{
    SaveFile.float3 position;
    SaveFile.float3 rotation;
    string gravitySource = "null";

    public void CopyFromReceiver(GravityReceiver receiver)
    {
        GravitySource gravitySource = GravityController.FindClosest(receiver, true);
        position = new SaveFile.float3(receiver.transform.position - gravitySource.transform.position);
        rotation = new SaveFile.float3(receiver.transform.localEulerAngles);
        this.gravitySource = gravitySource.Key;
    }

    public bool LoadIntoTransform(Transform transform, out Vector3 velocity)
    {
        velocity = Vector3.zero;

        if (gravitySource == "null" | !GravityController.FindSource(gravitySource, out GravitySource source)) return false;

        transform.position = position.GetVector() + source.transform.position;
        transform.localEulerAngles = rotation.GetVector();
        velocity = source.GetVelocity();

        return true;
    }

    public Vector3 Position { get { return position.GetVector(); } }
    public Vector3 Rotation { get { return rotation.GetVector(); } }
    public string GravitySource { get { return gravitySource; } }
}
