using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSplatterHandler : MonoBehaviour
{
    private void OnEnable()
    {
        Health.OnDeath += SpawnDeathSplatterPrefab;
        Health.OnDeath += SpawnDeathVFX;

    }
    private void OnDisable()
    {
        Health.OnDeath -= SpawnDeathSplatterPrefab;
        Health.OnDeath -= SpawnDeathVFX;
    }
    private void SpawnDeathSplatterPrefab(Health sender)
    {
        GameObject newSplatterPrefab = Instantiate(sender.SplatterPrefab, sender.transform.position, sender.transform.rotation);
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        newSplatterPrefab.GetComponent<SpriteRenderer>().color = colorChanger.DefaultColor;
        newSplatterPrefab.transform.SetParent(this.transform);
    }

    private void SpawnDeathVFX(Health sender)
    {
        GameObject deathVFX = Instantiate(sender.DeathVFX, sender.transform.position, sender.transform.rotation);
        ParticleSystem.MainModule ps = deathVFX.GetComponent<ParticleSystem>().main;
        ColorChanger colorChanger = sender.GetComponent<ColorChanger>();
        ps.startColor = colorChanger.DefaultColor;
        deathVFX.transform.SetParent(this.transform);
    }
}
