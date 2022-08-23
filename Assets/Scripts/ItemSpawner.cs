using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] SellController _sellController;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private Transform _container;
    [SerializeField] private int _itemSpawnCount;
    [SerializeField] private float _waitTime;
    [SerializeField] private List<GameObject> _itemPool;
    [SerializeField] private bool _autoExpand;
    private bool _canCreate = true;


    private void Awake()
    {
        CreatePool(_itemSpawnCount);
        _sellController.Notify += ReturnElement;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HitToRandomPoint();
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnItem());
    }

    private void CreatePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    private GameObject CreateObject(bool IsActiveByDefault = false)
    {
        GameObject createdObject = Instantiate(_itemPrefab, _container);
        createdObject.SetActive(IsActiveByDefault);
        _itemPool.Add(createdObject);
        return createdObject;
    }

    private IEnumerator SpawnItem()
    {
        Vector3 ItemSpawnPoint = HitToRandomPoint();
        GameObject go = GetFreeElement();
        if (go != null)
        {
            go.transform.position = ItemSpawnPoint;
            go = null;
            yield return new WaitForSeconds(_waitTime);
            StartCoroutine(SpawnItem());
        }

        else if (!_canCreate)
        {
            yield return new WaitWhile(() => _canCreate != true);
            StartCoroutine(SpawnItem());
        }
    }

    private Vector3 HitToRandomPoint()
    {
        RaycastHit hit;
        Ray ray = _camera.ViewportPointToRay(new Vector3(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f, 1f), 0));
        if (Physics.Raycast(ray, out hit))
        {
            return new Vector3(hit.point.x, 0.6f, hit.point.z);
        }

        else
        {
            return HitToRandomPoint();
        }
    }

    public GameObject GetFreeElement()
    {
        if (HasFreeElement(out GameObject freeElement))
            return freeElement;
        if (_autoExpand)
            return CreateObject(true);
        _canCreate = false;
        return null;
    }

    public bool HasFreeElement(out GameObject freeElement)
    {
        foreach (GameObject go in _itemPool)
        {
            if (!go.activeInHierarchy)
            {
                freeElement = go;
                freeElement.SetActive(true);
                return true;
            }
        }

        freeElement = null;
        return false;
    }

    public void ReturnElement(GameObject element)
    {
        element.SetActive(false);
        element.transform.parent = _container;
        element.transform.position = _container.position;
        BoxCollider boxCollider = element.GetComponent<BoxCollider>();
        boxCollider.enabled = true;
        if (!_canCreate)
            _canCreate = true;
    }
}
