using DarkTonic.PoolBoss;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonBase<EffectManager>
{
    [SerializeField] private Transform shapeDespawnEffectPrefab;
    [SerializeField] private Transform hitShapeEffectPrefab;
    [SerializeField] private Transform receiveEffectPrefab;
    [SerializeField] private Transform breakEffectPrefab;
    [SerializeField] private ParticleSystem winEffect;
    [SerializeField] private List<ParticleSystem> lightWinEffects;

    private const float DEFAULT_ALPHA_LIGHT_WIN = 0.427451f;

    private void Start()
    {
        GameplayManager.Instance.OnStartLevelAction += GameplayManager_OnStartLevelAction;
    }

    private void GameplayManager_OnStartLevelAction(object sender, System.EventArgs e)
    {
        winEffect.Stop();
        winEffect.gameObject.SetActive(false);
    }

    public void PlayShapeDespawnEffect(float x)
    {
        Transform t = PoolBoss.Spawn(shapeDespawnEffectPrefab, new Vector3(x, -15, 0), Quaternion.identity, null);

        t.GetComponent<ParticleSystem>().Play();
    }

    public void PlayHitShapeEffect(Vector3 position)
    {
        Transform t = PoolBoss.Spawn(hitShapeEffectPrefab, position, Quaternion.identity, null);

        t.GetComponent<ParticleSystem>().Play();
    }

    public void PlayWinEffect()
    {
        winEffect.gameObject.SetActive(true);
        winEffect.Play();

        foreach (var light in lightWinEffects)
        {
            light.gameObject.SetActive(true);
        }
    }

    public void FadeLightWin()
    {
        foreach (var light in lightWinEffects)
        {
            light.gameObject.SetActive(false);
        }
    }

    public void PlayReceiveEffect(Vector3 position, float scale = 1f)
    {
        Transform t = PoolBoss.Spawn(receiveEffectPrefab, position, Quaternion.identity, null);

        t.localScale = Vector3.one * scale;

        t.GetComponent<ParticleSystem>().Play();
    }

    public void PlayBreakShapeEffect(Vector3 position, Color color)
    {
        Transform t = PoolBoss.Spawn(breakEffectPrefab, position, Quaternion.identity, null);

        ParticleSystem effect = t.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule main = effect.main;
        main.startColor = color;

        effect.Play();
    }
}
