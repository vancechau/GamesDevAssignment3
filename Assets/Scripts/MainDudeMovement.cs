using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDudeMovement : MonoBehaviour
{
    Vector2[] locations = {
        new Vector2(1, 27),
        new Vector2(6, 27),
        new Vector2(6, 23),
        new Vector2(1, 23),
    };

    public float speed;
    int movingTo = 1;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, locations[movingTo], speed * Time.deltaTime);
        if (transform.position.x == locations[movingTo].x && transform.position.y == locations[movingTo].y)
        {
            if (movingTo == locations.Length)
            {
                movingTo = 0;
            }

            if (movingTo + 1 == locations.Length)
            {
                movingTo = 0;
                return;
            }

            movingTo += 1;
        }
    }
}