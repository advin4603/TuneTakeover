using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackDefendTrigger : MonoBehaviour
{
    public InputActionReference attackActionReference;

    public InputActionReference defendActionReference;

    private AttackDefendInvoker _attackDefendInvoker;

    private void Attack(InputAction.CallbackContext _)
    {
        _attackDefendInvoker.Attack();
    }

    private void Defend(InputAction.CallbackContext _)
    {
        _attackDefendInvoker.Defend();
    }

    private void OnEnable()
    {
        attackActionReference.action.started += Attack;
        defendActionReference.action.started += Defend;
        _attackDefendInvoker = GetComponent<AttackDefendInvoker>();
        

        attackActionReference.action.Enable();
        defendActionReference.action.Enable();
        
    }

    private void OnDisable()
    {
        attackActionReference.action.started -= Attack;
        defendActionReference.action.started -= Defend;


        attackActionReference.action.Disable();
        defendActionReference.action.Disable();
    }
}