// Character.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public string name;
    public int x;
    public int y;
    public int health;
    public int defense;

    public Character(string name, int startX, int startY)
    {
        this.name = name;
        x = startX;
        y = startY;
        health = 100;
        defense = 0;
    }

    public void Move(int newX, int newY)
    {
        x = newX;
        y = newY;
        Debug.Log(name + "가 " + newX + ", " + newY + "로 이동했습니다.");
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        health -= damageTaken;
        Debug.Log(name + "가 " + damageTaken + "의 피해를 입었습니다. 남은 체력: " + health);
    }

    public void Defend(int defenseValue)
    {
        defense += defenseValue;
        Debug.Log(name + "의 방어력이 " + defenseValue + "만큼 증가했습니다. 현재 방어력: " + defense);
    }

    public void Buff(int buffValue)
    {
        // 강화 효과 (예시로 방어력 증가)
        defense += buffValue;
        Debug.Log(name + "가 강화되어 방어력이 " + buffValue + "만큼 증가했습니다. 현재 방어력: " + defense);
    }

    public void Weaken(int weakenValue)
    {
        // 약화 효과 (예시로 방어력 감소)
        defense = Mathf.Max(defense - weakenValue, 0);
        Debug.Log(name + "가 약화되어 방어력이 " + weakenValue + "만큼 감소했습니다. 현재 방어력: " + defense);
    }
}
