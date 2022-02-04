using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image bar;

    public void SetPercent(float percent)
    {
        bar.fillAmount = percent;
    }
}
