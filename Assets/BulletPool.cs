using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet bulletPrefab;
    public int prewarmCount = 80;
    public Transform bulletsRoot;

    private readonly Queue<Bullet> queue = new Queue<Bullet>();

    private void Awake()
    {
        if (bulletsRoot == null)
        {
            bulletsRoot = transform;
        }

        for (int i = 0; i < prewarmCount; i++)
        {
            Bullet bullet = CreateNew();
            if (bullet != null)
            {
                Return(bullet);
            }
        }
    }

    public Bullet Get(Vector2 position)
    {
        Bullet bullet = queue.Count > 0 ? queue.Dequeue() : CreateNew();
        if (bullet == null)
        {
            return null;
        }

        bullet.transform.position = position;
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
        queue.Enqueue(bullet);
    }

    private Bullet CreateNew()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPool: bulletPrefab is not assigned.");
            return null;
        }

        Bullet bullet = Instantiate(bulletPrefab, bulletsRoot);
        bullet.gameObject.SetActive(false);
        return bullet;
    }
}
