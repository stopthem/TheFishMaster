using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class Fish : MonoBehaviour
{
    [Serializable]
    public class FishType
    {
        public int price;

        public float fishCount;
        public float minLength;
        public float maxLength;
        public float colliderRadius;

        public Sprite sprite;
    }

    private Fish.FishType m_fishType;

    private CircleCollider2D m_circleCollider2D;

    private SpriteRenderer m_spriteRenderer;

    private float m_screenLeft;

    private Tweener m_tweener;

    public Fish.FishType Type
    {
        get
        {
            return m_fishType;
        }
        set
        {
            m_fishType = value;
            m_circleCollider2D.radius = m_fishType.colliderRadius;
            m_spriteRenderer.sprite = m_fishType.sprite;

        }
    }

    private void Awake()
    {
        m_circleCollider2D = GetComponent<CircleCollider2D>();
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_screenLeft = Camera.main.ScreenToWorldPoint(Vector3.zero).x;
    }

    public void ResetFish()
    {
        if (m_tweener != null)
        {
            m_tweener.Kill(false);
        }

        float num = UnityEngine.Random.Range(m_fishType.minLength, m_fishType.maxLength);
        m_circleCollider2D.enabled = true;

        Vector3 position = transform.position;
        position.y = num;
        position.x = m_screenLeft;
        transform.position = position;

        float num2 = 1;
        float y = UnityEngine.Random.Range(num - num2, num + num2);
        Vector2 v = new Vector2(-position.x, y);

        float num3 = 3;
        float delay = UnityEngine.Random.Range(0, 2 * num3);
        m_tweener = transform.DOMove(v, num3, false).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetDelay(delay).OnStepComplete(delegate {
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        });

    }

    public void Hooked()
    {
        m_circleCollider2D.enabled = false;
        m_tweener.Kill(false);
    }

}
