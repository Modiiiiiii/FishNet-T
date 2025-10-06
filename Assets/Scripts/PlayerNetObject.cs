using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerNetObject : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Vector2 _moveInput;
    private PlayerInput _playerInput;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        // 只有本地客户端才启用输入
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
        
        _playerInput = GetComponent<PlayerInput>();
        
        // 设置回调函数
        _playerInput.actions["Move"].performed += OnMove;
        _playerInput.actions["Move"].canceled += OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 确保只有Owner才能处理输入
        if (!IsOwner) return;
        
        _moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        // 双重检查，确保只有Owner才执行移动
        if (!IsOwner) return;
        
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        Vector3 movement = new Vector3(_moveInput.x, 0f, _moveInput.y);
        
        // 应用移动
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        
        // 如果移动方向不为零，让角色面向移动方向
        /*if (movement.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(movement.normalized);
        }*/
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        
        // 清理回调函数
        if (_playerInput != null)
        {
            _playerInput.actions["Move"].performed -= OnMove;
            _playerInput.actions["Move"].canceled -= OnMove;
        }
    }
}