using UnityEngine;

public interface IMovable
{
    bool Moving { get; }

    void MoveTo(Vector3 destination);
    void Stop();
}
