using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private float _camZoomSpeed;
    [SerializeField] private float _camMoveSpeed;
    GameObject _player;
    [SerializeField] Camera _camera;
    [SerializeField] private PanelTrigger _panel1enter;
    [SerializeField] private PanelTrigger _panel2enter;

    [SerializeField] private List<GameObject> _panelsScene = new List<GameObject>();
    [SerializeField] private List<GameObject> _panelsView = new List<GameObject>();

    private void Start()
    {
        _panel1enter.TriggerEnter += Panel1Enter_Performed;
        _panel2enter.TriggerEnter += Panel2Enter_Performed;
    }

    private void SetPanel(int CurrentIndex, int newIndex)
    {
        _panelsScene[CurrentIndex].SetActive(false);
        _panelsScene[newIndex].SetActive(true);
    }
    private void SetPlayerPos(Vector3 pos)
    {
        if(_player == null) _player = GameObject.Find("Square(Clone)");
        pos.z = 0;
        _player.transform.position = pos;
    }

    private void SetCameraPos(int index)
    {
        StartCoroutine(DoMoveCam(_panelsView[index].transform.position));
    }

    private void Panel1Enter_Performed(object sender, EventArgs e)
    {
        GameObject obj = sender as GameObject;
        SetPanel(1, 0);
        SetPlayerPos(_panelsScene[0].transform.position);
        SetCameraPos(0);
    }

    private void Panel2Enter_Performed(object sender, EventArgs e)
    {
        GameObject obj = sender as GameObject;
        SetPanel(0,1);
        SetPlayerPos(_panelsScene[1].transform.position);
        SetCameraPos(1);
    }

    private IEnumerator DoMoveCam(Vector3 newCamPos)
    {
        float camSize = 5;
        newCamPos.z = -10;
        float time = 0;
        Vector3 startpos = _camera.transform.position;
        while(_camera.orthographicSize < camSize + 0.76f)
        {
            _camera.orthographicSize += _camZoomSpeed * Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = camSize + 0.76f;

        while (Vector3.Distance(_camera.transform.position , newCamPos) > 0.2f)
        {
            time += Time.deltaTime;
            _camera.transform.position = Vector3.Lerp(startpos, newCamPos, _camMoveSpeed * time);
            yield return null;
        }
        _camera.transform.position = newCamPos;

        while (_camera.orthographicSize > camSize + 0.76f)
        {
            _camera.orthographicSize -= _camZoomSpeed * Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = camSize;
    }
}
