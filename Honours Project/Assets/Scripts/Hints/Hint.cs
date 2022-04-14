using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "My Assets/Hint")]
public class Hint : ScriptableObject
{
    [SerializeField] string key;
    [SerializeField] string text;
    [SerializeField] float additionalTime = 0;

    public string Key { get { return key; } }
    public string Text { get { return text; } }
    public float Time { get { return additionalTime; } }
}
