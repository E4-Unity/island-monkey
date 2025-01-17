using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IslandMonkey;

public class BuildingBtn : MonoBehaviour
{
	[SerializeField] private List<Button> buildingButtons; // 건물 버튼 리스트
	[SerializeField] private List<GameObject> finList; // 완료된 건물 UI 이미지 리스트
	[SerializeField] private List<int> payGoldList; // 건물 건설에 필요한 골드 리스트
	[SerializeField] private ShowcaseMonkey showcaseMonkey; // 연출용 원숭이 프리팹

	[SerializeField] private GameObject getAnimalPanel; // 동물 획득 UI 패널
	[SerializeField] private GameObject buildingPanel; // 건물 UI 패널

	// 캐릭터 등장 연출
	CutsceneController cutsceneController;

	// 컴포넌트
	VoyageDataManager voyageDataManager;
	BuildingManager buildingManager;
	GoodsManager goodsManager;
	HexagonalPlacementManager placementManager;

	void Start()
	{
		// 컴포넌트 할당
		goodsManager = GlobalGameManager.Instance.GetGoodsManager();
		voyageDataManager = GlobalGameManager.Instance.GetVoyageDataManager();
		buildingManager = IslandGameManager.Instance.GetBuildingManager();
		placementManager = IslandGameManager.Instance.GetPlacementManager();

		// 이벤트 바인딩
		cutsceneController = getAnimalPanel.GetComponent<CutsceneController>();
		if (showcaseMonkey && cutsceneController)
		{
			cutsceneController.OnCutSceneEnd += OnCutSceneEnd_Event;
		}

		for (int i = 0; i < buildingButtons.Count; i++)
		{
			int index = i;
			buildingButtons[i].onClick.AddListener(() => OnBuildingButtonClicked(index));
		}
	}

	void OnCutSceneEnd_Event()
	{
		showcaseMonkey.ToggleAnimation();
	}

	void InitMonkey(Monkey monkey, MonkeyType monkeyType)
	{
		if (monkey is null) return;

		monkey.ChangeSkin(monkeyType);

#if UNITY_EDITOR
		Debug.Log(monkey.name + " 스킨 변경 완료");
#endif
	}

	private void OnBuildingButtonClicked(int buttonIndex)
	{
		// 이미 존재하는 건물은 건설하지 않음
		if (buildingManager.IsBuildingExist(buttonIndex))
		{
#if UNITY_EDITOR
			Debug.LogWarning("이미 건설되었거나 건설중인 건물입니다 : " + buttonIndex);
#endif
			return;
		}

		// 이미 다른 건물이 건설중이면 건설 불가
		if (voyageDataManager.CanEnterVoyageScene)
		{
#if UNITY_EDITOR
			Debug.LogWarning("이미 다른 건물이 건설중입니다 : " + voyageDataManager.CurrentBuildingData.Definition.ID);
#endif
			return;
		}

		if (buttonIndex >= 0 && buttonIndex < payGoldList.Count)
		{
			int payGold = payGoldList[buttonIndex];

			// 소지 금액 확인
			if (goodsManager.CanSpend(GoodsType.Gold, payGold))
			{
				// 건설 비용 지불
				goodsManager.SpendGoods(GoodsType.Gold, payGold);

				// 건설 패널 비활성화
				buildingPanel.SetActive(false);

				if (buttonIndex == 0 || buttonIndex == 3 || buttonIndex == 6)
				{
					StartCoroutine(BuildingSequence(buttonIndex)); // 연출 시작
				}
				else
				{
					RequestSpawnBuilding(buttonIndex); // 연출 없이 즉시 건설
				}
			}
			else
			{
				Debug.Log("건설에 필요한 골드가 부족합니다.");
			}
		}
		else
		{
			Debug.LogWarning("버튼 인덱스가 payGoldList 범위를 벗어났습니다.");
		}
	}

	IEnumerator BuildingSequence(int buttonIndex)
	{
		getAnimalPanel.SetActive(true);

		/* 연출용 원숭이 초기화 */
		// 원숭이 스킨을 변경하고 저장합니다.
		MonkeyType selectedType = MonkeyType.Basic; // 기본값 설정
		switch (buttonIndex)
		{
			case 0:
				selectedType = MonkeyType.Basic;
				break;
			case 3:
				selectedType = MonkeyType.Barista;
				break;
			case 6:
				selectedType = MonkeyType.Boss;
				break;
		}

		// 연출용 원숭이를 활성화 후 초기화합니다.
		showcaseMonkey.gameObject.SetActive(true);
		InitMonkey(showcaseMonkey, selectedType);
		voyageDataManager.MonkeyType = selectedType;

		// 건설 없이 건물 데이터만 저장
		RequestSpawnBuilding(buttonIndex, false);

		// TODO 리팩토링
		yield return new WaitForSeconds(11f); // 연출 지연

		// TODO BuildingSpawnManager 에서 처리
		// 원숭이 타입
		voyageDataManager.MonkeyType = selectedType;

		SceneLoadingManager.Instance.ChangeScene(BuildScene.Voyage, SceneLoadingManager.ChangeSceneType.Animation); // 항해 씬 넘어가기
	}

	private void RequestSpawnBuilding(int buttonIndex, bool spawnImmediately = true)
	{
		// 유효성 검사
		if (placementManager is null) return;

		// 건설되지 않은 경우에만 실행
		if (buildingManager.IsBuildingExist(buttonIndex)) return;

		// 건설 요청
		placementManager.RequestSpawnBuilding(buttonIndex, spawnImmediately);
	}
}
