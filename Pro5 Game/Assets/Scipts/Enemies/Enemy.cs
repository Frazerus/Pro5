using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    public int BaseDist;
    public int EnemyType;

    [SerializeField] private int savedBeats = 16;


    private float beatTime;
    private Vector3 dir;
    private GameObject player;
    private int moved = 0;
    private Material material;
    private bool moving = false;

    private float[] enemySize =
    {
        0.5f,
        0.5f,
        0.5f,
        0.5f,
    };

    void Start()
    {
        material = this.GetComponent<MeshRenderer>().material;
        Constants.changeCol(material, EnemyType);


        player = GameObject.Find("Player");
        dir = player.transform.position - this.transform.position;
        dir.y = 0;
        dir = dir.normalized;


        float angle = Vector3.Angle(transform.position, Vector3.forward);
        transform.rotation = Quaternion.Euler(0, angle, 0);

        BeatMachine.current.onBeat += Move;
        BeatMachine.current.onAttack += Attacked;
        
    }

    private void Update()   
    {
        if (moving)
        {
            //Lineare Bewegung
            float inBeatDiff = (Time.time - beatTime) / (float)BeatMachine.current.beatSec;
            //jumping
            //inBeatDiff = 0;
            transform.position = -dir * (player.GetComponent<Player>().PerfectKillZone + speed * (BaseDist - moved - inBeatDiff));
        }
    }



    private void Move()
    {
        if (!moving)
        {
            moving = true;  
        }
        beatTime = Time.time;

        if(moved >= BaseDist)
        {
            moving = false;

            BeatMachine.current.Rating(0);

            EnemyKilled();

        }
        moved++;
        //transform.position += dir * speed;

    }


    private void Attacked(int type)
    {
        if(type == EnemyType && BaseDist - moved <  1.5f)
        {
            BeatMachine.current.createRatingAndSend();
            EnemyKilled();
        }
    }

    public int GetEnemyType()
    {
        return EnemyType;
    }

    public int GetSavedBeats()
    {
        return savedBeats;
    }

    private void EnemyKilled()
    {
        BeatMachine.current.onBeat -= Move;
        BeatMachine.current.onAttack -= Attacked;

        
        BeatMachine.current.Killed(gameObject);

        Destroy(gameObject);
    }

}