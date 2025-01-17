using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using IslandMonkey;

public class FishingManager : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] Transform fishingPos;
	[SerializeField] FishingAnimator fishingAnimator;
	[SerializeField] GameObject waveEffect;
	[SerializeField] GameObject clamEffect;
	List<Transform> posList = new List<Transform>() ; //낚시 찌를 던질 위치

	bool isFishing = false; //낚시 상태
	int fishingPoint; //찌 던진 위치

	float fishingTime; //물고기가 잡히는 시간
	float minTime = 3.0f;
	float maxTime = 10.0f;

	float overTime = 1.0f;//놓침 판정 범위
	float validTime = 1.0f; //잡음 판정 범위

	int fishStatus = 0; // 0 : 아무 일 없음, 1: 빨리 건져서 놓침, 2: 느리게 건져서 놓침, 3: 잡음. 4: 시간 초과\

	float clickInterval = 1; //더블 클릭 기준 시간
	float doubleClickTime = 0;

	Coroutine fishCoroutine;

	GameObject _waveEffect;
	GameObject _clamEffect;

	void Start()
	{
		for(int i = 0; i < fishingPos.childCount; i++)
		{
			posList.Add(fishingPos.GetChild(i));
		}
	}

	//화면 터치 시
	public void OnPointerClick(PointerEventData eventData)
	{
		//더블 클릭 방지
		if (Time.time - doubleClickTime > clickInterval)
		{
			doubleClickTime = Time.time;
			//낚시 중이면 찌 꺼내기
			if (isFishing)
			{
				
				fishingAnimator.PlayNextAnimation();
				Debug.Log("(찌 들어올림)");
				isFishing = false;
				StopCoroutine(fishCoroutine);
				switch (fishStatus)
				{
					case 0:
						Debug.Log("아무 일도 없다.");
						SoundManager.instance.PlaySoundEffect("Fishing_Up");
						break;
					case 1:
						Debug.Log("너무 빨리 건졌다.");
						SoundManager.instance.PlaySoundEffect("Fishing_Up");
						break;
					case 2:
						Debug.Log("너무 느리게 건졌다.");
						SoundManager.instance.PlaySoundEffect("Fishing_Up");
						break;
					case 3:
						Debug.Log("잡았다!");
						fishingAnimator.succeed = true;
						SoundManager.instance.PlaySoundEffect("Fishing");
						fishingAnimator.PlayAndShowPopup();
						break;
					case 4:
						Debug.Log("시간초과");
						break;
				}

			}
			else //찌 던지기
			{
				fishingAnimator.PlayNextAnimation();
				SoundManager.instance.PlaySoundEffect("Fishing_Wave");
				fishingAnimator.succeed = false;
				SetFishingPoint();
				isFishing = true;
				fishCoroutine = StartCoroutine(Fishing());
			}
		}
		
	}

	void SetFishingPoint()
	{
		//랜덤한 값 생성
		fishingPoint = Random.Range(0, posList.Count);
		Debug.Log("(찌 던짐)fishing Point : " + fishingPoint);
	}
	IEnumerator Fishing()
	{
		//찌를 들어올릴 때까지 반복
		while (true)
		{
			fishStatus = 0;
			fishingTime = Random.Range(minTime, maxTime);
			Debug.Log("fishingTime : " + fishingTime);

			//조개 모여듬
			yield return new WaitForSeconds(fishingTime-overTime);
			Debug.Log("조개가 모여든다.");
			fishStatus = 1;

			//조개가 모여드는 효과
			_clamEffect = Instantiate(clamEffect);
			_clamEffect.transform.position = posList[fishingPoint].position;

			//조개가 찌를 뭄
			yield return new WaitForSeconds(overTime);
			SoundManager.instance.PlaySoundEffect("Fishing");
			Debug.Log("조개가 물었다!");
			fishStatus = 3;
			Handheld.Vibrate();

			//파동 이펙트
			_waveEffect = Instantiate(waveEffect);
			_waveEffect.transform.position = posList[fishingPoint].position;

			//조개가 도망감
			yield return new WaitForSeconds(validTime);
			Debug.Log("조개가 도망갔다.");
			fishingAnimator.succeed = false;
			fishStatus = 2;
			yield return new WaitForSeconds(1f);
			fishingAnimator.PlayNextAnimation();
			Debug.Log("자동 건지기");
			isFishing = false;
			SoundManager.instance.PlaySoundEffect("Fishing_Up");
			break;

		}
	}



}
