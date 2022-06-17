using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public Label Team1Money { get; set; }
    public Button Team1Unit1Button { get; set; }

    public Label Team2Money { get; set; }

    private void Awake()
    {
        var root = this.GetComponent<UIDocument>().rootVisualElement;

        // Set team names
        root.Q<Label>("lbTeam1Name").text = GameManager.Instance.Team1.Name;
        root.Q<Label>("lbTeam2Name").text = GameManager.Instance.Team2.Name;

        // Handle money update
        this.Team1Money = root.Q<Label>("lbTeam1Money");
        GameManager.Instance.Team1.OnMoneyChanged += this.Team1_OnMoneyChanged;
        this.Team2Money = root.Q<Label>("lbTeam2Money");
        GameManager.Instance.Team2.OnMoneyChanged += this.Team2_OnMoneyChanged;

        // Handle unit creation buttons
        root.Query<Button>().ForEach(b => b.clickable.clickedWithEventInfo += this.Clickable_ClickedWithEventInfo);

        //this.Team1Unit1Button = root.Q<Button>("bTeam1Unit1");
        //this.Team1Unit1Button.clickable.clickedWithEventInfo += Clickable_ClickedWithEventInfo;
    }

    private void Team1_OnMoneyChanged(object sender, EventArgs e)
    {
        this.Team1Money.text = $"{GameManager.Instance.Team1.Money} $";
    }
    private void Team2_OnMoneyChanged(object sender, EventArgs e)
    {
        this.Team2Money.text = $"{GameManager.Instance.Team2.Money} $";
    }

    private void Clickable_ClickedWithEventInfo(EventBase obj)
    {
        Button sender = (Button)obj.target;
        Debug.Log(sender.name);
    }

    private void Update()
    {
        // TODO: move in change event
        if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Create Team 1 Unit 1");
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("Create Team 1 Unit 2");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Create Team 1 Unit 3");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Create Team 1 Unit 4");
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Create Team 1 Unit 5");
        }
    }
}
