using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [field:SerializeField]
    public Slider Slider { get; set; }

    [field: SerializeField]
    public TextMeshProUGUI TextMesh { get; set; }

    [field: SerializeField]
    public Gradient Gradient { get; set; }

    [field: SerializeField]
    public Image Fill { get; set; }

    [field: SerializeField]
    public bool HideAtMaxHealth { get; set; }

    [field: SerializeField]
    public bool HideAtZeroHealth { get; set; }

    public void SetMaxHealth(float health)
    {
        this.Slider.maxValue = health;
        this.SetHealth(health);
    }

    public void SetHealth(float health)
    {
        this.TextMesh.text = health.ToString();
        this.Slider.value = health;
        this.Fill.color = this.Gradient.Evaluate(this.Slider.normalizedValue);

        if(this.HideAtZeroHealth || this.HideAtMaxHealth)
        {
            gameObject.SetActive(
                (this.Slider.normalizedValue > 0 || !this.HideAtZeroHealth)
             && (this.Slider.normalizedValue < 1 || !this.HideAtMaxHealth)
            );
        }
    }
}
