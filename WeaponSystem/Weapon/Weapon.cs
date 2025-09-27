using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    #region Variables
    [SerializeField, Required] private SO_Weapon so_WeaponData;

    private ushort magazine;

    private Timer shootCooldownTimer;
    private Timer reloadTimer;

    private bool canShoot = true;
    private bool wantsToShoot = false;
    private bool cancelReload = false;

    private delegate void ReloadFunction();
    private ReloadFunction reloadFunction;

    #endregion Variables

    #region Unity Functions

    private void Awake()
    {
        shootCooldownTimer = TimerManager.CreateTimer(1 / so_WeaponData.stats.fireRate, OnFinishedShootCooldownTimer);
        reloadTimer = TimerManager.CreateTimer(so_WeaponData.stats.reloadTime, OnFinishedReload);

        magazine = so_WeaponData.stats.magazineSize;

        reloadFunction = so_WeaponData.automaticReload ? AutomaticReload : SemiAutomaticReload;
    }

    private void OnEnable()
    {
        canShoot = true;
    }

    private void OnDisable()
    {
        shootCooldownTimer.StopTimer();
        reloadTimer.StopTimer();
    }

    #endregion Unity Functions

    #region Shoot

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
            StartShooting();
        else if (context.canceled)
            StopShooting();
    }

    private void StartShooting()
    {
        wantsToShoot = true;

        if (!so_WeaponData.automaticReload && reloadTimer.IsActive())
        {
            cancelReload = true;
            return;
        }

        if (magazine == 0)
        {
            Reload();
            return;
        }

        if (canShoot && magazine > 0 && !reloadTimer.IsActive())
            ShootOnes();
    }

    private void StopShooting()
        => wantsToShoot = false;

    private void ShootOnes()
    {
        magazine--;
        canShoot = false;
        shootCooldownTimer.StartTimerAtTheBeginning();
    }

    private void OnFinishedShootCooldownTimer()
    {
        canShoot = true;

        if (wantsToShoot && so_WeaponData.automaticFire)
            StartShooting();
    }

    #endregion Shoot

    #region Reload

    private void CallReload(InputAction.CallbackContext context)
    {
        Reload();
    }

    private void Reload()
    {
        if (!reloadTimer.IsActive())
        {
            shootCooldownTimer.StopTimer();
            canShoot = false;
            reloadTimer.StartTimerAtTheBeginning();
        }
    }

    private void OnFinishedReload()
    {
        canShoot = true;
        reloadFunction();
    }

    private void AutomaticReload()
    {
        magazine = so_WeaponData.stats.magazineSize;
        if (wantsToShoot)
            StartShooting();
    }

    private void SemiAutomaticReload()
    {
        magazine++;

        if (magazine < so_WeaponData.stats.magazineSize && !cancelReload)
            Reload();

        cancelReload = false;
    }

    #endregion Reload
}