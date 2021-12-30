using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using UnityEngine.InputSystem;

namespace Multiplayer.GameControls
{
    public class PlayerCameraController : NetworkBehaviour
    {    
        [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

        public override void OnStartAuthority()
        {
            virtualCamera.gameObject.SetActive(true);
            enabled = true;
        }
        
        //Do camera shake here
    }
}