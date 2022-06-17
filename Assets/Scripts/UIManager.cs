using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        // Set team units
        root.Query<Label>().ForEach(l => this.SetLabel(l));

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

    private void SetLabel(Label label)
    {
        var match = Regex.Match(label.name, "lbT([0-9])U([0-9])([a-zA-Z]*)");
        if(!match.Success)
        {
            return;
        }

        int teamId = Convert.ToInt32(match.Groups[1].Value);
        int unitTypeId = Convert.ToInt32(match.Groups[2].Value);
        string info = match.Groups[3].Value;

        TeamSettings team = teamId < 2 ? GameManager.Instance.Team1 : GameManager.Instance.Team2;
        UnitTypeSettings unitType = null;
        switch(unitTypeId)
        {
            case 1: unitType = team.UnitType1; break;
            case 2: unitType = team.UnitType2; break;
            case 3: unitType = team.UnitType3; break;
            case 4: unitType = team.UnitType4; break;
            case 5: unitType = team.UnitType5; break;
        }

        string text = null;
        switch(info)
        {
            case "Key": text = unitType.Shortcut.ToString(); break;
            case "Cost": text = $"{unitType.Cost}$"; break;
            case "Name": text = unitType.Name; break;
        }

        label.text = text;
    }

    private void Clickable_ClickedWithEventInfo(EventBase obj)
    {
        Button sender = (Button)obj.target;
        Debug.Log(sender.name);
    }

    private void Update()
    {
        // TODO: move in change event
        if(Input.GetKeyDown(GameManager.Instance.Team1.UnitType1.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team1.UnitType1.Name);
        }
        else if(Input.GetKeyDown(GameManager.Instance.Team1.UnitType2.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team1.UnitType2.Name);
        }
        else if(Input.GetKeyDown(GameManager.Instance.Team1.UnitType3.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team1.UnitType3.Name);
        }
        else if(Input.GetKeyDown(GameManager.Instance.Team1.UnitType4.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team1.UnitType4.Name);
        }
        else if(Input.GetKeyDown(GameManager.Instance.Team1.UnitType5.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team1.UnitType5.Name);
        }
        
        if (Input.GetKeyDown(GameManager.Instance.Team2.UnitType1.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team2.UnitType1.Name);
        }
        else if (Input.GetKeyDown(GameManager.Instance.Team2.UnitType2.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team2.UnitType2.Name);
        }
        else if (Input.GetKeyDown(GameManager.Instance.Team2.UnitType3.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team2.UnitType3.Name);
        }
        else if (Input.GetKeyDown(GameManager.Instance.Team2.UnitType4.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team2.UnitType4.Name);
        }
        else if (Input.GetKeyDown(GameManager.Instance.Team2.UnitType5.Shortcut))
        {
            Debug.Log(GameManager.Instance.Team2.UnitType5.Name);
        }
    }
}
