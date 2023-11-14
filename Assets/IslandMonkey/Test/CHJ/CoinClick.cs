using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; // �̺�Ʈ ó���� ���� ���ӽ����̽�

public class CoinClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject coinPrefab; // ���� ������
    public Transform targetPosition; // ������ �̵��� ��ǥ ��ġ
    public int numberOfCoins = 10; // ������ ������ ��
    public float speed = 1.0f; // ���� �̵� �ӵ�

    // Ŭ�� �̺�Ʈ�� �߻����� �� ȣ��� �޼���
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(SpawnAndMoveCoins());
    }

    // ������ �����ϰ� ��ǥ ��ġ���� �̵���Ű�� �ڷ�ƾ
    IEnumerator SpawnAndMoveCoins()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // ���� �ν��Ͻ� ����
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            // ���� �̵�
            StartCoroutine(MoveCoin(coin, targetPosition.position));
            yield return new WaitForSeconds(0.1f); // ���� ���� ���� ���� ��� ���
        }
    }

    // ������ ��ǥ ��ġ���� �̵���Ű�� �ڷ�ƾ
    IEnumerator MoveCoin(GameObject coin, Vector3 target)
    {
        while (Vector3.Distance(coin.transform.position, target) > 0.01f)
        {
            // ������ ��ǥ ��ġ���� ���������� �̵�
            coin.transform.position = Vector3.MoveTowards(coin.transform.position, target, speed * Time.deltaTime);
            yield return null; // ���� �����ӱ��� ���
        }

        Destroy(coin); // ��ǥ ��ġ�� �����ϸ� ���� �ı�
    }
}