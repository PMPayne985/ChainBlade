using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {
    public float radius = 5;
    public float velocity = 2f;
    Vector3[] points;
    Vector3[] positions;
    int[] starts;
    Transform[] trails;
	// Use this for initialization
	void Start () {
        points = new Vector3[5];
        positions = new Vector3[5];
        trails = new Transform[5];
        starts = new int[] { 0, 1, 2, 3, 4 };
        points[0] = new Vector3(Mathf.Cos(18 * Mathf.Deg2Rad), Mathf.Sin(18 * Mathf.Deg2Rad)) * radius;
        points[1] = new Vector3(-Mathf.Cos(54 * Mathf.Deg2Rad), -Mathf.Sin(54 * Mathf.Deg2Rad)) * radius;
        points[2] = new Vector3(0, 1) * radius;
        points[3] = new Vector3(Mathf.Cos(54 * Mathf.Deg2Rad), -Mathf.Sin(54 * Mathf.Deg2Rad)) * radius;
        points[4] = new Vector3(-Mathf.Cos(18 * Mathf.Deg2Rad), Mathf.Sin(18 * Mathf.Deg2Rad)) * radius;
        for (int i = 0; i < 5; i++)
        {
            trails[i] = transform.GetChild(i);
            positions[i] = points[i];
            trails[i].position = transform.TransformPoint(positions[i]);
            trails[i].gameObject.SetActive(true);
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        int next = 1;
        Vector3 direction;
        for (int i = 0; i < 5; i++)
        {
            next = starts[i] + 1;
            if (next > 4) next = 0;
            direction = transform.TransformDirection(points[next] - positions[i]);
            positions[i] = Vector3.MoveTowards(positions[i], points[next], velocity * Time.deltaTime);
            trails[i].position = transform.TransformPoint(positions[i]);
            if (direction != Vector3.zero)
            {
                trails[i].rotation = Quaternion.RotateTowards(trails[i].rotation, Quaternion.LookRotation(direction, transform.forward), Time.deltaTime * 10000);
            }
        }
        if (trails[4].position == transform.TransformPoint(points[next]))
        {
            for (int j = 0; j < 5; j++)
            {
                starts[j]++;
                if (starts[j] > 4) starts[j] = 0;
            }
        }
	}
}
