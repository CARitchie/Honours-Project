using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Image bar;
    [SerializeField] GameObject reductionBar;

    public void SetPercent(float percent)
    {
        bar.fillAmount = percent;
    }

    public void ReduceMax()
    {
        reductionBar.SetActive(true);
    }
}
