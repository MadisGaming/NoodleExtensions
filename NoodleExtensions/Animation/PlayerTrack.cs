﻿namespace NoodleExtensions.Animation
{
    using System.Linq;
    using IPA.Utilities;
    using UnityEngine;
    using static NoodleExtensions.Plugin;

    internal class PlayerTrack : MonoBehaviour
    {
        private static readonly FieldAccessor<PauseController, bool>.Accessor PauseBool = FieldAccessor<PauseController, bool>.GetAccessor("_paused");
        private static PlayerTrack _instance;
        private static Track _track;
        private static GameObject _origin;
        private static PauseController _pauseController;

        internal static void AssignTrack(Track track)
        {
            if (_instance == null)
            {
                _origin = GameObject.Find("GameCore/Origin");
                _instance = _origin.AddComponent<PlayerTrack>();
                _pauseController = FindObjectOfType<PauseController>();
            }

            _track = track;
        }

        private void Update()
        {
            bool paused = PauseBool(ref _pauseController);

            Transform transform = _origin.transform;
            object position = _track.Properties[POSITION].Value;
            if (position != null && !paused)
            {
                transform.localPosition = (Vector3)position;
            }
            else
            {
                transform.localPosition = _vectorZero;
            }

            object rotation = _track.Properties[ROTATION].Value;
            if (rotation != null && !paused)
            {
                transform.localRotation = (Quaternion)rotation;
            }
            else
            {
                transform.localRotation = _quaternionIdentity;
            }
        }
    }
}