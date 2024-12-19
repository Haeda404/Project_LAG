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

    public GameObject strategyPhaseUI; // 전략 페이즈 UI 오브젝트
    public Transform zeroSkillZone; // Zero Skill Zone 영역
    public Transform firstSkillZone; // First Skill Zone 영역
    public Transform secondSkillZone; // Second Skill Zone 영역
    public Transform thirdSkillZone; // Third Skill Zone 영역
    public Button confirmStrategyButton; // 전략 페이즈 확인 버튼

    public List<SkillCard> availableSkillCards; // 사용 가능한 스킬 카드 리스트

    private void Awake()
    {
        // 전투 맵 초기화
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
        Debug.Log("전략 페이즈 시작");
        strategyPhaseUI.SetActive(true);
        PopulateZeroSkillZone();
        confirmStrategyButton.onClick.RemoveAllListeners();
        confirmStrategyButton.onClick.AddListener(OnConfirmStrategy);
        // 적의 무작위 스킬 3개 선택
        enemySkills = GetRandomEnemySkills();
        Debug.Log("적의 스킬: " + string.Join(", ", enemySkills));
    }

    private void PopulateZeroSkillZone()
    {
        // Zero Skill Zone에 스킬 카드 배치
        foreach (SkillCard card in availableSkillCards)
        {
            SkillCard cardInstance = Instantiate(card, zeroSkillZone);
            cardInstance.Init(this);
        }
    }

    public void OnConfirmStrategy()
    {
        // 플레이어가 드래그 앤 드롭으로 설정한 스킬을 저장
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
        Debug.Log("배틀 페이즈 시작");
        StartCoroutine(ExecuteBattlePhase());
    }

    private IEnumerator ExecuteBattlePhase()
    {
        // 퍼스트 스킬들 실행
        Debug.Log("퍼스트 스킬들 실행");
        foreach (string skill in playerFirstSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // 적의 첫 번째 스킬 실행
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[0]);
        yield return new WaitForSeconds(1f);

        // 세컨드 스킬들 실행
        Debug.Log("세컨드 스킬들 실행");
        foreach (string skill in playerSecondSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // 적의 두 번째 스킬 실행
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[1]);
        yield return new WaitForSeconds(1f);

        // 서드 스킬들 실행
        Debug.Log("서드 스킬들 실행");
        foreach (string skill in playerThirdSkills)
        {
            ExecuteSkill(playerCharacter, enemyCharacter, skill);
            yield return new WaitForSeconds(1f);
        }
        // 적의 세 번째 스킬 실행
        ExecuteSkill(enemyCharacter, playerCharacter, enemySkills[2]);
        yield return new WaitForSeconds(1f);

        // 배틀 종료 후 전략 페이즈로 전환
        StartStrategyPhase();
    }

    private void ExecuteSkill(Character caster, Character target, string skill)
    {
        if (skill == "공격")
        {
            if (IsInRange(caster, target, 1))
            {
                Debug.Log(caster.name + "가 " + target.name + "에게 공격을 사용했습니다.");
                target.TakeDamage(10);
            }
            else
            {
                Debug.Log(caster.name + "의 공격이 범위를 벗어났습니다.");
            }
        }
        else if (skill == "방어")
        {
            Debug.Log(caster.name + "가 방어를 사용했습니다.");
            caster.Defend(5);
        }
        else if (skill == "강화")
        {
            Debug.Log(caster.name + "가 자신을 강화했습니다.");
            caster.Buff(5);
        }
        else if (skill == "약화")
        {
            if (IsInRange(caster, target, 2))
            {
                Debug.Log(caster.name + "가 " + target.name + "에게 약화를 사용했습니다.");
                target.Weaken(5);
            }
            else
            {
                Debug.Log(caster.name + "의 약화가 범위를 벗어났습니다.");
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
        // 적의 스킬을 무작위로 선택하는 로직 (예시로 간단히 문자열 사용)
        List<string> allSkills = new List<string> { "공격", "방어", "강화", "약화" };
        List<string> selectedSkills = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            selectedSkills.Add(allSkills[Random.Range(0, allSkills.Count)]);
        }
        return selectedSkills;
    }
}
