using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UISettingsToggle : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    [SerializeField] GameObject toggleOffGraphic;
    [SerializeField] GameObject toggleOnGraphic;

    private void OnValidate()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
    }

    private void Awake()
    {
        toggle.onValueChanged.AddListener(OnToggleValueChange);
        OnToggleValueChange(toggle.isOn);
    }

    public void OnToggleValueChange(bool value)
    {
        toggleOffGraphic.SetActive(!value);
        toggleOnGraphic.SetActive(value);
    }
}
