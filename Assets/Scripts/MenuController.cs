using System;
using UnityEngine;
using UnityEngine.Events;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    public void ChangeMenuVisibility(bool focused)
    {
        menu.SetActive(!focused);
    }

    private void Awake()
    {
        menu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
