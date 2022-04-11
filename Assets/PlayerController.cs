using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool GodMode = false;
    public int Force = 10;
    public float TimeScale = 1f;
    public SideScrolling SideScrolling;
    public Sprite Normal;
    public Sprite Flap;
    public SpriteRenderer SpriteRenderer;
    public Text Text;
    public GameObject PauseMenuUI;
    public GameObject DeadMenuUI;
    public GameObject HandUI;
    public AudioSource JumpAudioSource;
    public AudioSource DeathAudioSource;

    private bool _started = false;
    private bool _paused = false;
    private bool _alive = true;
    private bool _wasAlive = true;

    private float _lastFlap = 0;

    private int _score = -1;



    // ref vars (get in Start)
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Time.timeScale = TimeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!_paused);
        }

        if (_paused) return;

        if (_wasAlive && !_alive)
        {
            Debug.Log("Dead");
            _rb.simulated = false;
            _wasAlive = false;
            SideScrolling.Finished = true;
            DeathAudioSource.Play();
            SetDead();
        }

        var input = ISeeInput();
        if (_alive && input)
        {
            _rb.velocity = new Vector2(0, Force);
            _lastFlap = 0.0f;
            SpriteRenderer.sprite = Flap;
            transform.rotation = Quaternion.Euler(0, 0, 20);
            JumpAudioSource.Play();
        }
        else if (_alive && !(_lastFlap<0))
        {
            _lastFlap += Time.deltaTime;
            if (_lastFlap > 0.3f)
            {
                _lastFlap = -1f;
                SpriteRenderer.sprite = Normal;
            }
        }
        if (!_started && input) {
            SideScrolling.Started = true;
            _started = true;
            _rb.simulated = true && !GodMode;
            HandUI.SetActive(false);
        }

        if (_alive)
        {
            var val = Mathf.Clamp(_rb.velocity.y / -200, 0, 1);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(-80, Vector3.forward), val);
        }

        if (_started && _score != SideScrolling.Score)
        {
            _score = SideScrolling.Score;
            Text.text = _score.ToString();
        }
    }

    public void SetPause(bool pause = true)
    {
        if (!_alive) return;
        SideScrolling.Paused = pause;
        _paused = pause;
        PauseMenuUI.SetActive(pause);
        Time.timeScale = pause ? 0 : TimeScale;
    }

    public void SetDead()
    {
        DeadMenuUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    bool ISeeInput()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButtonDown(0))
            return true;
        return Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.Space);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("BadCollider"))
            return;

        _alive = false || GodMode;
    }
}
