using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class CoinObjectPool : MonoBehaviour
{
    public GameObject coinPrefab; // ���� ������

    private ObjectPool<GameObject> pool;

    private void Awake()
    {
        pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(coinPrefab),  // �� ������Ʈ ����
            actionOnGet: (obj) => obj.SetActive(true),  // ������Ʈ�� ������ �� ����
            actionOnRelease: (obj) => obj.SetActive(false),  // ������Ʈ�� ��ȯ�� �� ����
            actionOnDestroy: Destroy,  // ������Ʈ�� �ı��� �� ����
            defaultCapacity: 10,  // �⺻ �뷮
            maxSize: 20  // �ִ� �뷮
        );
    }

    // Ǯ���� ������Ʈ�� �������� �޼���
    public GameObject GetFromPool()
    {
        return pool.Get();
    }

    // ������Ʈ�� Ǯ�� ��ȯ�ϴ� �޼���
    public void ReturnToPool(GameObject obj)
    {
        pool.Release(obj);
    }
}