using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DiscoBallManager : MonoBehaviour
{
    public static Action OnDiscoBallHitEvent;
    
    [SerializeField] private float _discoBallPartyTime = 2f;
    [SerializeField] private float _discoGlobalLightIntensity = 0.2f;
    [SerializeField] private Light2D _globallLight;

    private float _defaultGlobalLightIntensity;
    private Coroutine _discoCoroutine;
    private ColorSpotlight[] _allSpotlights;

    private void Awake()
    {
        _defaultGlobalLightIntensity = _globallLight.intensity;
    }
    private void Start()
    {
        _allSpotlights = FindObjectsByType<ColorSpotlight>(FindObjectsSortMode.None);
    }
    private void OnEnable()
    {
        OnDiscoBallHitEvent += DimTheLights;
    }
    private void OnDisable()
    {
        OnDiscoBallHitEvent -= DimTheLights;
        
    }
    public void DiscoBallParty()
    {
        if (_discoCoroutine != null)
            return;

        OnDiscoBallHitEvent?.Invoke();
    }
    private void DimTheLights()
    {
        foreach (ColorSpotlight spotLight in _allSpotlights)
        {
            StartCoroutine(spotLight.SpotLightDiscoParty(_discoBallPartyTime));
        }

        _discoCoroutine = StartCoroutine(GlobalLightResetRoutine());
    }
    private IEnumerator GlobalLightResetRoutine()
    {
        _globallLight.intensity = _discoGlobalLightIntensity;
        yield return new WaitForSeconds(_discoBallPartyTime);
        _globallLight.intensity = _defaultGlobalLightIntensity;
        _discoCoroutine = null;
    }
}
