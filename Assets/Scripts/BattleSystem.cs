// BattleSystem.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleSystem : MonoBehaviour
{
    public int mapWidth = 5;
    public int mapHeight = 5;
    public BattleTile[,] battleMap;
    public Character playerCharacter;
    public Character enemyCharacter;

    public List<string> enemySkills = new List<string>();
    public List<string> playerFirstSkills = new List<string>();
    public List<string> playerSecondSkills = new List<string>();
    public List<string> playerThirdSkills = new List<string>();

    public GameObject strategyPhaseUI; // ���� ������ UI ������Ʈ
    public Transform zeroSkillZone; // Zero Skill Zone ����
    public Transform firstSkillZone; // First Skill Zone ����
    public Transform secondSkillZone; // Second Skill Zone ����
    public Transform thirdSkillZone; // Third Skill Zone ����
    public Button confirmStrategyButton; // ���� ������ Ȯ�� ��ư

    public List<SkillCard> availableSkillCards; // ��� ������ ��ų ī�� ����Ʈ

    private void Awake()
    {
        // ���� �� �ʱ�ȭ
        battleMap = new BattleTile[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                battleMap[x, y] = new BattleTile(x, y);
            }
        }
    }

    public void StartStrategyPhase()
    {
        Debug.Log("���� ������ ����");
        strategyPhaseUI.SetActive(true);
        PopulateZeroSkillZone();
        confirmStrategyButton.onClick.RemoveAllListeners();
        confirmStrategyButton.onClick.AddListener(OnConfirmStrategy);
        // ���� ������ ��ų 3�� ����
        enemySkills = GetRandomEnemySkills();
        Debug.Log("���� ��ų: " + string.Join(", ", enemySkills));
    }

    private void PopulateZeroSkillZone()
    {
        // Zero Skill Zone�� ��ų ī�� ��ġ
        foreach (SkillCard card in availableSkillCards)
        {
            SkillCard cardInstance = Instantiate(card, zeroSkillZone);
            cardInstance.Init(this);
        }
    }

    public void OnConfirmStrategy()
    {
        // �÷��̾ �巡�� �� ������� ������ ��ų�� ����
        playerFirstSkills.Clear();
        playerSecondSkills.Clear();
        playerThirdSkills.Clear();

        foreach (Transform child in firstSkillZone)
        {
            SkillCard skillCard = child.GetComponent<SkillCard>();
            if (skillCard != null)
            {
                playerFirstSkills.Add(skillCard.skillName);
            }
        }

        foreach (Transform child in secondSkillZone)
        {
            SkillCard skillCard = child.GetComponent<SkillCard>();
            if (skillCard != null)
            {
                playerSecondSkills.Add(skillCard.skillName);
            }
        }

        foreach (Transform child in thirdSkillZone)
        {
            SkillCard skillCard = child.GetComponent<SkillCard>();
            if (skillCard != null)
            {
                playerThirdSkills.Add(skillCard.skillName);
            }
        }

        strategyPhaseUI.SetActive(false);
        StartBattlePhase();
    }

    public void StartBattlePhase()
    {
        Debug.Log("��Ʋ ������ ����");
        StartCoroutine(ExecuteBattlePhase());
    }

    private IEnumerator ExecuteBattlePhase()
    {
        // �۽�Ʈ ��ų�� ����
        Debug.Log("�۽�Ʈ ��ų�� ����");
        foreach (string skill in playerFirstSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // ���� ù ��° ��ų ����
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[0]);
        yield return new WaitForSeconds(1f);

        // ������ ��ų�� ����
        Debug.Log("������ ��ų�� ����");
        foreach (string skill in playerSecondSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // ���� �� ��° ��ų ����
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[1]);
        yield return new WaitForSeconds(1f);

        // ���� ��ų�� ����
        Debug.Log("���� ��ų�� ����");
        foreach (string skill in playerThirdSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // ���� �� ��° ��ų ����
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[2]);
        yield return new WaitForSeconds(1f);

        // ��Ʋ ���� �� ���� ������� ��ȯ
        StartStrategyPhase();
    }

    private void ExecuteSkill(Character caster, Character target, string skill)
    {
        if (skill == "����")
        {
            if (IsInRange(caster, target, 1))
            {
                Debug.Log(caster.name + "�� " + target.name + "���� ������ ����߽��ϴ�.");
                target.TakeDamage(10);
            }
            else
            {
                Debug.Log(caster.name + "�� ������ ������ ������ϴ�.");
            }
        }
        else if (skill == "���")
        {
            Debug.Log(caster.name + "�� �� ����߽��ϴ�.");
            caster.Defend(5);
        }
        else if (skill == "��ȭ")
        {
            Debug.Log(caster.name + "�� �ڽ��� ��ȭ�߽��ϴ�.");
            caster.Buff(5);
        }
        else if (skill == "��ȭ")
        {
            if (IsInRange(caster, target, 2))
            {
                Debug.Log(caster.name + "�� " + target.name + "���� ��ȭ�� ����߽��ϴ�.");
                target.Weaken(5);
            }
            else
            {
                Debug.Log(caster.name + "�� ��ȭ�� ������ ������ϴ�.");
            }
        }
    }

    private bool IsInRange(Character caster, Character target, int range)
    {
        int distance = Mathf.Abs(caster.x - target.x) + Mathf.Abs(caster.y - target.y);
        return distance <= range;
    }

    private List<string> GetRandomEnemySkills()
    {
        // ���� ��ų�� �������� �����ϴ� ���� (���÷� ������ ���ڿ� ���)
        List<string> allSkills = new List<string> { "����", "���", "��ȭ", "��ȭ" };
        List<string> selectedSkills = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            selectedSkills.Add(allSkills[Random.Range(0, allSkills.Count)]);
        }
        return selectedSkills;
    }
}
