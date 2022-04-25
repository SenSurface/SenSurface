using UnityEngine;
using UnityEngine.UI;

public class SliderUpdateInt : MonoBehaviour
{
    public Slider slider = null;
    public Text textValue;

    public void Start()
    {
        try
        {

            if (slider == null)
                slider = this.transform.GetComponent<Slider>();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
        SetTextValue();
    }
    public void SetTextValue()
    {
        if(slider != null)
            textValue.text = slider.value.ToString();
    }

}
