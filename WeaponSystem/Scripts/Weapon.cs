using UnityEngine;

public class Weapon : MonoBehaviour
{
    private enum FireMode
    {
        Auto,
        Semi_Auto
    }

    private enum ReloadMode
    {
        FastReload,
        Manual
    }

    [SerializeField] private FireMode fireMode;
    [SerializeField] private ReloadMode reloadMode;

    [SerializeField] private int magazineSize;

    [SerializeField] private float maxRange;
    [SerializeField] private float fireRate;
    [SerializeField, Tooltip("If in semi-auto mode, this is the time to reload one bullet.")] private float reloadTime;

    [SerializeField] private Transform firePoint;
    [SerializeField] private LayerMask shootableLayer;

    private int magazine;

    private float fireTimer;
    private float reloadTimer;

    private void Start()
    {
        magazine = magazineSize;
    }

    private void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= 1 / fireRate)
                fireTimer = 0;
        }

        if (reloadTimer > 0)
        {
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= reloadTime)
            {
                reloadTimer = 0;
                Reload();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            StartReload();

        if (reloadTimer == 0)
        {
            switch (fireMode)
            {
                case FireMode.Auto:
                    if (Input.GetMouseButton(0))
                        AutoFire();
                    break;

                case FireMode.Semi_Auto:
                    if (Input.GetMouseButtonDown(0))
                        ShootOnes();
                    break;
            }
        }
    }

    private void ShootOnes()
    {
        if (magazine > 0)
        {
            magazine -= 1;
            Debug.Log(magazine);
            Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, maxRange, shootableLayer);

            if (hit.collider != null)
            {
                // Hit logic
            }
        }
        else
            StartReload();
    }

    private void AutoFire()
    {
        if (reloadTimer == 0 && magazine == 0)
        {
            StartReload();
            return;
        }

        if (fireTimer == 0)
        {
            ShootOnes();
            fireTimer += Time.deltaTime;
        }
    }

    private void StartReload()
    {
        if (magazine < magazineSize)
            reloadTimer += Time.deltaTime;
        else
            Debug.Log("magazine full");
    }

    private void Reload()
    {
        switch (reloadMode)
        {
            case ReloadMode.FastReload:
                    magazine = magazineSize;
                break;
            case ReloadMode.Manual:
                magazine += 1;
                if (magazine < magazineSize)
                    reloadTimer += Time.deltaTime;
                break;
        }
    }
}