using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    public enum GunType
    {
        Normal,
        Burst
    }

    [Header("Action Map")]
    [SerializeField] private InputActionAsset actionMap;

    [Header("Muzzle Flash Settings")]
    [SerializeField] private GameObject muzzleFlashPrefab;
    [SerializeField] private Transform muzzleFlashTransform;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bulletSound;

    [Header("Gun Settings")]
    [SerializeField] private GunType gunType;
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float delayBetweenBullets = 0.075f;

    [Header("Burst Settings")]
    [SerializeField] private int burstSize = 3;
    [SerializeField] private float burstCooldown = 0.3f;
    [SerializeField] private bool requireRetriggerForBurst = true;

    [Header("Reload Settings")]
    [SerializeField] private bool autoReload = true;
    [SerializeField] private float reloadTime = 2f;

    [Header("Shooting Raycast Settings")]
    [SerializeField] private float fireDistance = 100f;
    [SerializeField] private float fireSpread = 1.0f;
    [SerializeField] private LayerMask hitLayers;

    private InputAction shootAction;
    private InputAction reloadAction;

    private int currentBulletsInMagazine;
    private bool isTryingToShoot;
    private bool canShoot = true;
    private bool isReloading;
    private bool isBursting;

    private void Awake()
    {
        InitialiseInputActions();
        currentBulletsInMagazine = magazineSize;
    }

    private void Update()
    {
        if (isTryingToShoot && canShoot && !isReloading)
        {
            StartCoroutine(ShootSequence());
        }
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        isTryingToShoot = true;
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        isTryingToShoot = false;
    }

    private void OnReloadPerformed(InputAction.CallbackContext context)
    {
        AttemptReload();
    }

    private void InitialiseInputActions()
    {
        InputActionMap gunActionMap = actionMap.FindActionMap("Gun");
        shootAction = gunActionMap.FindAction("Shoot");
        reloadAction = gunActionMap.FindAction("Reload");

        shootAction.performed += OnShootPerformed;
        shootAction.canceled += OnShootCanceled;
        reloadAction.performed += OnReloadPerformed;

        shootAction.Enable();
        reloadAction.Enable();
    }

    private void UnsubscribeFromInputActions()
    {
        shootAction.performed -= OnShootPerformed;
        shootAction.canceled -= OnShootCanceled;
        reloadAction.performed -= OnReloadPerformed;
    }

    private void OnDestroy()
    {
        UnsubscribeFromInputActions();
    }

    private bool CheckIfGunCanShoot()
    {
        if (currentBulletsInMagazine <= 0)
            return false;

        if (isReloading)
            return false;

        return true;
    }

    private Vector3 GetRandomShotDirection()
    {
        Vector3 forward = muzzleFlashTransform.forward;

        float spreadX = Random.Range(-fireSpread, fireSpread);
        float spreadY = Random.Range(-fireSpread, fireSpread);

        Quaternion spreadRotation = Quaternion.Euler(spreadX, spreadY, 0f);

        return spreadRotation * forward;
    }

    private void PerformRaycastShot()
    {
        Vector3 direction = GetRandomShotDirection();

        RaycastHit hit;
        bool hitSomething = Physics.Raycast(muzzleFlashTransform.position, direction, out hit, fireDistance, hitLayers);

        Color lineColor = hitSomething ? Color.red : Color.green;
        Debug.DrawRay(muzzleFlashTransform.position, direction * fireDistance, lineColor, 1f);

        if (hitSomething)
        {
            //Hit logic
        }
    }

    private IEnumerator ReloadSequence()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentBulletsInMagazine = magazineSize;
        isReloading = false;
    } 

    private void AttemptReload()
    {
        if (isReloading || currentBulletsInMagazine >= magazineSize)
            return;

        StartCoroutine(ReloadSequence());
    }

    private void TryAutoReload()
    {
        if (currentBulletsInMagazine <= 0 && autoReload && !isReloading)
        {
            StartCoroutine(ReloadSequence());
        }
    }

    private void PlayMuzzleFlash()
    {
        GameObject flashInstance = Instantiate(muzzleFlashPrefab, muzzleFlashTransform);
        Destroy(flashInstance, Mathf.Min(delayBetweenBullets, 0.03f));
    }

    private void ShootGun()
    {
        audioSource.PlayOneShot(bulletSound);
        PlayMuzzleFlash();
        currentBulletsInMagazine--;

        TryAutoReload();

        PerformRaycastShot();
    }

    private IEnumerator NormalShootRoutine()
    {
        if (CheckIfGunCanShoot())
        {
            ShootGun();
            yield return new WaitForSeconds(delayBetweenBullets);
        }
    }

    private IEnumerator BurstShootRoutine()
    {
        if (isBursting)
            yield break;

        isBursting = true;

        for (int i = 0; i < burstSize; i++)
        {
            if (!CheckIfGunCanShoot())
                break;

            ShootGun();
            yield return new WaitForSeconds(delayBetweenBullets);
        }

        if (requireRetriggerForBurst)
        {
            yield return new WaitUntil(() => !isTryingToShoot);
        }

        yield return new WaitForSeconds(burstCooldown);
        isBursting = false;
    }

    private IEnumerator ShootSequence()
    {
        canShoot = false;

        switch (gunType)
        {
            case GunType.Normal:
                yield return NormalShootRoutine();
                break;

            case GunType.Burst:
                yield return BurstShootRoutine();
                break;
        }

        canShoot = true;
    }
}
