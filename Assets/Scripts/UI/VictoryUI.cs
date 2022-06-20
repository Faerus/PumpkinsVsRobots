using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class VictoryUI : MonoBehaviour
{
    [field: SerializeField]
    public GameObject GameUI { get; set; }
    private VisualElement Root { get; set; }

    private Label Label { get; set; }
    private IMGUIContainer Image { get; set; }
    private Button RestartButton { get; set; }
    private Button QuitButton { get; set; }

    private void Awake()
    {
        this.Root = this.GetComponent<UIDocument>().rootVisualElement;
        this.Root.style.display = DisplayStyle.None;

        this.Label = this.Root.Q<Label>("lbVictory");
        this.Image = this.Root.Q<IMGUIContainer>("iWinner");
        this.RestartButton = this.Root.Q<Button>("bStart");
        this.QuitButton = this.Root.Q<Button>("bQuit");

        this.RestartButton.clicked += this.RestartGame;
        this.QuitButton.clicked += this.QuitGame;

        GameManager.Instance.Team1.OnTeamDead += this.Team_OnTeamDead; 
        GameManager.Instance.Team2.OnTeamDead += this.Team_OnTeamDead;
    }

    private void Team_OnTeamDead(object sender, EventArgs e)
    {
        TeamSettings loser = (TeamSettings)sender;
        TeamSettings winner = GameManager.Instance.GetEnemyTeam(loser);
        this.DisplayWinner(winner);
    }
    public void DisplayWinner(TeamSettings winner)
    {
        this.Label.text = $"{winner.Name} ont gagné !";
        this.Image.style.backgroundImage = new StyleBackground(winner.UnitTypes[0].Sprite);

        this.GameUI.SetActive(false);
        this.Root.style.display = DisplayStyle.Flex;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("Main");
    }

    private void QuitGame()
    {
        SceneManager.LoadScene("Menu");
    }
}