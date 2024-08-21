using DarkTonic.PoolBoss;
using Obi;
using System.Collections;
using UnityEngine;

public class Rope : MonoBehaviour
{
    //[SerializeField] private GameObject topRope;
    //[SerializeField] private GameObject bottomRope;

    public Screw topScrew;
    public Screw bottomScrew;

    [SerializeField] private Transform startPin;
    [SerializeField] private Transform endPin;

    [SerializeField] private ObiParticleAttachment startAttach;
    [SerializeField] private ObiParticleAttachment endAttach;

    [SerializeField] private Rigidbody2D rigidBody;

    private bool isDestroying;

    public void Init(Screw startScrew, Screw endScrew)
    {
        isDestroying = false;

        topScrew = startScrew;
        bottomScrew = endScrew;
    }

    public void DestroyImmediate()
    {
        //PoolBoss.Despawn(transform);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    public void DestroySelf()
    {
        if (!isDestroying)
        {
            isDestroying = true;

            ObstacleController.Instance.ropes.Remove(this);

            StartCoroutine(IeDestroySelf());
        }
    }

    private IEnumerator IeDestroySelf()
    {
        while (topScrew.IsMoving || bottomScrew.IsMoving)
        {
            yield return null;
        }

        topScrew = null;
        bottomScrew = null;

        yield return new WaitForSeconds(0.5f);

        rigidBody.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(4f);

        //PoolBoss.Despawn(transform);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (topScrew != null && bottomScrew != null)
        {
            var topPosZ = -topScrew.shape.Layer;
            var botPosZ = -bottomScrew.shape.Layer;

            startPin.transform.position = new Vector3(topScrew.transform.position.x, topScrew.transform.position.y, topPosZ);
            endPin.transform.position = new Vector3(bottomScrew.transform.position.x, bottomScrew.transform.position.y, botPosZ);

        }
    }
}
