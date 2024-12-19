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
        Debug.Log(name + "�� " + newX + ", " + newY + "�� �̵��߽��ϴ�.");
    }

    public void TakeDamage(int damage)
    {
        int damageTaken = Mathf.Max(damage - defense, 0);
        health -= damageTaken;
        Debug.Log(name + "�� " + damageTaken + "�� ���ظ� �Ծ����ϴ�. ���� ü��: " + health);
    }

    public void Defend(int defenseValue)
    {
        defense += defenseValue;
        Debug.Log(name + "�� ������ " + defenseValue + "��ŭ �����߽��ϴ�. ���� ����: " + defense);
    }

    public void Buff(int buffValue)
    {
        // ��ȭ ȿ�� (���÷� ���� ����)
        defense += buffValue;
        Debug.Log(name + "�� ��ȭ�Ǿ� ������ " + buffValue + "��ŭ �����߽��ϴ�. ���� ����: " + defense);
    }

    public void Weaken(int weakenValue)
    {
        // ��ȭ ȿ�� (���÷� ���� ����)
        defense = Mathf.Max(defense - weakenValue, 0);
        Debug.Log(name + "�� ��ȭ�Ǿ� ������ " + weakenValue + "��ŭ �����߽��ϴ�. ���� ����: " + defense);
    }
}
