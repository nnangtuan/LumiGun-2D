using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using Unity.VisualScripting;

public class Gnerade : MonoBehaviour
{
    public Action OnExplode;
    public Action OnBeep;

    [SerializeField] private GameObject _explodeVFX;
    [SerializeField] private GameObject _grenadeLight;

    [SerializeField] private float _launchForce=15f;
    [SerializeField] private float _torqueAmoust = 0.2f;

    [SerializeField] private float _explosionRadius = 3.5f;
    [SerializeField] private LayerMask _enemyLayerMask;
    [SerializeField] private int _dameAmount = 3;
    [SerializeField] private float _lightBlinkTime = 0.15f;
    [SerializeField] private int _totalBlinks = 3;
    [SerializeField] private int _explodeTime = 3;

    private int _currentBlinks ;
    private Rigidbody2D _rigidBody;
    private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }
    private void OnEnable()
    {
        OnExplode += Explosion;
        OnExplode += GrenadeScreenShake;
        OnExplode += DamageNearby;
        OnExplode += AudioManager.instance.Grenade_OnExplode;
        OnBeep += AudioManager.instance.Grenade_OnBeep;
        OnBeep += BlinkLight;
    }
    private void OnDisable()
    {
        OnExplode -= Explosion;
        OnExplode -= GrenadeScreenShake;
        OnExplode -= DamageNearby;
        OnExplode -= AudioManager.instance.Grenade_OnExplode;
        OnBeep -= AudioManager.instance.Grenade_OnBeep;
        OnBeep -= BlinkLight;
    }
    private void Start()
    {
        LanchGrenae();
        StartCoroutine(CountdownExplodeRoutine());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>())
        {
            OnExplode?.Invoke();
        }
    }

    private void LanchGrenae()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionToMouse = (mousePos - (Vector2)transform.position).normalized;
        _rigidBody.AddForce(directionToMouse * _launchForce, ForceMode2D.Impulse);
        _rigidBody.AddTorque(_torqueAmoust, ForceMode2D.Impulse);
    }
    private void Explosion()
    {
        Instantiate(_explodeVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private void GrenadeScreenShake()
    {
        _impulseSource.GenerateImpulse();
    }
    private void DamageNearby()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius, _enemyLayerMask);
        foreach (Collider2D hit in hits)
        {
            Health healh = hit.GetComponent<Health>();
            healh?.TakeDamage(_dameAmount);
        }
    }

    private IEnumerator CountdownExplodeRoutine()
    {
        while (_currentBlinks<_totalBlinks)
        {
            yield return new WaitForSeconds(_explodeTime / _totalBlinks);
            OnBeep?.Invoke();

            yield return new WaitForSeconds(_lightBlinkTime);
            _grenadeLight.SetActive(false);
        }

        OnExplode?.Invoke();
    }
    private void BlinkLight()
    {
        _grenadeLight.SetActive(true);
        _currentBlinks++;
    }
}
