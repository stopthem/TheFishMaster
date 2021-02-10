using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Hook : MonoBehaviour
{
    private Camera m_mainCamera;

    public Transform hookedTransform;

    private int m_length, m_strength, m_fishCount;

    private Collider2D m_theCD2D;

    private bool m_canMove;

    private List<Fish> hookedFishes;

    private Tweener m_cameraTween;

    private void Awake()
    {
        m_mainCamera = Camera.main;
        m_theCD2D = GetComponent<Collider2D>();
        hookedFishes = new List<Fish>();
    }

    private void Update()
    {
        if (m_canMove && Input.GetMouseButton(0))
        {
            Vector3 vector = m_mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 position = transform.position;
            position.x = vector.x;
            transform.position = position;
        }
    }

    public void StartFishing()
    {
        m_length = IdleManager.instance.length - 20;
        m_strength = IdleManager.instance.strength;
        m_fishCount = 0;

        float time = (-m_length) * .1f;

        m_cameraTween = m_mainCamera.transform.DOMoveY(m_length, 1 + time * .25f, false).OnUpdate(delegate
        {
            if (m_mainCamera.transform.position.y <= -11)
            {
                transform.SetParent(m_mainCamera.transform);
            }
        }).OnComplete(delegate
        {
            m_theCD2D.enabled = true;
            m_cameraTween = m_mainCamera.transform.DOMoveY(0, time * 5, false).OnUpdate(delegate
            {
                if (m_mainCamera.transform.position.y >= -25f)
                {
                    StopFishing();
                }
            });
        });
        ScreenManager.instance.ChangeScreen(Screens.Game);
        m_theCD2D.enabled = false;
        m_canMove = true;
        hookedFishes.Clear();
    }

    private void StopFishing()
    {
        m_canMove = false;
        m_cameraTween.Kill(false);
        m_cameraTween = m_mainCamera.transform.DOMoveY(0, 2, false).OnUpdate(delegate
        {
            if (m_mainCamera.transform.position.y >= -11)
            {
                transform.SetParent(null);
                transform.position = new Vector2(transform.position.x, -6);
            }
        }).OnComplete(delegate
        {
            transform.position = Vector2.down * 6;
            m_theCD2D.enabled = true;
            int num = 0;

            if (num == 0)
            {
                for (int i = 0; i < hookedFishes.Count; i++)
                {
                    hookedFishes[i].transform.SetParent(null);
                    hookedFishes[i].ResetFish();
                    num += hookedFishes[i].Type.price;
                    IdleManager.instance.totalGain = num;
                    ScreenManager.instance.ChangeScreen(Screens.End);
                }
            }
        });

    }
 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish") && m_fishCount != m_strength)
        {
            m_fishCount++;
            Fish hookedFish = other.GetComponent<Fish>();
            hookedFish.Hooked();
            hookedFishes.Add(hookedFish);
            other.transform.SetParent(transform);
            other.transform.position = hookedTransform.position;
            other.transform.rotation = hookedTransform.rotation;
            other.transform.localScale = Vector3.one;

            other.transform.DOShakeRotation(5, Vector3.forward * 45, 10, 90, false).SetLoops(1, LoopType.Yoyo).OnComplete(delegate
            {
                other.transform.rotation = Quaternion.identity;
            });

            if (m_fishCount == m_strength)
            {
                StopFishing();
            }
        }
    }
}
