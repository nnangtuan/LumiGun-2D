using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private float _disableColliderTime=1f;


    private bool _playerOnPlatform = false;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        DetectPlayerInput();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            _playerOnPlatform = false;
        }
    }
    private void DetectPlayerInput()
    {
        Debug.Log(_playerOnPlatform);
        if (!_playerOnPlatform) return;
        Debug.Log(PlayerController.Instance.MoveInput.y);
        if (PlayerController.Instance.MoveInput.y <= 0) {
            StartCoroutine(DisablePlatformColliderRoutine());
        }
    }
    private IEnumerator DisablePlatformColliderRoutine()
    {
        yield return new WaitForSeconds(_disableColliderTime);
        Collider2D[] playerColliders = PlayerController.Instance.GetComponents<Collider2D>();

        Debug.Log("Player Colliders Found: " + playerColliders.Length);

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, true);

        }
        yield return new WaitForSeconds(_disableColliderTime);

        foreach (Collider2D playerCollider in playerColliders)
        {
            Physics2D.IgnoreCollision(playerCollider, _collider, false);
        }
    }
}
