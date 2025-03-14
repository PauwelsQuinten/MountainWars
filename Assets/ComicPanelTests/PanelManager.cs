using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _interaction;
    [SerializeField] private bool _doPanelClimb;
    [SerializeField] private float _camZoomSpeed;
    [SerializeField] private float _camMoveSpeed;
    [SerializeField] private float _maxEnemyDistance;
    [SerializeField] private GameObject _showdownPanel;
    [SerializeField] Camera _camera;
    [SerializeField] private PanelTrigger _panel1Trigger;
    [SerializeField] private PanelTrigger _panel2Trigger;
    [SerializeField] private PanelTrigger _panel2Trigger2;
    [SerializeField] private PanelTrigger _panel3Trigger;
    [SerializeField] private PanelTrigger _panel3Trigger2;


    [SerializeField] private List<GameObject> _panelsScene = new List<GameObject>();
    [SerializeField] private List<GameObject> _panelsView = new List<GameObject>();
    [SerializeField] private List<Transform> _spawnPos = new List<Transform>();
    [SerializeField] private List<GameObject> _biomePanels = new List<GameObject>();

    private GameObject _player;
    private bool _CanTeleport = true;
    private GameObject _teleportedFromTrigger;
    private GameObject _staminaBar;
    private bool _isNearTree;
    private bool _isInTree;
    private GameObject _lastEnemy;
    private bool _canDoShowdown;
 
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
        ShowTreePannel();
        GameObject enemy = GameObject.Find("enemy(Clone)");
        if (enemy == null) return;

        if (_lastEnemy == null)
        {
            _lastEnemy = enemy;
            _canDoShowdown = true;
        }

            if (_player == null) _player = GameObject.Find("Square(Clone)");

        float distance = Vector3.Distance(enemy.transform.position, _player.transform.position);

        //if (distance <= 4 || distance > _maxEnemyDistance) _showdownPanel.SetActive(false);

        if (!_canDoShowdown) return;

        if (distance <= _maxEnemyDistance)
        {
            _showdownPanel.SetActive(true);
            _canDoShowdown = false;
            StartCoroutine(ResetShowdownPanel());
        }
    }

    private void ShowTreePannel()
    {
        if (_isNearTree && _interaction.action.WasPerformedThisFrame())
        {
            _isInTree = true;
            _isNearTree = false;
            SetPlayerPos(_spawnPos[5].position);
            SetCam(2, false);
            //SetPanel(2, 3);

            Vector3 playerFromPos = _panelsView[2].transform.position;
            Vector3 playerToPos = _panelsView[3].transform.position;
            playerFromPos.y += 10.8f / 2;
            playerToPos.y -= 10.8f / 2;
            //SetCameraPos(2, playerFromPos, playerToPos);
            SetCamSize(2, 2, true);
            _panelsView[3].gameObject.SetActive(true);

            if (_player != null) _player.GetComponent<CharacterController>().enabled = false;
        }
        else if (_isInTree && _interaction.action.WasPerformedThisFrame())
        {
            _isInTree = false;
            SetPlayerPos(_spawnPos[4].position);
            SetCam(2, true);
            //SetPanel(3, 2);

            Vector3 playerFromPos = _panelsView[3].transform.position;
            Vector3 playerToPos = _panelsView[2].transform.position;
            playerFromPos.y += 10.8f / 2;
            playerToPos.y -= 10.8f / 2;
            //SetCameraPos(2, playerFromPos, playerToPos);
            SetCamSize(2, 0);
            _panelsView[3].gameObject.SetActive(false);

            if (_player != null) _player.GetComponent<CharacterController>().enabled = true;
        }
    }
    private void SetPanel(int CurrentIndex, int newIndex)
    {
        if(!_isInTree && CurrentIndex != 3)_panelsScene[CurrentIndex].SetActive(false);
        _panelsScene[newIndex].SetActive(true);
    }
    private void SetPlayerPos(Vector3 pos)
    {
        if(_player == null) _player = GameObject.Find("Square(Clone)");
        pos.z = 0;
        _player.GetComponent<CharacterController>().enabled = false; 
        _player.transform.position = pos;
        _player.GetComponent<CharacterController>().enabled = true;
    }

    private void SetCam(int index, bool canMove)
    {
        if (_player == null) _player = GameObject.Find("Square(Clone)");
        Camera[] cams = _panelsScene[index].GetComponentsInChildren<Camera>();
        Camera cam = null;

        if (cams.Count() < 1) cams = _player.GetComponentsInChildren<Camera>();

        foreach (Camera camera in cams)
        {
            if (camera.name != "PlayerCam") cam = camera;
        }

        if (canMove)
        {
            cam.transform.parent = _player.transform;
            cam.transform.localPosition = new Vector3(0, 0, -1);
        }
        else
        {
            cam.transform.parent = _panelsScene[index].transform;
            cam.transform.localPosition = new Vector3(0, 0, -1);
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

    private void SetCameraPos(int indexBiome, int indexPanel)
    {
        if(indexBiome == -1) StartCoroutine(DoMoveCamToBiome(Vector3.zero, _panelsView[indexPanel].transform.position));
        else StartCoroutine(DoMoveCamToBiome(_biomePanels[indexBiome].transform.position, _panelsView[indexPanel].transform.position));
    }

    private void Panel1Enter_Performed(object sender, EventArgs e)
    {
        if (!_CanTeleport) return;
        SetPanel(0, 1);
        SetPlayerPos(_spawnPos[1].position);
        SetCam(1, false);

        Vector3 playerFromPos = _panelsView[0].transform.position;
        Vector3 playerToPos = _panelsView[1].transform.position;
        playerFromPos.x += 19.2f / 2;
        playerToPos.x -= 19.2f / 2;
        SetCameraPos(-1, 1);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel1Exit_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true;
    }

    private void Panel2Enter_Performed(object sender, EventArgs e)
    {
        if (!_CanTeleport) return;
        SetPanel(1,0);
        SetPlayerPos(_spawnPos[0].position);
        SetCam(0, false);

        SetCameraPos(-1,0);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel2Exit_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true; 
    }

    private void Panel2Enter2_Performed(object sender, EventArgs e)
    {
        if (!_CanTeleport) return;
        SetPanel(1, 2);
        SetCam(1, false);
        SetPlayerPos(_spawnPos[3].position);
        SetCam(2, true);

        Vector3 playerFromPos = _panelsView[1].transform.position;
        Vector3 playerToPos = _panelsView[2].transform.position;
        playerFromPos.y -= 10.8f / 2;
        playerToPos.y += 10.8f / 2;
        _biomePanels[1].gameObject.SetActive(false);
        _biomePanels[0].gameObject.SetActive(true);
        SetCameraPos(0,2);

        _CanTeleport = false;
        _teleportedFromTrigger = sender as GameObject;
    }

    private void Panel2Exit2_Performed(object sender, EventArgs e)
    {
        if (sender as GameObject != _teleportedFromTrigger) _CanTeleport = true;
    }

    private void Panel3Enter_Performed(object sender, EventArgs e)
    {
        if (!_CanTeleport) return;
        SetCam(2, false);
        SetPlayerPos(_spawnPos[2].position);
        SetCam(1,true);
        SetPanel(2, 1);

        Vector3 playerFromPos = _panelsView[2].transform.position;
        Vector3 playerToPos = _panelsView[1].transform.position;
        playerFromPos.y += 10.8f / 2;
        playerToPos.y -= 10.8f / 2;
        _biomePanels[0].gameObject.SetActive(false);
        _biomePanels[1].gameObject.SetActive(true);
        SetCameraPos(1,1);

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

    private IEnumerator DoMoveCamToPanel(Vector3 newCamPos)
    {
        _staminaBar.SetActive(false);
        float camSize = _camera.orthographicSize;
        newCamPos.z = -1;
        float time = 0;
        Vector3 startpos = _camera.transform.position;
        while(_camera.orthographicSize < camSize + 0.76f)
        {
            _camera.orthographicSize += _camZoomSpeed * Time.deltaTime;
            yield return null;
        }
        _camera.orthographicSize = camSize + 0.76f;

        while (Vector3.Distance(_camera.transform.position, newCamPos) > 0.2f)
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
        if (_player != null && !_isInTree) _player.GetComponent<CharacterController>().enabled = true;
        _staminaBar.SetActive(true);
    }
    private IEnumerator DoMoveCamToBiome(Vector3 newCamPosBiome, Vector3 newCamPosPanel)
    {
        if(newCamPosBiome != Vector3.zero)
        {
            if (_player != null) _player.GetComponent<CharacterController>().enabled = false;
            _staminaBar.SetActive(false);
            float camSize = _camera.orthographicSize;
            newCamPosBiome.z = -1;
            float time = 0;
            Vector3 startpos = _camera.transform.position;
            while (_camera.orthographicSize < camSize + 0.76f)
            {
                _camera.orthographicSize += _camZoomSpeed * Time.deltaTime;
                yield return null;
            }
            _camera.orthographicSize = camSize + 0.76f;

            while (Vector3.Distance(_camera.transform.position, newCamPosBiome) > 0.2f)
            {
                time += Time.deltaTime;
                _camera.transform.position = Vector3.Lerp(startpos, newCamPosBiome, _camMoveSpeed * time);
                yield return null;
            }
            _camera.transform.position = newCamPosBiome;

            while (_camera.orthographicSize > camSize + 0.76f)
            {
                _camera.orthographicSize -= _camZoomSpeed * Time.deltaTime;
                yield return null;
            }
            _camera.orthographicSize = camSize;
            _staminaBar.SetActive(true);

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(DoMoveCamToPanel(newCamPosPanel));
        }
        else StartCoroutine(DoMoveCamToPanel(newCamPosPanel));
    }

    private IEnumerator ResetShowdownPanel()
    {
        yield return new WaitForSeconds(3);
        _showdownPanel.SetActive(false);
    }
}
