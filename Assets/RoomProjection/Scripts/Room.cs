using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RoomProjection
{
    public class Room : MonoBehaviour
    {

        public enum Face
        {
            Front,
            Right,
            Back,
            Left,
            Bottom
        }

        static Dictionary<Face, Quaternion> _faceToRot = new Dictionary<Face, Quaternion>()
    {
        {Face.Front, Quaternion.Euler(0f, 0f, 0f)},
        {Face.Right, Quaternion.Euler(0f, 90f, 0f)},
        {Face.Back, Quaternion.Euler(0f, 180f, 0f)},
        {Face.Left, Quaternion.Euler(0f, 270f, 0f)},
        {Face.Bottom, Quaternion.Euler(90f, 0f, 0f)},
    };

        public static Quaternion FaceToRot(Face face)
        {
            return _faceToRot[face];
        }

        public Vector3 _size = new Vector3(20f, 5f, 10f);

        public class FaceData
        {
            public Face face;
            public Vector2 size;
            public float distance;
        }

        List<FaceData> _faceDatas;
        public List<FaceData> GetFaceDatas()
        {
            if (_faceDatas == null)
            {
                _faceDatas = new FaceData[]{
                new FaceData(){face = Face.Front, size = new Vector2(_size.x, _size.y), distance=_size.z * 0.5f},
                new FaceData(){face = Face.Right, size = new Vector2(_size.z, _size.y), distance=_size.x * 0.5f},
                new FaceData(){face = Face.Back, size = new Vector2(_size.x, _size.y), distance=_size.z * 0.5f},
                new FaceData(){face = Face.Left, size = new Vector2(_size.z, _size.y), distance=_size.x * 0.5f},
                new FaceData(){face = Face.Bottom, size = new Vector2(_size.x, _size.z), distance=_size.y * 0.5f},
            }
                .ToList();
            }

            return _faceDatas;
        }

        public Dictionary<Face, GameObject> faces { get; protected set; } = new Dictionary<Face, GameObject>();

        void Awake()
        {
            var center = Vector3.up * _size.y * 0.5f + transform.position;
            GetFaceDatas().ForEach(data =>
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                go.name = data.face.ToString();
                go.layer = gameObject.layer;

                faces[data.face] = go;

                var trans = go.transform;
                trans.SetParent(transform);
                var rot = FaceToRot(data.face);
                trans.localScale = new Vector3(data.size.x, data.size.y, 1);
                trans.SetPositionAndRotation(
                    rot * Vector3.forward * data.distance + center,
                    rot);
            });
        }
    }
}