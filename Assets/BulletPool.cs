using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet bulletPrefab;
    public int prewarmCount = 80;
    public Transform bulletsRoot;

    private readonly Queue<Bullet> available = new Queue<Bullet>();

    private void Awake()
    {
        if (bulletsRoot == null)
        {
            bulletsRoot = transform;
        }

        for (int i = 0; i < prewarmCount; i++)
        {
            Bullet bullet = CreateBullet();
            if (bullet != null)
            {
                Return(bullet);
            }
        }
    }

    public Bullet Get(Vector2 position)
    {
        Bullet bullet = available.Count > 0 ? available.Dequeue() : CreateBullet();
        if (bullet == null)
        {
            return null;
        }

        bullet.transform.SetParent(bulletsRoot);
        bullet.transform.position = position;
        bullet.gameObject.SetActive(true);
        return bullet;
    }

    public void Return(Bullet b)
    {
        if (b == null)
        {
            return;
        }

        b.gameObject.SetActive(false);
        b.transform.SetParent(bulletsRoot);
        available.Enqueue(b);
    }

    private Bullet CreateBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPool is missing bulletPrefab reference.");
            return null;
        }

        Bullet bullet = Instantiate(bulletPrefab, bulletsRoot);
        bullet.gameObject.SetActive(false);
        return bullet;
    }
}
