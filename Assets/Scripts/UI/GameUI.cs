using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private const string CLASS_BUTTON_DISABLED = "button-disabled";
    private const string CLASS_LOADER = "loader";

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

        // Initialize labels & buttons
        root.Query<Label>().ForEach(l => this.InitializeLabel(l));
        root.Query<Button>().ForEach(b => this.InitializeButton(b));
    }

    private void Team1_OnMoneyChanged(object sender, EventArgs e)
    {
        this.Team1Money.text = $"{GameManager.Instance.Team1.Money} $";
        this.RefreshButtonEnabled(GameManager.Instance.Team1);
    }
    private void Team2_OnMoneyChanged(object sender, EventArgs e)
    {
        this.Team2Money.text = $"{GameManager.Instance.Team2.Money} $";
        this.RefreshButtonEnabled(GameManager.Instance.Team2);
    }

    private bool TryParse(VisualElement control, out TeamSettings team, out UnitTypeSettings unitType, out string info)
    {
        var match = Regex.Match(control.name, "[a-z]+T([0-9])U([0-9])([a-zA-Z]*)");
        if (match.Success)
        {
            int teamId = Convert.ToInt32(match.Groups[1].Value);
            int unitTypeId = Convert.ToInt32(match.Groups[2].Value);
            info = match.Groups[3].Value;

            team = teamId < 2 ? GameManager.Instance.Team1 : GameManager.Instance.Team2;
            unitType = team.UnitTypes.ElementAt(unitTypeId - 1);
        }
        else
        {
            team = null;
            unitType = null;
            info = null;
        }

        return match.Success;
    }
    private void InitializeLabel(Label label)
    {
        if(this.TryParse(label, out TeamSettings team, out UnitTypeSettings unitType, out string info))
        {
            switch (info)
            {
                case "Key": label.text = unitType.Shortcut.ToString(); break;
                case "Cost": label.text = $"{unitType.Cost}$"; break;
                case "Name": label.text = unitType.Name; break;
            }
        }
    }
    private void InitializeButton(Button button)
    {
        button.clickable.clickedWithEventInfo += this.Clickable_ClickedWithEventInfo;
        if (this.TryParse(button, out TeamSettings team, out UnitTypeSettings unitType, out string info))
        {
            unitType.Button = button;
            if(team.Money < unitType.Cost)
            {
                button.AddToClassList(CLASS_BUTTON_DISABLED);
            }
        }
    }
    private void RefreshButtonEnabled(TeamSettings team)
    {
        foreach(UnitTypeSettings unitType in team.UnitTypes)
        {
            if (team.Money < unitType.Cost)
            {
                unitType.Button.AddToClassList(CLASS_BUTTON_DISABLED);
            }
            else
            {
                unitType.Button.RemoveFromClassList(CLASS_BUTTON_DISABLED);
            }
        }
    }

    private void Clickable_ClickedWithEventInfo(EventBase obj)
    {
        Button sender = (Button)obj.target;
        if (this.TryParse(sender, out TeamSettings team, out UnitTypeSettings unitType, out string info))
        {
            this.Build(team, unitType);
        }
    }

    private void Update()
    {
        this.CheckTeamShortcuts(GameManager.Instance.Team1);
        this.CheckTeamShortcuts(GameManager.Instance.Team2);

        this.UpdateTeamCooldowns(GameManager.Instance.Team1);
        this.UpdateTeamCooldowns(GameManager.Instance.Team2);
    }
    private void CheckTeamShortcuts(TeamSettings team)
    {
        var unitType = team.UnitTypes
            .OrderByDescending(u => u.Cost)
            .FirstOrDefault(u => team.Money >= u.Cost && Input.GetKeyDown(u.Shortcut));

        if (unitType != null)
        {
            this.Build(team, unitType);
        }
    }
    private void UpdateTeamCooldowns(TeamSettings team)
    {
        foreach(UnitTypeSettings unitType in team.UnitTypes)
        {
            if (unitType.CanBuildAt0 <= 0)
            {
                continue;
            }

            unitType.CanBuildAt0 -= Time.deltaTime;
            if (unitType.CanBuildAt0 <= 0)
            {
                unitType.CanBuildAt0 = 0;
            }

            var loader = unitType.Button.Children().First(c => c.ClassListContains(CLASS_LOADER));
            loader.style.height = unitType.CanBuildAt0 / unitType.CoolDown * 100;
        }
    }

    private void Build(TeamSettings team, UnitTypeSettings unitType)
    {
        if(!unitType.CanBuild(team))
        {
            return;
        }

        var gameObject = Instantiate(AssetManager.Instance.Unit, team.Spawn);
        gameObject.name = $"{unitType.Name}{++team.UnitCounter}";

        float spawnVariance = UnityEngine.Random.Range(-0.5f, 0.5f);
        gameObject.transform.position = team.Spawn.position + new Vector3(0, spawnVariance, spawnVariance);

        Unit unit = gameObject.GetComponent<Unit>();
        unit.Initialize(team, unitType);

        // Pay the cost
        team.Money -= unitType.Cost;
        // Start cooldown
        unitType.StartCooldown();
    }
}
