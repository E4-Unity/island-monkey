using System.Collections;
using UnityEngine;

public class GoldAnimation : MonoBehaviour
{
    public CoinObjectPool pool; // ������Ʈ Ǯ�� ����
    public RectTransform targetPosition; // Ÿ�� ��ġ�� UI ����� ��� RectTransform

    public void StartGoldAnimation()
    {
        StartCoroutine(SpawnAndMoveGold());
    }

    private IEnumerator SpawnAndMoveGold()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject gold = pool.GetFromPool(); // ������Ʈ Ǯ���� ��� ������Ʈ�� �����ɴϴ�.
            gold.SetActive(true);

            // ��� ������Ʈ�� RectTransform�� �����ɴϴ�.
            RectTransform goldRect = gold.GetComponent<RectTransform>();
            goldRect.SetParent(targetPosition.parent, false); // Ÿ�ٰ� ������ �θ� ����, worldPositionStays�� false�� ����
            goldRect.localScale = Vector3.one; // �������� 1�� ����
            goldRect.anchoredPosition = Vector2.zero; // ĵ������ �� �߾ӿ��� ����

            // ��带 Ÿ�� ��ġ�� �̵���ŵ�ϴ�.
            StartCoroutine(MoveGold(goldRect, targetPosition.anchoredPosition, gold));
            yield return new WaitForSeconds(0.1f); // ���� ��� ���� ���� ��� ���
        }
    }

    private IEnumerator MoveGold(RectTransform goldRect, Vector2 targetAnchoredPos, GameObject gold)
    {
        float timeToMove = 1.0f;
        float elapsedTime = 0;

        Vector2 startingPos = goldRect.anchoredPosition;

        while (elapsedTime < timeToMove)
        {
            goldRect.anchoredPosition = Vector2.Lerp(startingPos, targetAnchoredPos, (elapsedTime / timeToMove));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        goldRect.anchoredPosition = targetAnchoredPos;

        pool.ReturnToPool(gold); // ������Ʈ Ǯ�� ��ȯ
    }
}