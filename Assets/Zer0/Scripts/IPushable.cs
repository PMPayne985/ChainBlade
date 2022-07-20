using UnityEngine;

namespace Zer0
{
    public interface IPushable
    {
        public void Push(Transform pusher, float force);
    }
}
