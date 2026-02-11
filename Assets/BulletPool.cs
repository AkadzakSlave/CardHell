using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int initialSize = 32;

    private readonly Queue<Bullet> pool = new Queue<Bullet>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            Bullet bullet = CreateNew();
            if (bullet != null)
            {
                Return(bullet);
            }
        }
    }

    public Bullet Get()
    {
        if (pool.Count == 0)
        {
            return CreateNew();
        }

        Bullet bullet = pool.Dequeue();
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void Return(Bullet bullet)
    {
        if (bullet == null)
        {
            return;
        }

        bullet.gameObject.SetActive(false);
        pool.Enqueue(bullet);
    }

    private Bullet CreateNew()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPool: bulletPrefab is not assigned.");
            return null;
        }

        Bullet bullet = Instantiate(bulletPrefab, transform);
        bullet.gameObject.SetActive(false);
        return bullet;
    }
}
