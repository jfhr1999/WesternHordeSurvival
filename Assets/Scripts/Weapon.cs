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

    private void Awake()
    {
        if(_damageDealer != null)
        {
            _damageDealer.Initialize(this.gameObject);
        }
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
    }
}
