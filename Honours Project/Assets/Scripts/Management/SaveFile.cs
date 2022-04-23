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

    // Function to save all of the player's data
    public void CopyFromPlayer(PlayerController player)
    {
        gravitySource = player.GetNearestSource().Key;                                                      // Find the player's closest gravity source
        position.SetData(player.transform.position - player.GetNearestSource().transform.position);         // Save the player's position relative to the nearest source
        parentRot.SetData(player.GetParentRotation());                                                      // Save the player's physics rotation
        localRot.SetData(player.GetLocalRotation());                                                        // Save the player's look direction
        player.WeaponManager()?.SaveWeapons();                                                              // Save the state of all the weapons
        currentWeapon = player.GetWeaponIndex();                                                            // Save which weapon is currently equipped

        PlayerDetails details = player.GetDetails();
        if (details == null) return;
        currentHealth = details.GetHealth();
        currentEnergy = details.GetEnergy();
        numberOfCells = details.NumberOfCells();
    }

    public void CopyFromShip(ShipController ship)
    {
        shipTransform = new RelativeTransform();
        shipTransform.CopyFromReceiver(ship.GetComponent<GravityReceiver>());       // Save the ship's position and rotation
    }

    // Function to save data for all of the cryopods
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
            weapons.Add(new WeaponData());          // Add more weapon data until the list is large enough
        }

        weapons[index].CopyFromWeaponContainer(container);
    }

    public void UnlockWeapon(int index)
    {
        if (weapons == null) weapons = new List<WeaponData>();
        while (index >= weapons.Count)
        {
            weapons.Add(new WeaponData());          // Add more weapon data until the list is large enough
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
            combatAreas.Add(key, true);         // Add the combat area to the dictionary if it doesn't already exist
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
            combatAreas.Add(key, false);        // Add the combat area to the dictionary if it doesn't already exist
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
            cryoPods.Add(key, new Pod());       // Add the cryopod to the dictionary if it doesn't already exist
        }
        cryoPods[key].SetState(state);
    }

    public void SetPodTransform(string key, GravityReceiver receiver)
    {
        if (receiver == null) return;

        if (!cryoPods.ContainsKey(key))
        {
            cryoPods.Add(key, new Pod());       // Add the cryopod to the dictionary if it doesn't already exist
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
        else if( state > upgrades[key]) upgrades[key] = state;          // Only change the upgrade state if it is of a higher value than the current state
    }

    public int NumberOfFoundPods()
    {
        int count = 0;
        foreach(KeyValuePair<string,Pod> pair in cryoPods)
        {
            if (pair.Value.GetState() >= 4) count++;                    // Increase the count if the cryopod has been found and returned
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

    public int CurrentWeapon()
    {
        return currentWeapon;
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

    // Function to work out and save an object's relative position
    public void CopyFromReceiver(GravityReceiver receiver)
    {
        GravitySource gravitySource = GravityController.FindClosest(receiver, true);
        position = new SaveFile.float3(receiver.transform.position - gravitySource.transform.position);
        rotation = new SaveFile.float3(receiver.transform.localEulerAngles);
        this.gravitySource = gravitySource.Key;
    }

    // Function to load the position and rotation data into an object
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
