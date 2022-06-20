using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{

    private void Awake()
    {
        var root = this.GetComponent<UIDocument>().rootVisualElement;

        root.Q<Button>("bStart").clicked += this.StartGame;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}