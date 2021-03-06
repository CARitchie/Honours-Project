using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public static class SaveManager
{
    public delegate void UpgradeChanged();
    public static event UpgradeChanged OnUpgradeChanged;

    public static SaveFile save;
    static string filePath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar + "SaveData.dat";

    public static SaveFile LoadGame()
    {
        if (!SaveFileExists())
        {
            CreateNewSave();            // Create a new save file if one doesn't already exist
        }

        return save;
    }

    public static bool SaveFileExists()
    {
        if (File.Exists(filePath))
        {
            // Load the save file and store it in the save field
            FileStream file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                save = (SaveFile)bf.Deserialize(file);
                file.Close();
                return true;
            }
            catch (SerializationException e)
            {
                Debug.LogError("Failed to deserialize Save File. Reason: " + e.Message);
                file.Close();
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public static void CreateNewSave()
    {
        // If there is no save file
        // create one
        Debug.Log("Creating new save file");
        save = new SaveFile();
        SaveToFile();
        OnUpgradeChanged?.Invoke();
    }

    public static void SaveToFile()
    {
        if(!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves");
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);

        try
        {
            bf.Serialize(file, save);
            file.Close();
            Debug.Log("Game data saved!");
        }
        catch (SerializationException e)
        {
            Debug.LogError("Failed to save game data. Reason: " + e.Message);
            file.Close();
        }
    }

    public static int GetState()
    {
        if (save == null) return -450;

        return save.GetState();
    }

    public static void SetGameState(int state, bool forceSave = false)
    {
        if (save == null) return;

        save.SetState(state);
        if(forceSave) SaveToFile();
    }

    public static bool SaveExists()
    {
        return save != null;
    }

    public static Vector3 GetRelativePlayerPos()
    {
        if (!SaveExists()) return new Vector3(-450000,0,0);

        return save.GetPosition();
    }

    public static RelativeTransform GetShipTransform()
    {
        if (!SaveExists()) return null;

        return save.GetShipData();
    }

    public static string GetGravitySource()
    {
        if (!SaveExists()) return "null";

        return save.GetGravitySource();
    }

    public static SaveFile.Pod GetPod(string key)
    {
        if (!SaveExists()) return null;
        return save.GetPod(key);
    }

    public static void SetPodTransform(string key, GravityReceiver receiver)
    {
        if (!SaveExists()) return;
        save.SetPodTransform(key, receiver);
    }

    public static void SetPodState(string key, int state)
    {
        if (!SaveExists()) return;
        save.SetPodState(key, state);
    }

    public static SaveFile.UpgradeState GetUpgradeState(string key)
    {
        if (!SaveExists()) return SaveFile.UpgradeState.NonExistent;
        return save.GetUpgradeState(key);
    }

    public static void SetUpgradeState(string key, SaveFile.UpgradeState state)
    {
        if (!SaveExists()) return;
        save.SetUpgradeState(key, state);
        OnUpgradeChanged?.Invoke();
    }

    // Function to determine whether the specified sacrifce/upgrade was sacrificed
    public static bool SacrificeMade(string key)
    {
        if (!SaveExists()) return false;
        SaveFile.UpgradeState state = save.GetUpgradeState(key);
        return state == SaveFile.UpgradeState.Sacrificed;
    }

    // Function to determine whether the specified upgrade was used for the player or colony ship
    public static bool SelfUpgraded(string key)
    {
        if (!SaveExists()) return false;
        return save.GetUpgradeState(key) == SaveFile.UpgradeState.PlayerUnlocked;
    }

    public static int NumberOfFoundPods()
    {
        if (!SaveExists()) return 0;
        return save.NumberOfFoundPods();
    }

    public static bool GetBool(string key)
    {
        if (!SaveExists()) return false;
        return save.GetBool(key);
    }

    public static void SetBool(string key, bool value)
    {
        if (!SaveExists()) return;
        save.SetBool(key, value);
    }

    public static int CurrentWeapon()
    {
        if (!SaveExists()) return 0;
        return save.CurrentWeapon();
    }

    // Try to save the game
    public static bool AttemptSave()
    {
        if (save == null || PlayerController.Instance == null || !PlayerController.Instance.Saveable()) return false;

        SaveGame();

        return true;
    }

    // Copy all data and write to file
    static void SaveGame() {
        save.CopyFromPlayer(PlayerController.Instance);
        save.CopyFromShip(ShipController.Instance);
        save.CopyCryoPods(CryoPod.allPods);
        SaveToFile();
    }

    public static SaveFile.WeaponData GetWeaponState(int index)
    {
        if (!SaveExists()) return null;

        return save.GetWeaponData(index);
    }

    public static bool WeaponUnlocked(int index)
    {
        if (!SaveExists()) return false;

        SaveFile.WeaponData weapon = save.GetWeaponData(index);
        if (weapon == null) return false;
        return weapon.unlocked;
    }

    public static void UnlockWeapon(int index)
    {
        if (!SaveExists()) return;
        save.UnlockWeapon(index);
    }

    public static void SetWeaponData(int index, WeaponContainer container)
    {
        if (!SaveExists()) return;
        save.SetWeaponState(index, container);
    }

    public static void CompleteCombatArea(string key)
    {
        if (!SaveExists()) return;

        save.CompleteCombatArea(key);
    }

    public static bool IsCombatAreaComplete(string key)
    {
        if (!SaveExists()) return false;

        return save.IsCombatAreaComplete(key);
    }
}
