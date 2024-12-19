// GameManager.cs (수정된 부분)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        MapExploration,
        Battle,
        Shop,
        Event,
        Treasure,
        Rest,
        BossBattle,
        EliteBattle
    }

    public GameState currentState;
    public Canvas mainMenuCanvas;
    public Canvas mapCanvas;
    public Canvas battleCanvas;
    public Canvas shopCanvas;
    public Canvas eventCanvas;
    public Canvas treasureCanvas;
    public Canvas restCanvas;
    public Canvas bossCanvas;
    public Canvas eliteCanvas;

    void Start()
    {
        ChangeState(GameState.MainMenu);
    }

    public void ChangeState(GameState newState)
    {
        Canvas previousCanvas = GetCurrentCanvas(); // 이전 Canvas를 저장
        currentState = newState; // 상태를 변경

        switch (currentState)
        {
            case GameState.MainMenu:
                SwitchCanvas(previousCanvas, mainMenuCanvas);
                break;
            case GameState.MapExploration:
                SwitchCanvas(previousCanvas, mapCanvas);
                break;
            case GameState.Battle:
                StartBattleStage();
                SwitchCanvas(previousCanvas, battleCanvas);
                break;
            case GameState.Shop:
                OpenShop();
                SwitchCanvas(previousCanvas, shopCanvas);
                break;
            case GameState.Event:
                StartEvent();
                SwitchCanvas(previousCanvas, eventCanvas);
                break;
            case GameState.Treasure:
                OpenTreasure();
                SwitchCanvas(previousCanvas, treasureCanvas);
                break;
            case GameState.Rest:
                StartRestStage();
                SwitchCanvas(previousCanvas, restCanvas);
                break;
            case GameState.BossBattle:
                StartBossBattle();
                SwitchCanvas(previousCanvas, bossCanvas);
                break;
            case GameState.EliteBattle:
                StartEliteBattleStage();
                SwitchCanvas(previousCanvas, eliteCanvas);
                break;
        }
    }

    private Canvas GetCurrentCanvas()
    {
        switch (currentState)
        {
            case GameState.MainMenu:
                return mainMenuCanvas;
            case GameState.MapExploration:
                return mapCanvas;
            case GameState.Battle:
                return battleCanvas;
            case GameState.Shop:
                return shopCanvas;
            case GameState.Event:
                return eventCanvas;
            case GameState.Treasure:
                return treasureCanvas;
            case GameState.Rest:
                return restCanvas;
            case GameState.BossBattle:
                return bossCanvas;
            case GameState.EliteBattle:
                return eliteCanvas;
            default:
                return null;
        }
    }

    private void SwitchCanvas(Canvas currentCanvas, Canvas nextCanvas)
    {
        if (currentCanvas != null)
        {
            Debug.Log($"Deactivating Canvas: {currentCanvas.name}");
            currentCanvas.gameObject.SetActive(false);
        }
        if (nextCanvas != null)
        {
            Debug.Log($"Activating Canvas: {nextCanvas.name}");
            nextCanvas.gameObject.SetActive(true);
        }
    }


    public void OnGameStartButtonClicked()
    {
        RandomMapGenerator mapGenerator = FindObjectOfType<RandomMapGenerator>();
        if (mapGenerator != null)
        {
            mapGenerator.GenerateRandomMap();
        }
        ChangeState(GameState.MapExploration);
    }

    private void StartBattleStage()
    {
        int randomBattleIndex = Random.Range(0, 3);
        switch (randomBattleIndex)
        {
            case 0:
                SetupBattleScenario1();
                break;
            case 1:
                SetupBattleScenario2();
                break;
            case 2:
                SetupBattleScenario3();
                break;
        }
    }

    private void StartEliteBattleStage()
    {
        int randomEliteBattleIndex = Random.Range(0, 2);
        switch (randomEliteBattleIndex)
        {
            case 0:
                SetupEliteBattleScenario1();
                break;
            case 1:
                SetupEliteBattleScenario2();
                break;
        }
    }

    private void OpenShop()
    {
        int randomShopIndex = Random.Range(0, 2);
        switch (randomShopIndex)
        {
            case 0:
                SetupShopScenario1();
                break;
            case 1:
                SetupShopScenario2();
                break;
        }
    }

    private void StartEvent()
    {
        int randomEventIndex = Random.Range(0, 3);
        switch (randomEventIndex)
        {
            case 0:
                TriggerEvent1();
                break;
            case 1:
                TriggerEvent2();
                break;
            case 2:
                TriggerEvent3();
                break;
        }
    }

    private void OpenTreasure()
    {
        // 보물 상자 보상 처리 로직
        SetupTreasureScenario();
    }

    private void StartRestStage()
    {
        int randomRestIndex = Random.Range(0, 2);
        switch (randomRestIndex)
        {
            case 0:
                SetupRestScenario1();
                break;
            case 1:
                SetupRestScenario2();
                break;
        }
    }

    private void StartBossBattle()
    {
        SetupBossBattleScenario();
    }



    // 전투, 상점, 이벤트 등의 개별 시나리오 설정 메서드들 추가 구현 필요
    private void SetupBattleScenario1() { /* 전투 시나리오 1 설정 */ }
    private void SetupBattleScenario2() { /* 전투 시나리오 2 설정 */ }
    private void SetupBattleScenario3() { /* 전투 시나리오 3 설정 */ }
    private void SetupEliteBattleScenario1() { /* 엘리트 전투 시나리오 1 설정 */ }
    private void SetupEliteBattleScenario2() { /* 엘리트 전투 시나리오 2 설정 */ }
    private void SetupShopScenario1() { /* 상점 시나리오 1 설정 */ }
    private void SetupShopScenario2() { /* 상점 시나리오 2 설정 */ }
    private void TriggerEvent1() { /* 이벤트 1 실행 */ }
    private void TriggerEvent2() { /* 이벤트 2 실행 */ }
    private void TriggerEvent3() { /* 이벤트 3 실행 */ }
    private void SetupTreasureScenario() { /* 보물 상자 보상 설정 */ }
    private void SetupRestScenario1() { /* 휴식 시나리오 1 설정 */ }
    private void SetupRestScenario2() { /* 휴식 시나리오 2 설정 */ }
    private void SetupBossBattleScenario() { /* 보스 전투 설정 */ }
}
