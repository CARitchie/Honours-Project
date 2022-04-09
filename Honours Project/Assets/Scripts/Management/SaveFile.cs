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
    List<WeaponData> weapons;

    float currentHealth = 100;
    float currentEnergy = 200;
    int numberOfCells = 0;
    int currentWeapon = 0;

    Dictionary<string, bool> combatAreas = new Dictionary<string, bool>();
    Dictionary<string, Pod> cryoPods = new Dictionary<string, Pod>();
    Dictionary<string, UpgradeState> upgrades = new Dictionary<string, UpgradeState>();
    Dictionary<string, bool> booleans = new Dictionary<string, bool>();

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
        player.WeaponManager()?.SaveWeapons();

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

    public void SetWeaponState(int index, WeaponContainer container)
    {
        if (weapons == null) weapons = new List<WeaponData>();
        while(index >= weapons.Count)
        {
            weapons.Add(new WeaponData());
        }

        weapons[index].CopyFromWeaponContainer(container);
    }

    public void UnlockWeapon(int index)
    {
        if (weapons == null) weapons = new List<WeaponData>();
        while (index >= weapons.Count)
        {
            weapons.Add(new WeaponData());
        }

        weapons[index].unlocked = true;
    }

    public WeaponData GetWeaponData(int index)
    {
        if (weapons == null || index >= weapons.Count) return null;

        return weapons[index];
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

    public UpgradeState GetUpgradeState(string key)
    {
        if (!upgrades.ContainsKey(key)) return UpgradeState.NonExistent;
        return upgrades[key];
    }

    public void SetUpgradeState(string key, UpgradeState state)
    {
        if (!upgrades.ContainsKey(key)) upgrades.Add(key, state);
        else if( state > upgrades[key]) upgrades[key] = state;
    }

    public int NumberOfFoundPods()
    {
        int count = 0;
        foreach(KeyValuePair<string,Pod> pair in cryoPods)
        {
            if (pair.Value.GetState() >= 4) count++;
        }
        return count;
    }

    public bool GetBool(string key)
    {
        if(booleans == null || !booleans.ContainsKey(key)) return false;

        return booleans[key];
    }

    public void SetBool(string key, bool value)
    {
        if (booleans == null) return;
        if (!booleans.ContainsKey(key)) booleans.Add(key, value);
        else booleans[key] = value;
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
        int state = -1; // 1 - picked up, 4 - returned

        public void SetState(int value)
        {
            if (value > state) state = value;
        }

        public int GetState()
        {
            return state;
        }
    }

    [System.Serializable]
    public enum UpgradeState
    {
        NonExistent = -1,
        NotAvailable = 0,
        Available = 1,
        PlayerUnlocked = 5,
        Sacrificed = 10,
        Lost = 15
    }

    [System.Serializable]
    public class WeaponData
    {
        public bool unlocked = false;
        public int currentAmmo = -1000;

        public void CopyFromWeaponContainer(WeaponContainer container)
        {
            unlocked = !container.IsLocked();
            currentAmmo = container.GetWeapon().GetAmmo();
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
