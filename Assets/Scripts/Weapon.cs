using System.Collections;
using UnityEngine;

public enum WeaponType
{
    Melee,
    HitScan,
    ProjectileWeapon,
    AreaOfEffect
}

public class Weapon : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField]
    private WeaponType WeaponType;
    [Header("Damage Settings")]
    [SerializeField] private DamageDealer _damageDealer;
    [SerializeField] private GameObject Owner;

    [Header("Hitscan Settings")]
    [SerializeField]
    private float Range = 100f;
    [SerializeField]
    private Transform RaycastOrigin;


    [Header("Projectile Settings")]
    [SerializeField]
    private GameObject BulletPrefab;
    [SerializeField]
    private Transform BulletSpawn;
    [SerializeField]
    private float BulletVelocity = 30;
    [SerializeField]
    private float BulletPrefabLifeTime = 3f;

    [Header("Melee Settings")]
    [SerializeField]
    private Collider MeleeCollider; // Drag the Box Collider here in the Inspector
    [SerializeField]
    private float AttackDuration = 0.2f; // How long the swing lasts (in seconds)


    private void Awake()
    {

    }

    public void Initialize()
    {
        if (_damageDealer != null)
        {
            if (Owner != null) 
            {
                _damageDealer.Initialize(this.Owner);
            }
            else
            {
                _damageDealer.Initialize(this.gameObject);
            } 
        }
    }

    public void Initialize(float Damage, DamageType type, GameObject source)
    {
        _damageDealer.Initialize(Damage,type,source);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack()
    {
        if (WeaponType == WeaponType.HitScan)
        {
            RaycastHit hit;
            if (Physics.Raycast(RaycastOrigin.position, RaycastOrigin.transform.forward, out hit, Range))
            {
                if (_damageDealer != null)
                {
                    _damageDealer.ApplyDamageToRaycastHit(hit);
                }
            }
        }
        else if (WeaponType == WeaponType.ProjectileWeapon)
        {
            if (BulletPrefab != null)
            {

                GameObject newBullet = GameObject.Instantiate(BulletPrefab, BulletSpawn.position, BulletSpawn.rotation);

                DamageDealer bulletDamageDealer = newBullet.GetComponent<DamageDealer>();
                if (bulletDamageDealer != null)
                {

                    bulletDamageDealer.SetDamageSource(this.gameObject);
                }

                Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = BulletSpawn.forward * BulletVelocity;
                }

                Destroy(newBullet, BulletPrefabLifeTime);
            }
        }
        else if(WeaponType == WeaponType.Melee) 
        {
            if (MeleeCollider != null && !MeleeCollider.enabled)
            {
                StartCoroutine(MeleeSwingCoroutine());
            }
        }
    }

    private IEnumerator MeleeSwingCoroutine()
    {
        // 1. Enable the collider (start of the swing)
        MeleeCollider.enabled = true;

        // 2. Wait for the duration of the attack
        yield return new WaitForSeconds(AttackDuration);

        // 3. Disable the collider (end of the swing)
        MeleeCollider.enabled = false;
    }
}
