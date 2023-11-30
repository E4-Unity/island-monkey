using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : Singleton<SoundManager>
{
	public AudioSource Main_BGM;
	public AudioSource Additional_BGM;

	// 볼륨 슬라이더를 위한 public 변수 추가
	public Slider mainBGMSlider;
	public Slider additionalBGMSlider;

	[Serializable]
	public class SceneBGM
	{
		public string sceneName;
		public AudioClip bgm;
		public bool playAdditionalSound;
	}

	public List<SceneBGM> sceneBGMs;
	public AudioClip voyageWaveClip;

	[Serializable]
	public class SceneSoundEffect
	{
		public string soundName;
		public AudioClip clip;
	}

	public List<SceneSoundEffect> mainSceneSoundEffects;
	public List<SceneSoundEffect> voyageSceneSoundEffects;

	private Dictionary<string, AudioClip> soundEffects = new Dictionary<string, AudioClip>();

	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SceneManager.sceneLoaded += OnSceneLoaded;

		Main_BGM = GetComponent<AudioSource>();
		Additional_BGM = gameObject.AddComponent<AudioSource>();
		Additional_BGM.clip = voyageWaveClip;

		InitializeSoundEffects(mainSceneSoundEffects);
		InitializeSoundEffects(voyageSceneSoundEffects);

		// 볼륨 슬라이더 초기화
		mainBGMSlider.value = PlayerPrefs.GetFloat("MainBGMVolume", 1f);
		additionalBGMSlider.value = PlayerPrefs.GetFloat("AdditionalBGMVolume", 1f);

		// 슬라이더의 OnValueChanged 이벤트에 메소드 연결
		mainBGMSlider.onValueChanged.AddListener(HandleMainBGMVolumeChange);
		additionalBGMSlider.onValueChanged.AddListener(HandleAdditionalBGMVolumeChange);
	}

	private void InitializeSoundEffects(List<SceneSoundEffect> soundEffectList)
	{
		foreach (var soundEffect in soundEffectList)
		{
			if (!soundEffects.ContainsKey(soundEffect.soundName))
			{
				soundEffects[soundEffect.soundName] = soundEffect.clip;
			}
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		foreach (var sceneBGM in sceneBGMs)
		{
			if (sceneBGM.sceneName == scene.name)
			{
				Main_BGM.clip = sceneBGM.bgm;
				Main_BGM.loop = true;
				Main_BGM.Play();

				if (sceneBGM.playAdditionalSound)
				{
					Additional_BGM.loop = true;
					Additional_BGM.Play();
				}
				else
				{
					Additional_BGM.Stop();
				}

				break;
			}
		}
	}

	public void PlaySoundEffect(string soundName)
	{
		if (soundEffects.TryGetValue(soundName, out AudioClip clip))
		{
			AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
		}
		else
		{
			Debug.LogWarning("이 사운드 이펙트에 오류: " + soundName);
		}
	}

	// 메인 BGM 볼륨 조절 메소드
	private void HandleMainBGMVolumeChange(float volume)
	{
		Main_BGM.volume = volume;
		PlayerPrefs.SetFloat("MainBGMVolume", volume);
		PlayerPrefs.Save();
	}

	// 추가 BGM 볼륨 조절 메소드
	private void HandleAdditionalBGMVolumeChange(float volume)
	{
		Additional_BGM.volume = volume;
		PlayerPrefs.SetFloat("AdditionalBGMVolume", volume);
		PlayerPrefs.Save();
	}
}
