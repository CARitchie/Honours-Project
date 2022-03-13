using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerAccessor : MonoBehaviour
{
    public void FirstLoad()
    {
        SceneManager.SetToBlack();
        SceneManager.LoadScene("Space");
    }
}
