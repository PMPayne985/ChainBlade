using UnityEngine;

public class ShiftPoints : MonoBehaviour
{
    [SerializeField] private GameObject[] allPoints;
    [SerializeField] private float shiftTime;
    [SerializeField] private GameObject pointPrefab;

    [SerializeField]
    private GameObject perceiverGroupObject;
    private int _pointIndex;
    private float _shiftTimer;

    private GameObject _currentPoint;
    private GameObject _lastPoint;

    private void Start()
    {
        _shiftTimer = shiftTime;
    }

    void Update()
    {
        _shiftTimer -= Time.deltaTime;
        if (_shiftTimer <= 0)
        {
            _lastPoint = _currentPoint;
            _currentPoint = Instantiate(pointPrefab, allPoints[_pointIndex].transform.position,
                allPoints[_pointIndex].transform.rotation, perceiverGroupObject.transform);
            _pointIndex++;
            if (_pointIndex >= allPoints.Length)
                _pointIndex = 0;
            
            if (_lastPoint)
                Destroy(_lastPoint);

        }
    }
}
