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
        _rb.isKinematic = true;

        // Prevent physics tipping/launching
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("Enemy"),
            LayerMask.NameToLayer("Enemy")
        );




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
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.isKinematic = true;

                Destroy(gameObject);
                return;
            }

            target = _path.Get(_index);
            toTarget = target.position - transform.position;
        }

        // Move along path
        Vector3 step = toTarget.normalized * speed * Time.fixedDeltaTime;
        
        Vector3 newPos = transform.position + step;
        // Snap to ground
        if (Physics.Raycast(transform.position + step + Vector3.up, Vector3.down, out RaycastHit hit, 5f))
        {
            newPos.y = hit.point.y;
        }

        _rb.MovePosition(transform.position + step);

        // Keep upright while facing forward
        Vector3 forwardFlat = new Vector3(toTarget.x, 0, toTarget.z).normalized;
        if (forwardFlat.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(forwardFlat, Vector3.up);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRot, 0.15f));
        }
    }
}
