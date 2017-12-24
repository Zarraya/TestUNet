using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : MonoBehaviour {

    private const float _maxStamina = 100;

    public float Stamina = _maxStamina;
    public RectTransform StaminaBar;

    public void Decrease(float amount)
    {
        Stamina -= amount;
        if(Stamina < 0)
        {
            Stamina = 0;
        }

        StaminaBar.sizeDelta = new Vector2(Stamina, StaminaBar.sizeDelta.y);
    }

    private void Update()
    {
        //add to stamina with the frame time taken into consideration.
        if (Stamina < 100)
        {
            Stamina += 2 * Time.deltaTime;
        }

        if (Stamina > 100)
        {
            Stamina = 100;
        }

        StaminaBar.sizeDelta = new Vector2(Stamina, StaminaBar.sizeDelta.y);
    }
}
