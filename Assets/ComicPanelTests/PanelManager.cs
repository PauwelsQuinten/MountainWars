using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _interaction;
    [SerializeField] private bool _doPanelClimb;
    [SerializeField] private float _camZoomSpeed;
    [SerializeField] private float _camMoveSpeed;
    [SerializeField] Camera _camera;
    [SerializeField] private PanelTrigger _panel1Trigger;
    [SerializeField] private PanelTrigger _panel2Trigger;
    [SerializeField] private PanelTrigger _panel2Trigger2;
    [SerializeField] private PanelTrigger _panel3Trigger;
    [SerializeField] private PanelTrigger _panel3Trigger2;


    [SerializeField] private List<GameObject> _panelsScene = new List<GameObject>();
    [SerializeField] private List<GameObject> _panelsView = new List<GameObject>();
    [SerializeField] private List<Transform> _spawnPos = new List<Transform>();

    private GameObject _player;
    private bool _CanTeleport = true;
    private GameObject _teleportedFromTrigger;
    private bool _camCanMove;
    private GameObject _staminaBar;
    private bool _isNearTree;
    private bool _isInTree;
 
    private void Start()
    {
        _staminaBar = _camera.GetComponentInChildren<SpriteRenderer>().gameObject;
        _panel1Trigger.TriggerEnter += Panel1Enter_Performed;
        _panel1Trigger.TriggerExit += Panel1Exit_Performed;

        _panel2Trigger.TriggerEnter += Panel2Enter_Performed;
        _panel2Trigger.TriggerExit += Panel2Exit_Performed;

        _panel2Trigger2.TriggerEnter += Panel2Enter2_Performed;
        _panel2Trigger2.TriggerExit += Panel2Exit2_Performed;

        _panel3Trigger.TriggerEnter += Panel3Enter_Performed;
        _panel3Trigger.TriggerExit += Panel3Exit_Performed;

        _panel3Trigger2.TriggerEnter += Panel3Enter2_Performed;
        _panel3Trigger2.TriggerExit += Panel3Exit2_Performed;

    }

    private void Update()
    {
        if(_isNearTree && _interaction.action.WasPerformedThisFrame())
        {
            _isInTree = true;
            _isNearTree = false;
            _camCanMove = false;
            SetPlayerPos(_spawnPos[5].position, 2);
            SetPanel(2, 3);

            Vector3 playerFromPos = _panelsView[2].transform.position;
            Vector3 playerToPos = _panelsView[3].transform.position;
            playerFromPos.y += 10.8f / 2;
            playerToPos.y -= 10.8f / 2;
            SetCameraPos(3, playerFromPos, playerToPos);
            SetCamSize(2 ,1, true);

            if (_player != null)_player.GetComponent<CharacterController>().enabled = false;
        }
        else if(_isInTree && _interaction.action.WasPerformedThisFrame())
        {
            _isInTree = false;
            _camCanMove = true;
            SetPlayerPos(_spawnPos[4].position, 2);
            SetPanel(3, 2);

            Vector3 playerFromPos = _panelsView[3].transform.position;
            Vector3 playerToPos = _panelsView[2].transform.position;
            playerFromPos.y += 10.8f / 2;
            playerToPos.y -= 10.8f / 2;
            SetCameraPos(2, playerFromPos, playerToPos);
            SetCamSize(2, 0);

            if (_player != null) _player.GetComponent<CharacterController>().enabled = true;
        }
    }

    private void SetPanel(int CurrentIndex, int newIndex)
    {
        if(!_isInTree && CurrentIndex != 3)_panelsScene[CurrentIndex].SetActive(false);
        _panelsScene[newIndex].SetActive(true);
    }
    private void SetPlayerPos(Vector3 pos, int index)
    {
        if(_player == null) _player = GameObject.Find("Square(Clone)");
        pos.z = 0;
        _player.GetComponent<CharacterController>().enabled = false; 
        _player.transform.position = pos;
        _player.GetComponent<CharacterController>().enabled = true;

        Camera cam = _panelsScene[index].GetComponentInChildren<Camera>();
        if(cam == null) cam = _player.GetComponentInChildren<Camera>();

        if (_camCanMove)
        {
            cam.transform.parent = _player.transform;
            cam.transform.localPosition = new Vector3(0,0,-10);
        }
        else
        {
            cam.transform.parent = _panelsScene[index].transform;
            cam.transform.localPosition = new Vector3(0, 0, -10);
        }
    }

    private void SetCamSize(int index, float size, bool setPos = false)
    {
        Camera cam = _panelsScene[index].GetComponentInChildren<Camera>();
        if (cam == null) cam = _player.GetComponentInChildren<Camera>();
        cam.orthographicSize = 5 + size;

        if (!setPos) return;
        Vector3 newPos = cam.transform.localPosition;
        newPos.x += size;
        newPos.y += size * 2;
        cam.transform.localPosition = newPos;
    }

    private void SetCameraPos(int index, Vector3 playerSpawnPos, Vector3 playerToPos)
    {
        StartCoroutine(DoMoveCam(_panelsView[index].transform.position, playerSpawnPos, playerToPos));
    }

    private void Panel1Enter_Performed(object sender, EventArgs e)
    {
        _camCanMove = false;
        if (!_CanTeleport) return;
        SetPanel(0, 1);
        SetPlayerPos(_spawnPos[1].position, 1);

        Vector3 playerFromPos = _panelsView[0].transform.position;
        Vector3 playerToPos = _panelsView[1].transform.position;
        playerFromPos.x += 19.2f / 2;
        playerToPos.x -= 19.2f / 2;
        SetCameraPos(1, playerFromPos, playerToPos);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel1Exit_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true;
    }

    private void Panel2Enter_Performed(object sender, EventArgs e)
    {
        _camCanMove = false;
        if (!_CanTeleport) return;
        SetPanel(1,0);
        SetPlayerPos(_spawnPos[0].position, 0);

        Vector3 playerFromPos = _panelsView[1].transform.position;
        Vector3 playerToPos = _panelsView[0].transform.position;
        playerFromPos.x -= 19.2f / 2;
        playerToPos.x += 19.2f / 2;
        SetCameraPos(0, playerFromPos, playerToPos);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel2Exit_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true; 
    }

    private void Panel2Enter2_Performed(object sender, EventArgs e)
    {
        _camCanMove = true;
        if (!_CanTeleport) return;
        SetPanel(1, 2);
        SetPlayerPos(_spawnPos[3].position, 2);

        Vector3 playerFromPos = _panelsView[1].transform.position;
        Vector3 playerToPos = _panelsView[2].transform.position;
        playerFromPos.y -= 10.8f / 2;
        playerToPos.y += 10.8f / 2;
        SetCameraPos(2, playerFromPos, playerToPos);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel2Exit2_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true;
    }

    private void Panel3Enter_Performed(object sender, EventArgs e)
    {
        _camCanMove = false;
        if (!_CanTeleport) return;
        SetPlayerPos(_spawnPos[2].position, 2);
        SetPanel(2, 1);

        Vector3 playerFromPos = _panelsView[2].transform.position;
        Vector3 playerToPos = _panelsView[1].transform.position;
        playerFromPos.y += 10.8f / 2;
        playerToPos.y -= 10.8f / 2;
        SetCameraPos(1, playerFromPos, playerToPos);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel3Exit_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true;
    }

    private void Panel3Enter2_Performed(object sender, EventArgs e)
    {
        _isNearTree = true;
    }

    private void Panel3Exit2_Performed(object sender, EventArgs e)
    {
        _isNearTree = false;
    }

    private IEnumerator DoMoveCam(Vector3 newCamPos, Vector3 spawnPos, Vector3 toPos)
    {
        if(_player != null) _player.GetComponent<CharacterController>().enabled = false;
        _staminaBar.SetActive(false);
        float camSize = _camera.orthographicSize;
        newCamPos.z = -10;
        float time = 0;
        Vector3 startpos = _camera.transform.position;
        while(_camera.orthographicSize < camSize + 0.76f)
        {
            _camera.orthographicSize += _camZoomSpeed * Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = camSize + 0.76f;

        if (_doPanelClimb)
        {
            time = 0;
            startpos = _camera.transform.position;
            float midwayPoint = Vector3.Distance(_camera.transform.position, newCamPos) / 2;
            while (Vector3.Distance(_camera.transform.position, newCamPos) > midwayPoint)
            {
                time += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(startpos, newCamPos, _camMoveSpeed * time);
                yield return null;
            }
            time = 0;

            GameObject newPlayer = Instantiate(_player, spawnPos, Quaternion.identity);
            newPlayer.name = "Temp";
            newPlayer.GetComponent<CharacterController>().enabled = false;
            startpos = newPlayer.transform.position;
            while (Vector3.Distance(newPlayer.transform.position, toPos) > 0.1f)
            {
                time += Time.deltaTime;
                newPlayer.transform.position = Vector3.Lerp(startpos, toPos, (_camMoveSpeed) * time);
                yield return null;
            }
            newPlayer.transform.position = toPos;
            Destroy(newPlayer);
            time = 0;

            startpos = _camera.transform.position;
            while (Vector3.Distance(_camera.transform.position, newCamPos) > 0.2f)
            {
                time += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(startpos, newCamPos, _camMoveSpeed * time);
                yield return null;
            }
            //_camera.transform.position = newCamPos;
        }
        else
        {
            while (Vector3.Distance(_camera.transform.position, newCamPos) > 0.2f)
            {
                time += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(startpos, newCamPos, _camMoveSpeed * time);
                yield return null;
            }
            _camera.transform.position = newCamPos;
        }

        while (_camera.orthographicSize > camSize + 0.76f)
        {
            _camera.orthographicSize -= _camZoomSpeed * Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = camSize;
        if (_player != null && !_isInTree) _player.GetComponent<CharacterController>().enabled = true;
        _staminaBar.SetActive(true);
    }
}
