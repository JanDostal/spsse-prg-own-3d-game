using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMaxStamina(int stamina) 
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }

    public void SetStamina(int stamina) 
    {
        slider.value = stamina;
    }
}
