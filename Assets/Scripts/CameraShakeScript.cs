using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeScript : MonoBehaviour
{

    public float defaultShakeAmount;
    public float defaultShakeSpeed;
    public float defaultShakeTime;

    private float shakeAmount;
    private float shakeSpeed;

    private uint storedFrameCount;
    public bool shaking = false;

    private float FOV;

    public float cameraZoom = 1f;
    public float cameraZoomSpeed = 5f;

    private CinemachineVirtualCamera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
        FOV = cam.m_Lens.FieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (shaking)
        {
            cam.m_Lens.Dutch = Mathf.Sin((Time.frameCount- storedFrameCount)*shakeSpeed)*shakeAmount;
        }
        else if (cam.m_Lens.Dutch!=0)
        {
            cam.m_Lens.Dutch = Mathf.Lerp(cam.m_Lens.Dutch, 0f, Time.deltaTime);
        }

        if (cam.m_Lens.FieldOfView != FOV * cameraZoom)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(cam.m_Lens.FieldOfView, FOV * cameraZoom, cameraZoomSpeed* Time.deltaTime);
        }
    }

    public void CameraShake()
    {
        shakeAmount = defaultShakeAmount;
        shakeSpeed = defaultShakeSpeed;
        StartCoroutine(CameraShaking(defaultShakeTime));
    }

    public void CameraShake(float amount, float speed, float time)
    {
        shakeAmount = amount;
        shakeSpeed = speed;
        StartCoroutine(CameraShaking(time));
    }

    public IEnumerator CameraShaking(float time)
    {
        storedFrameCount = (uint)Time.frameCount;
        shaking = true;
        yield return new WaitForSeconds(time);
        shaking = false;
        cameraZoom = 1f;
    }

}
