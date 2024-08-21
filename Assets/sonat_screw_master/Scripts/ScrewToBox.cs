using DarkTonic.PoolBoss;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScrewToBox : MonoBehaviour
{
    public float totalTime = 1f;

    public Transform screwPrefab;
    public int minCount;
    public int maxCount;
    public float minX = -5f;
    public float maxX = 5f;
    public float forceMagnitude = 10f;
    public float torqueMagnitude = 5f;
    public float timeGap = 1f;

    public float timeDelayBox = 1f;
    public float timeAfter = 1f;

    private float time = 0f;

    public List<Material> materials;

    public SkeletonGraphic boxAnim;

    public Button btn;

    [SerializeField] private Screw3DSO screw3DSO;

    private void Start()
    {
        if (btn != null)
            btn.onClick.AddListener(Click);
    }

    public void Click()
    {
        StopAllCoroutines();

        StartCoroutine(IESpawn());

        StartCoroutine(PlayAnimBox());
    }

    public void Spawn()
    {
        StopAllCoroutines();

        StartCoroutine(IESpawn());
    }

    private IEnumerator IESpawn()
    {
        Camera.main.orthographicSize = 5f;

        Vector3 pos = Helper.GetTopScreen();
        pos.y += 2f;
        pos.z = 89f;

        time = 0f;

        StartCoroutine(IECountDown(time));

        while (time < totalTime)
        {
            for (int i = 0; i < Random.Range(minCount, maxCount + 1); i++)
            {
                pos.x = Random.Range(minX, maxX);

                var screw = PoolBoss.Spawn(screwPrefab, pos, Quaternion.Euler(GetRandomRotation()), null);

                screw.GetComponent<MeshRenderer>().sharedMaterial = materials[Random.Range(0, materials.Count)];

                var endPos = boxAnim.transform.position;

                endPos.z = 89f;

                Vector3 directionToBox = (endPos - screw.position).normalized;

                Rigidbody rb = screw.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.ResetInertiaTensor();
                    rb.velocity = Vector3.zero;

                    rb.AddForce(directionToBox * forceMagnitude, ForceMode.Impulse);

                    Vector3 randomTorque = new Vector3(
                        Random.Range(-torqueMagnitude, torqueMagnitude),
                        Random.Range(-torqueMagnitude, torqueMagnitude),
                        Random.Range(-torqueMagnitude, torqueMagnitude)
                    );

                    rb.AddTorque(randomTorque, ForceMode.Impulse);

                    //float fallDistance = pos.y - boxTransform.position.y;
                    //float fallTime = Mathf.Sqrt(2 * fallDistance / gravity);
                }
            }

            yield return new WaitForSeconds(timeGap);
        }
    }

    private IEnumerator PlayAnimBox()
    {
        yield return new WaitForSeconds(timeDelayBox);

        boxAnim.Initialize(true);

        boxAnim.AnimationState.SetAnimation(0, EStateBox.Drop.ToString(), true);

        yield return new WaitForSeconds(totalTime + timeAfter);

        boxAnim.AnimationState.SetAnimation(0, EStateBox.Out.ToString(), false);

        yield return new WaitForSeconds(Helper.GetTimeAnim(boxAnim, EStateBox.Out.ToString()));

        boxAnim.Initialize(true);

        boxAnim.AnimationState.SetAnimation(0, EStateBox.In.ToString(), false);
        boxAnim.AnimationState.AddAnimation(0, EStateBox.Idle.ToString(), true, 0);

    }

    private IEnumerator IECountDown(float value)
    {
        while (value < totalTime)
        {
            time += Time.deltaTime;

            yield return null;
        }
    }

    private Vector3 GetRandomRotation()
    {
        float randomX = Random.Range(0f, 360f);
        float randomY = Random.Range(0f, 360f);
        float randomZ = Random.Range(0f, 360f);

        return new Vector3(randomX, randomY, randomZ);
    }

    private enum ESkinBox
    {
        Hard,
        Normal,
        Superhard
    }

    private enum EStateBox
    {
        Drop,
        Idle,
        In,
        Out
    }
}