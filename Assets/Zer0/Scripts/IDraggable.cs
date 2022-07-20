using UnityEngine;

namespace Zer0
{
    public interface IDraggable
    {
        public void Drag(Transform dragger);
        public void ReleaseTarget();
    }
}
