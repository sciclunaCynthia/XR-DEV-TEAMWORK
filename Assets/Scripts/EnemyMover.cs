using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyMover : MonoBehaviour
{
    public float speed = 1.5f;
    public float arriveDistance = 0.15f;

    private WaypointPath _path;
    private int _index;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = false;

        // Prevent physics tipping/launching
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public void Init(WaypointPath path)
    {
        _path = path;
        _index = 0;

        // Snap to first waypoint
        if (_path != null && _path.Count > 0)
            transform.position = _path.Get(0).position;
    }

    private void FixedUpdate()
    {
        if (_path == null || _path.Count == 0) return;
        if (_index >= _path.Count) return;

        Transform target = _path.Get(_index);
        Vector3 toTarget = target.position - transform.position;

        if (toTarget.magnitude <= arriveDistance)
        {
            _index++;
            if (_index >= _path.Count)
            {
                Destroy(gameObject);
                return;
            }

            target = _path.Get(_index);
            toTarget = target.position - transform.position;
        }

        Vector3 step = toTarget.normalized * speed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + step);

        // Optional facing
        if (toTarget.sqrMagnitude > 0.001f)
            transform.forward = Vector3.Lerp(transform.forward, toTarget.normalized, 0.15f);
    }
}
