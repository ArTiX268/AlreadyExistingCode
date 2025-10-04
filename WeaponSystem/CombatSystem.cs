using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatSystem : MonoBehaviour
{
    [Title("Interaction")]
    [SerializeField] private float maxPickUpDistance;
    [SerializeField] private float maxPickUpRadius;
    [SerializeField] private LayerMask interactableLayerMask;

    [Title("Weapon")]
    [SerializeField, ChildGameObjectsOnly, Required] private Transform weaponHolder;

    [Title("Inventory")]
    [SerializeField] private AmmoInventory.InitialReserve[] initialReserves;

    private const float DROPPING_DISTANCE = 2;

    private Weapon currentWeapon;
    private WeaponInventory weaponInventory;
    private AmmoInventory ammoInventory;

    private void Awake()
    {
        weaponInventory = new();
        ammoInventory = new AmmoInventory(initialReserves);
    }

    private void Start()
    {
        InputManager.Instance.AssignInput(InputManager.EAction.Interact, Interact, InputManager.EventType.Started);
    }

    private void Interact(InputAction.CallbackContext pContext)
    {
        //Check if there is a weapon in front of the camera
        if (Physics.SphereCast(origin: Camera.main.transform.position, radius: maxPickUpRadius, direction: Camera.main.transform.forward, out RaycastHit pHitInfo, maxPickUpDistance, interactableLayerMask))
        {
            if (pHitInfo.collider.TryGetComponent(out WeaponManager pWeaponManager))
            {
                if (currentWeapon != null && weaponInventory.IsFull())
                    return;

                // Take the weapon
                pWeaponManager.transform.SetParent(weaponHolder);
                pWeaponManager.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                currentWeapon = pWeaponManager.TakeWeapon();
                currentWeapon.SetAmmoInventory(ammoInventory);
                InputManager.Instance.AssignInput(InputManager.EAction.Drop, DropWeapon, InputManager.EventType.Started);
                InputManager.Instance.AssignInput(InputManager.EAction.SelectInventorySlot, CheckInventorySlot, InputManager.EventType.Started);
            }
        }
    }

    private void DropWeapon(InputAction.CallbackContext pContext)
    {
        if (currentWeapon != null)
        {
            WeaponManager lWeaponManager = currentWeapon.GetComponentInParent<WeaponManager>();

            lWeaponManager.transform.SetParent(null);
            lWeaponManager.transform.position = transform.position + (transform.forward * DROPPING_DISTANCE);
            lWeaponManager.TogglePickUp(true);

            currentWeapon = null;
            InputManager.Instance.UnassignInput(InputManager.EAction.Drop, DropWeapon, InputManager.EventType.Started);

            if (weaponInventory.IsEmpty())
                InputManager.Instance.UnassignInput(InputManager.EAction.SelectInventorySlot, CheckInventorySlot, InputManager.EventType.Started);
        }
    }

    private void CheckInventorySlot(InputAction.CallbackContext pContext)
    {
        int lCheckedSlot = pContext.action.GetBindingIndexForControl(pContext.control);

        if (currentWeapon != null)
        {
            if (weaponInventory.IsSlotEmpty(lCheckedSlot))
            {
                weaponInventory.StoreWeapon(currentWeapon.GetComponentInParent<WeaponManager>(), lCheckedSlot);
                currentWeapon = null;
            }
            else
            {
                WeaponManager lWeaponManager = weaponInventory.TakeWeaponManager(lCheckedSlot);
                weaponInventory.StoreWeapon(currentWeapon.GetComponentInParent<WeaponManager>(), lCheckedSlot);
                currentWeapon = lWeaponManager.TakeWeapon();
            }
        }
        else
        {
            if (!weaponInventory.IsSlotEmpty(lCheckedSlot))
                currentWeapon = weaponInventory.TakeWeaponManager(lCheckedSlot).TakeWeapon();
        }
    }
}