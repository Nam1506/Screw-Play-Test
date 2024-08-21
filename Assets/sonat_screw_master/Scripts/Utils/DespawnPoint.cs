using DG.Tweening;
using UnityEngine;

public class DespawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Shape>(out var shape))
        {
            EffectManager.Instance.PlayShapeDespawnEffect(shape.transform.position.x);

            DOVirtual.DelayedCall(1f, () =>
            {
                shape.DestroySelf();
            });
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
    }
}
