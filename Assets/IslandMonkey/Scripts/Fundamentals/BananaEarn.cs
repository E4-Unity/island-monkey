using System.Collections;
using IslandMonkey.MVVM;
using UnityEngine;
using UnityEngine.Events;

public class BananaEarn : GoodsFactory
{
	public float bananaEarnInterval = 1.0f;
	public UnityEvent bananaEarnEvent;

	protected override void Start()
	{
		base.Start();

		StartCoroutine(EarnBananaRoutine());
	}

	private IEnumerator EarnBananaRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(bananaEarnInterval);
			EarnGoods();
			bananaEarnEvent.Invoke();
		}
	}
}
