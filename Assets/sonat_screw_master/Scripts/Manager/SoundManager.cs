using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource bgSource;
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private float volume;

    public bool IsVolumeOn => volume > 0;

    private void Start()
    {
        volume = DataManager.Instance.playerData.settings.isSoundOn ? 1f : 0;
    }

    public void ToggleVolume()
    {
        if (volume == 0)
        {
            volume = 1f;
        }
        else
        {
            volume = 0f;
        }

        audioSource.volume = volume;
        bgSource.volume = volume;

        Save();
    }

    private void Save()
    {
        DataManager.Instance.playerData.settings.isSoundOn = volume > 0;
        DataManager.Instance.Save();
    }

    private void PlaySound(AudioClip audioClip, bool isLoop = false, float volumeMultiplier = 1f)
    {
        if (!IsVolumeOn) return;

        if (audioClip == null)
        {
            audioSource.Stop();
            return;
        }

        if (isLoop)
        {
            audioSource.clip = audioClip;
            audioSource.volume = volumeMultiplier * volume;
            audioSource.Play();
            audioSource.loop = isLoop;
        }
        else
        {
            audioSource.PlayOneShot(audioClip, volumeMultiplier * volume);
        }
    }

    public void PlaySound(KeySound keySound, bool isLoop = false, float volumeMultiplier = 1f)
    {
        var audioClip = audioClipRefsSO.keySounds.Find(x => x.eSound == keySound);

        if (audioClip == null) return;

        PlaySound(audioClip.audioClip, isLoop, volumeMultiplier);
    }

    public void PlayGateSound(bool isLoop = false, float volumeMultiplier = 1f)
    {
        int random = Random.Range(1, 4);

        var audioClip = audioClipRefsSO.keySounds.Find(x => string.Equals($"Gate_{random}", x.eSound.ToString()));

        PlaySound(audioClip.audioClip, isLoop, volumeMultiplier);
    }

    public void PlayBGSound()
    {
        if (!IsVolumeOn) return;

        bgSource.volume = 0f;

        IncreaseVolume();

        bgSource.Play();
    }

    public void PlayComboSound(KeyCombo keyCombo, int index)
    {
        var combo = audioClipRefsSO.keyCombos.Find(x => x.key == keyCombo);

        if (combo == null) return;

        index = Mathf.Min(combo.audioClips.Count - 1, index);

        PlaySound(combo.audioClips[index]);
    }

    public void StopLoopSound()
    {
        PlaySound(null);
    }

    public void StopBGSound()
    {
        bgSource.Stop();
    }

    public void IncreaseVolume()
    {
        StartCoroutine(IEIncreaseVolume(1f));
    }

    public void DecreaseVolume()
    {
        StartCoroutine(IEDecreaseVolume(1f));
    }

    private IEnumerator IEIncreaseVolume(float duration)
    {
        float startVolume = 0f;
        float endVolume = 1f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, endVolume, currentTime / duration);
            bgSource.volume = newVolume;
            yield return null;
        }

        bgSource.volume = endVolume;
    }


    private IEnumerator IEDecreaseVolume(float duration)
    {
        float startVolume = 1f;
        float endVolume = 0f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(startVolume, endVolume, currentTime / duration);
            bgSource.volume = newVolume;
            yield return null;
        }

        bgSource.volume = endVolume;
        bgSource.Stop();
    }
}

public enum KeySound
{
    None = -1,
    HammerHit = 0,
    AddBox = 1,
    AddHole = 2,
    BoxMove = 3,
    BoxDone = 4,
    BtnClose = 5,
    BtnOpen = 6,
    BreakHeart = 7,
    GiveUp = 8,
    Lose = 9,
    Win = 10,
    ScrewPick = 11,
    CoinsPickup = 12,
    Unlock_Obstacle = 13,
    Booster_Appear = 14,
    Booster_Receive = 15,
    Rope = 16,
    Break_Ice = 17,
    Gate_1 = 18,
    Gate_2 = 19,
    Gate_3 = 20,
    Bomb_Start = 21,
    Bomb_Count = 22,
    Bomb_Warning = 25,
    Bomb_Explosion = 23,
    Bomb_Remove = 24,
    Chain_Remove = 26,
    Key_Lock = 27,
    Race_Banner_ComeIn = 28,
    Race_Banner_ComeOut = 29,
    Race_Clock_Disappear = 30,
    Race_Gift_Appear = 31,
    Race_Gift_Open = 32,
    Home_Box_In = 33,
    Home_Box_Out = 34,
    Home_Box_Fill = 35,
    Home_Machine = 36,
    Chest_Level_Open = 37,
    Chest_Level_Appear = 38,
    Race_Board = 39,
    Race_Mine = 40,
    Race_Mine_Loop = 41,
}

public enum KeyCombo
{
    None = -1,
    Chest_Level_Open = 0
}