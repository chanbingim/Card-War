using UnityEngine;


namespace Custom.Struct
{
    public struct BoxInfo
    {
        public readonly Vector3 center;
        public readonly Vector3 size;

        public BoxInfo(Vector3 center, Vector3 size)
        {
            this.center = center;
            this.size = size;
        }

        public BoxInfo(BoxCollider collider)
        {
            this.center = collider.center;
            this.size = collider.size;
        }

        public BoxInfo(BoxCollider2D collider)
        {
            this.center = collider.bounds.center;
            this.size = collider.bounds.size;
        }
    }

}