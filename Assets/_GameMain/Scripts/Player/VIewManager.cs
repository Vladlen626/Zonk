using System;
using DG.Tweening;
using Mirror;
using UnityEngine;

[Serializable]
public enum View
{
    Default,
    Table,
    EnemyTable
}

public class ViewManager : NetworkBehaviour
{
    private View currentView;
    private bool viewLocked;
    
    [Header("Moveable objects")] 
    [SerializeField] private Transform cameraRig;

    [Header("TargetPositions")] 
    [SerializeField] private Transform defaultPosition;
    [SerializeField] private Transform tablePosition;
    [SerializeField] private Transform enemyTablePosition;

    [Header("CameraController")]
    [SerializeField] private PlayerCameraController _playerCameraController;

    private const float TRANSITION_SPEED = 0.25f;

    private void Start()
    {
        SwitchToView(View.Default, immediate: true);
    }
    
    public void SwitchToView(View view, bool immediate = false)
    {
        if (!isLocalPlayer) return;
        if(currentView == view) return;
        var transitionSpeed = immediate ? 0f : TRANSITION_SPEED;
        switch (view)
        {
            case View.Default:
                cameraRig.DOMove(defaultPosition.position, transitionSpeed);
                cameraRig.DORotate(defaultPosition.rotation.eulerAngles, transitionSpeed).OnComplete(() => _playerCameraController.enabled = true);
                break;
            case View.Table:
                _playerCameraController.enabled = false;
                cameraRig.DOMove(tablePosition.position, transitionSpeed);
                cameraRig.DORotate(tablePosition.rotation.eulerAngles, transitionSpeed);
                break;
            case View.EnemyTable:
                _playerCameraController.enabled = false;
                cameraRig.DOMove(enemyTablePosition.position, transitionSpeed);
                cameraRig.DORotate(enemyTablePosition.rotation.eulerAngles, transitionSpeed);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(view), view, null);
        }

        currentView = view;
    }

    [ClientRpc]
    public void ResetViewToDefault()
    {   
        SwitchToView(View.Default);
    }
    
    [ClientRpc]
    public void Shake()
    {
        var originalPos = cameraRig.position;
        if (!isLocalPlayer) return;
        cameraRig.DOShakePosition(0.15f, 0.1f, 30, 180f)
            .OnComplete(() =>
            {
                cameraRig.position = originalPos;
            });
    }

    private void Update()
    {
        if (viewLocked || !isLocalPlayer) return;

        if (Input.GetButtonDown("View Up"))
        {
            if (currentView != View.EnemyTable)
            {
                SwitchToView(currentView + 1);
            }
        }
        
        if (Input.GetButtonDown("View Down"))
        {
            if (currentView != View.Default)
            {
                SwitchToView(currentView - 1);
            }
        }
    }
}