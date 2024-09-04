using DarkTonic.PoolBoss;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : SingletonBase<LevelGenerator>
{
    [SerializeField] private Transform shapePrefab;
    [SerializeField] private Transform holeShapePrefab;
    [SerializeField] private Transform holeBoxPrefab;
    [SerializeField] private Transform screwPrefab;

    [SerializeField] private ShapeDataSO shapeDataSO;
    [SerializeField] private ScrewAvailableSO screwAvailableSO;

    [Header("Parent")]
    [SerializeField] private Transform shapeParent;

    public ScrewAvailableSO ScrewData => screwAvailableSO;

    public ScrewSO GetCurScrewSkin()
    {
        return screwAvailableSO.GetCurSkin();
    }

    public void SetupLevel()
    {
        shapeParent.transform.position = new Vector3(0, 0f, 0f);

        GameplayManager.Instance.difficulty = DataManager.Instance.levelData.eDifficulty;

        for (int i = 0; i < DataManager.Instance.levelData.shapes.Count; i++)
        {
            ShapeData shapeData = DataManager.Instance.levelData.shapes[i];

            Shape shape = InitShapeData(shapeData.identify, shapeData.layer, shapeData.eColorShape, shapeData.worldPos.GetValue(), shapeData.localRotationZ);

            ShapeSO shapeVisual = shapeDataSO.GetShapeVisual(shapeData.id);

            if (shapeVisual == null)
            {
                Debug.LogError("Not found shape with id " + shapeData.id);
                return;
            }
            else
            {
                InitShapeVisual(shape, shapeVisual.fill, shapeVisual.shadow, shapeVisual.light, shapeVisual.posShadow);
                shape.id = shapeData.id;

                shape.transform.SetLocalScaleX(shapeData.localScaleX);
                shape.transform.SetLocalScaleY(shapeData.localScaleY);
                shape.FlipX = shapeData.flipX;
                shape.FlipY = shapeData.flipY;
            }

            InitShapeHoles(shape, shapeData.holes);

            for (int j = 0; j < shapeData.holes.Count; j++)
            {
                EHoleType type = shapeData.holes[j].eType;
                EScrewColor color = shapeData.holes[j].eScrewColor;

                ScrewSprite screwSprite = GetCurScrewSkin().GetScrewSprite(type, color);

                if (screwSprite == null)
                {
                    Debug.LogError($"Not found screwSprite with type {type} and color {color}");
                    return;
                }
                else
                {
                    Screw screw = InitScrew(shape, screwSprite.headSprite, screwSprite.tailSprite, color, type, shapeData.holes[j].localPos.GetValue());

                    shape.holes[j].screw = screw;
                    screw.hole = shape.holes[j];
                    screw.identify = shape.holes[j].identify;

                    screw.displayIdentify.text = screw.identify.ToString();
                    screw.displayIdentify.gameObject.SetActive(false);

                    GameplayManager.Instance.screws.Add(screw);
                }
            }
        }

        Resize();
    }

    private Shape InitShapeData(int identify, int layer, EColorShape color, Vector3 position, float zRotation)
    {
        Vector3 fixPos = new Vector3(position.x, position.y, 0);

        Shape shape = PoolBoss.Spawn(shapePrefab, fixPos, Quaternion.identity, shapeParent).GetComponent<Shape>();

        shape.transform.localEulerAngles = new Vector3(0, 0, zRotation);

        shape.InitData(identify, layer, color);

        GameplayManager.Instance.shapes.Add(shape);

        return shape;
    }

    private void InitShapeVisual(Shape shape, Sprite fill, Sprite shadow, Sprite fakeLight, Vector3 posShadow)
    {
        shape.InitVisual(fill, shadow, fakeLight, posShadow);
    }

    private void InitShapeHoles(Shape shape, List<HoleData> holes)
    {
        List<ShapeHole> holesList = new List<ShapeHole>();

        foreach (HoleData hole in holes)
        {
            Transform shapeHole = PoolBoss.Spawn(holeShapePrefab, shape.transform.position, Quaternion.identity, null);

            shapeHole.transform.localScale = Vector3.one * CheatManager.Instance.GetScalePercent();

            shapeHole.transform.SetParent(shape.transform);

            shapeHole.localPosition = hole.localPos.GetValue();
            shapeHole.SetLocalPositionZ(0);

            ShapeHole shapeHoleScript = shapeHole.GetComponent<ShapeHole>();

            shapeHoleScript.type = hole.eType;
            shapeHoleScript.eScrewColor = hole.eScrewColor;
            shapeHoleScript.identify = hole.identify;

            HingeJoint2D hingeJoint = shape.gameObject.AddComponent<HingeJoint2D>();
            hingeJoint.anchor = shapeHole.localPosition;
            shape.hingeJoints.Add(hingeJoint);

            holesList.Add(shapeHoleScript);
        }

        shape.holes = holesList;

        shape.SetHolesMaterial();
    }

    private Screw InitScrew(Shape shape, Sprite head, Sprite tail, EScrewColor color, EHoleType type, Vector3 position)
    {
        Transform screwTransform = Instantiate(screwPrefab, shape.transform.position, Quaternion.identity, shape.transform);

        screwTransform.localPosition = position;
        screwTransform.localScale = GameDefine.DEFAULT_SCREW_SCALE;
        screwTransform.localScale = new Vector3(screwTransform.localScale.x / shape.transform.localScale.x,
            screwTransform.localScale.y / shape.transform.localScale.y, screwTransform.localScale.z / shape.transform.localScale.z);
        screwTransform.localEulerAngles = Vector3.zero;

        screwTransform.parent = shapeParent;

        //screwTransform.GetChild(0).SetLocalRotateZ(-shape.transform.localEulerAngles.z);

        Screw screw = screwTransform.GetComponent<Screw>();

        screw.Init(head, tail, color, type);
        screw.shape = shape;

        screw.SetLayer(shape.Layer);

        return screw;
    }

    public RectTransform topNeo;
    public RectTransform botNeo;
    public RectTransform leftNeo;
    public RectTransform rightNeo;

    public RectTransform botNeoTut;

    Bounds _bounds;

    public void Resize()
    {
        StartCoroutine(IECalculate());
    }

    public IEnumerator IECalculate()
    {
        Camera cam = Camera.main;

        cam.orthographicSize = 15f;

        var sizeCam = cam.orthographicSize;

        shapeParent.transform.SetLocalPositionZ(1000f);

        Canvas.ForceUpdateCanvases();

        yield return new WaitForFixedUpdate();

        _bounds = CalculateEncapsulatedBounds(shapeParent.gameObject);

        var topLimit = topNeo.transform.position.y;
        var botLimit = TutorialManager.Instance.IsLevelShowTut() ? botNeoTut.transform.position.y : botNeo.transform.position.y;
        var rightLimit = rightNeo.transform.position.x;
        var leftLimit = leftNeo.transform.position.x;

        var objectCenter = _bounds.center.y;

        var objectMaxY = _bounds.max.y;
        var objectMinY = _bounds.min.y;

        var objectMaxX = _bounds.max.x;
        var objectMinX = _bounds.min.x;

        var offset = -objectCenter + (topLimit + botLimit) / 2f;

        shapeParent.transform.position = new Vector2(0f, offset);

        var top = objectMaxY - topLimit + offset;
        var bot = -(objectMinY - botLimit + offset);
        var right = objectMaxX - rightLimit;
        var left = -(objectMinX - leftLimit);

        var max = Mathf.Max(top, bot, right, left);

        var ratio = 0f;

        if (max == top || max == bot)
        {
            var topRatio = (topLimit) / sizeCam;
            var botRatio = (-botLimit) / sizeCam;

            ratio = (topRatio + botRatio) / 2f;
        }
        else if (max == right || max == left)
        {
            var rightRatio = rightLimit / (sizeCam * cam.aspect);
            var leftRatio = -(leftLimit / (sizeCam * cam.aspect));

            ratio = (rightRatio + leftRatio) / 2f;

            ratio *= cam.aspect;
        }

        Debug.Log("top: " + top);
        Debug.Log("bot: " + bot);
        Debug.Log("right: " + right);
        Debug.Log("left: " + left);

        max /= ratio;

        cam.orthographicSize = Mathf.Max(15, cam.orthographicSize + max);

        shapeParent.transform.position *= cam.orthographicSize / sizeCam;

        GameplayManager.Instance.SetupObstacle();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);

        // Set the color for the dot
        Gizmos.color = Color.red;
        // Draw a small sphere at the center of the bounds
        Gizmos.DrawSphere(_bounds.center, 0.1f); // Adjust the radius as needed
    }

    Bounds CalculateEncapsulatedBounds(GameObject root)
    {
        // Get all colliders in the root object and its children
        List<Collider2D> colliders = new();

        for (int i = 0; i < GameplayManager.Instance.shapes.Count; i++)
        {
            var shapeRender = GameplayManager.Instance.shapes[i];

            colliders.Add(shapeRender.GetComponent<Collider2D>());

        }

        if (colliders.Count == 0)
        {
            return new Bounds(root.transform.position, Vector3.zero);
        }

        Bounds encapsulatedBounds = colliders[0].bounds;

        foreach (Collider2D col in colliders)
        {
            encapsulatedBounds.Encapsulate(col.bounds);
        }

        return encapsulatedBounds;
    }
}
