using DarkTonic.PoolBoss;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Shape : MonoBehaviour
{
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private SpriteRenderer shadow;
    [SerializeField] private SpriteRenderer fakeLight;
    [SerializeField] private SortingGroup sortingGroup;
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private ColorShapeSO colorShapeSO;
    [SerializeField] public TMP_Text displayIdentify;

    public int id;

    public EColorShape eColorShape;

    public int identify;

    public List<ShapeHole> holes;
    public List<HingeJoint2D> hingeJoints = new();

    private bool isBroken = false;

    public SpriteRenderer Fill => fill;

    public int Layer
    {
        get
        {
            //return sortingGroup.sortingOrder;
            return -(int)transform.position.z;
        }
        set
        {
            //sortingGroup.sortingOrder = value;
            transform.SetPositionZ(-value);
        }
    }

    public bool IsBroken => isBroken;

    //private void OnMouseDown()
    //{
    //    if (GameplayManager.Instance.GameplayState == GameplayState.Pausing) return;

    //    if (BoosterManager.Instance.IsUsingHammer)
    //    {
    //        BoosterManager.Instance.IsUsingHammer = false;

    //        Break();
    //    }
    //}
    public bool IsEmpty()
    {
        foreach (ShapeHole hole in holes)
        {
            if (!hole.IsEmpty)
                return false;
        }

        return true;
    }

    public void InitData(int indentify, int layer, EColorShape color)
    {
        if (!CheatManager.Instance.IsLogicFirst())
            rigidbody2d.bodyType = RigidbodyType2D.Static;
        else
            rigidbody2d.bodyType = RigidbodyType2D.Dynamic;

        var config = CheatManager.Instance.GetPhysicConfig();

        rigidbody2d.mass = config.Item1;
        rigidbody2d.angularDrag = config.Item2;
        rigidbody2d.gravityScale = config.Item3;

        this.identify = indentify;
        Layer = layer;
        eColorShape = color;

        gameObject.layer = Layer + 7;

        displayIdentify.text = indentify.ToString();

        displayIdentify.gameObject.SetActive(false);

        hingeJoints.Clear();
    }

    public void ShowIndentify()
    {
        //displayIdentify.gameObject.SetActive(!displayIdentify.gameObject.activeSelf);
    }

    public void InitVisual(Sprite fill, Sprite shadow, Sprite fakeLight, Vector3 posShadow)
    {
        ActiveVisual(true);

        this.fill.sprite = fill;
        this.shadow.sprite = shadow;
        this.fakeLight.sprite = fakeLight;

        this.shadow.transform.localPosition = posShadow;

        this.fill.color = colorShapeSO.GetColorByEColorShape(eColorShape);
        this.shadow.color = colorShapeSO.GetColorByEColorShape(eColorShape);
        this.shadow.SetAlpha(0.2f);
        this.fill.SetAlpha(CheatManager.Instance.GetOpacity());

        SetCollider();

        SetLayerMaterial();
    }

    public void SetCollider()
    {
        polygonCollider.enabled = true;

        int numPath = fill.sprite.GetPhysicsShapeCount();
        polygonCollider.pathCount = numPath;

        for (int i = 0; i < numPath; i++)
        {
            List<Vector2> physicsShape = new List<Vector2>();

            int pointCount = fill.sprite.GetPhysicsShape(i, physicsShape);

            if (pointCount > 0)
            {
                polygonCollider.SetPath(i, physicsShape.ToArray());
            }
        }
    }

    private void SetLayerMaterial()
    {
        fill.sharedMaterial = PoolingManager.Instance.GetShapeMaterial(Layer + 1);
    }

    public void SetHolesMaterial()
    {
        foreach (ShapeHole hole in holes)
        {
            hole.SetMaterial(Layer + 1);
        }
    }

    public void RemoveScrewFromHole(Screw screw)
    {
        foreach (ShapeHole hole in holes)
        {
            if (hole.screw == screw)
            {
                hole.screw = null;
            }
        }

        if (rigidbody2d.bodyType == RigidbodyType2D.Static)
            rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
    }

    public bool IsFullOfScrews()
    {
        foreach (ShapeHole hole in holes)
        {
            if (hole.IsEmpty)
                return false;
        }
        return true;
    }

    public bool IsOutOfScrews()
    {
        foreach (ShapeHole hole in holes)
        {
            if (!hole.IsEmpty)
                return false;
        }
        return true;
    }

    public void CheckRemoveHinge(Screw screw)
    {
        foreach (HingeJoint2D hinge in hingeJoints)
        {
            float distance = Vector2.Distance(screw.hole.transform.localPosition, hinge.anchor);

            if (distance < 0.01f)
            {
                hingeJoints.Remove(hinge);
                Destroy(hinge);

                //if (IsOutOfScrews())
                //    DestroySelf();

                break;
            }
        }
    }

    public void DestroySelf()
    {
        ResetData();

        PoolBoss.Despawn(transform);
    }

    public void Break()
    {
        Debug.Log("Break " + name);

        isBroken = true;

        BoosterManager.Instance.boosterHammer.Use();

        Vector3 clickedWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 fixedPos = new Vector3(clickedWorldPos.x, clickedWorldPos.y, 0);

        SoundManager.Instance.PlaySound(KeySound.HammerHit);

        EffectManager.Instance.PlayHitShapeEffect(fixedPos);
        EffectManager.Instance.PlayBreakShapeEffect(transform.position, fill.color);

        rigidbody2d.bodyType = RigidbodyType2D.Kinematic;

        foreach (HingeJoint2D hinge in hingeJoints)
        {
            Destroy(hinge);
        }
        hingeJoints.Clear();

        foreach (ShapeHole hole in holes)
        {
            hole.gameObject.SetActive(false);
        }

        polygonCollider.enabled = false;

        ActiveVisual(false);

        PoolBoss.Despawn(transform);
    }

    private void ActiveVisual(bool isActive)
    {
        fill.gameObject.SetActive(isActive);
        //shadow.gameObject.SetActive(isActive);
        fakeLight.gameObject.SetActive(isActive);
    }

    public void ResetData()
    {
        foreach (HingeJoint2D hinge in hingeJoints)
        {
            Destroy(hinge);
        }
        hingeJoints.Clear();

        foreach (ShapeHole hole in holes)
        {
            if (!hole.IsEmpty)
            {
                Destroy(hole.screw.gameObject);
                //PoolBoss.Despawn(hole.screw.transform);
            }
            //Destroy(hole.transform);
            PoolBoss.Despawn(hole.transform);
        }
        holes.Clear();
    }

    public int GetNumScrewColor()
    {
        if (holes.Count == 0) return 0;

        HashSet<int> color = new();

        foreach (var hole in holes)
        {
            color.Add(((int)hole.eScrewColor));
        }

        return color.Count;
    }
}
