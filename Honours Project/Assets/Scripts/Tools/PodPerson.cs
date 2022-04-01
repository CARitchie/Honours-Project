using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Pod Person")]
public class PodPerson : ScriptableObject
{
    [SerializeField] float weight;
    [SerializeField] GameObject person;
    [SerializeField] Material[] materials;

    public GameObject SpawnPerson(Transform parent)
    {
        GameObject person = Instantiate(this.person, parent);

        Transform leftShoulder = person.transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_L/Shoulder_L");
        leftShoulder.localPosition = new Vector3(-0.182f, -0.001f, 0.026f);
        leftShoulder.localEulerAngles = new Vector3(0.343f, 27.433f, 90.585f);


        Transform rightShoulder = person.transform.Find("Root/Hips/Spine_01/Spine_02/Spine_03/Clavicle_R/Shoulder_R");
        rightShoulder.localPosition = new Vector3(0.182f, -0.001f, -0.026f);
        rightShoulder.localEulerAngles = new Vector3(0.343f, 27.433f, 90.585f);

        person.transform.localPosition = Vector3.zero;

        int index = Random.Range(0, materials.Length);
        person.GetComponentInChildren<SkinnedMeshRenderer>().material = materials[index];

        return person;
    }

    public float GetWeight()
    {
        return weight;
    }
}
