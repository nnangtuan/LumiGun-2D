using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Cinemachine;

public class Gun : MonoBehaviour
{
    public static Action OnShoot;
    
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _gunFireCD = 0.2f;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private float _muzzleFlashTime=0.05f;

    private Coroutine _muzzleFlashRoutine;
    private ObjectPool<Bullet> _bulletPool;
    //nhanh hon va toi uu
    private static readonly int FIRE_HASH = Animator.StringToHash("Fire");
    private Vector2 _mousePos;
    private float _lastFireTime = 0f;

    private CinemachineImpulseSource _impulseSource;
    private Animator _animator;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        CreateBulletPool();
    }
    private void Update()
    {
        Shoot();
        RotateGun();
    }
    private void OnEnable()
    {
        OnShoot += ResetLastFireTime;
        OnShoot += ShootProjectile;
        OnShoot += FireAnimation;
        OnShoot += GunSceenShake;
        OnShoot += MuzzleFlash;
    }
    private void OnDisable()
    {
        OnShoot -= ResetLastFireTime;
        OnShoot -= ShootProjectile;
        OnShoot -= FireAnimation;
        OnShoot -= GunSceenShake;
        OnShoot -= MuzzleFlash;
    }
    public void ReleaseBulletFromPool(Bullet bullet)
    {
        _bulletPool.Release(bullet);
    }
    private void CreateBulletPool()
    {
        _bulletPool = new ObjectPool<Bullet>(() =>
        {
            return Instantiate(_bulletPrefab);
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet);
        },false, 20,40);
    }
    

    private void Shoot()
    {
        if (Input.GetMouseButton(0) && Time.time>=_lastFireTime) {
            OnShoot?.Invoke();
        }
    }

    private void ShootProjectile()
    {
       
        Bullet newBullet = _bulletPool.Get();
        newBullet.Init(this,_bulletSpawnPoint.position, _mousePos);

    }
    private void FireAnimation()
    {
        _animator.Play(FIRE_HASH, 0, 0f);
    }

    private void ResetLastFireTime()
    {
        _lastFireTime = Time.time + _gunFireCD;


    }
    private void GunSceenShake()
    {
        _impulseSource.GenerateImpulse();
    }
    private void RotateGun()
    {
        _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        /*Vector2 direction = _mousePos - (Vector2)PlayerController.Instance.transform.position;*/
        Vector2 direction = PlayerController.Instance.transform.InverseTransformPoint(_mousePos);

        float angle = Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0,0,angle);
    }
    private void MuzzleFlash()
    {
        if (_muzzleFlashRoutine !=null)
        {
            StopCoroutine(_muzzleFlashRoutine);
        }

        _muzzleFlashRoutine = StartCoroutine(MuzzleFlashRoutine());
    }
    private IEnumerator MuzzleFlashRoutine()
    {
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_muzzleFlashTime);
        _muzzleFlash.SetActive(false);
    }

}
