using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDefendInvoker : MonoBehaviour
{
    public List<string> attackTriggers;
    public List<string> defendTriggers;
    List<int> attackHashes = new List<int>();
    List<int> defendHashes = new List<int>();
    private int attackCycle = 0;
    private int defendCycle = 0;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        foreach (var attackTrigger in attackTriggers)
            attackHashes.Add(Animator.StringToHash(attackTrigger));

        foreach (var defendTrigger in defendTriggers)
            defendHashes.Add(Animator.StringToHash(defendTrigger));
    }

    public void Attack()
    {
        if (attackHashes.Count <= attackCycle)
            return;
        _animator.SetTrigger(attackHashes[attackCycle]);
        attackCycle = (attackCycle + 1) % attackHashes.Count;
    }

    public void Defend()
    {
        if (defendHashes.Count <= defendCycle)
            return;
        _animator.SetTrigger(defendTriggers[defendCycle]);
        defendCycle = (defendCycle + 1) % defendHashes.Count;
    }
}