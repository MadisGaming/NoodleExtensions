﻿using IPA.Utilities;
using UnityEngine;
using static NoodleExtensions.HarmonyPatches.SpawnDataHelper.BeatmapObjectSpawnMovementDataVariables;

namespace NoodleExtensions.HarmonyPatches
{
    internal class SpawnDataHelper
    {
        internal static Vector3 GetNoteOffset(BeatmapObjectData beatmapObjectData, float? startRow, float? startHeight)
        {
            float distance = -(_noteLinesCount - 1f) * 0.5f + (startRow.HasValue ? _noteLinesCount / 2f : 0); // Add last part to simulate https://github.com/spookyGh0st/beatwalls/#wall
            float lineIndex = startRow.GetValueOrDefault(beatmapObjectData.lineIndex);
            distance = (distance + lineIndex) * _noteLinesDistance;

            return _rightVec * distance
                + new Vector3(0, LineYPosForLineLayer(beatmapObjectData, startHeight), 0);
        }

        internal static float LineYPosForLineLayer(BeatmapObjectData beatmapObjectData, float? height)
        {
            float ypos = _baseLinesYPos;
            if (height.HasValue)
                ypos = (height.Value * _noteLinesDistance) + _baseLinesYPos; // offset by 0.25
            else if (beatmapObjectData is NoteData noteData)
                ypos = beatmapObjectSpawnMovementData.LineYPosForLineLayer(noteData.noteLineLayer);
            return ypos;
        }

        internal static void GetNoteJumpValues(float? inputNoteJumpMovementSpeed, float? inputNoteJumpStartBeatOffset, out float localJumpDuration,
            out float localJumpDistance, out Vector3 localMoveStartPos, out Vector3 localMoveEndPos, out Vector3 localJumpEndPos)
        {
            float localNoteJumpMovementSpeed = inputNoteJumpMovementSpeed ?? _noteJumpMovementSpeed;
            float localNoteJumpStartBeatOffset = inputNoteJumpStartBeatOffset ?? _noteJumpStartBeatOffset;
            float num = 60f / _startBPM;
            float num2 = _startHalfJumpDurationInBeats;
            while (localNoteJumpMovementSpeed * num * num2 > _maxHalfJumpDistance)
            {
                num2 /= 2f;
            }
            num2 += localNoteJumpStartBeatOffset;
            if (num2 < 1f) num2 = 1f;
            localJumpDuration = num * num2 * 2f;
            localJumpDistance = localNoteJumpMovementSpeed * localJumpDuration;
            localMoveStartPos = _centerPos + _forwardVec * (_moveDistance + localJumpDistance * 0.5f);
            localMoveEndPos = _centerPos + _forwardVec * localJumpDistance * 0.5f;
            localJumpEndPos = _centerPos - _forwardVec * localJumpDistance * 0.5f;
        }

        internal static void InitBeatmapObjectSpawnController(BeatmapObjectSpawnMovementData beatmapObjectSpawnMovementData)
        {
            BeatmapObjectSpawnMovementDataVariables.beatmapObjectSpawnMovementData = beatmapObjectSpawnMovementData;
        }

        internal static class BeatmapObjectSpawnMovementDataVariables
        {
            internal static BeatmapObjectSpawnMovementData beatmapObjectSpawnMovementData;
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _startBPMAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_startBPM");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _topObstaclePosYAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_topObstaclePosY");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _jumpOffsetYAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_jumpOffsetY");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _verticalObstaclePosYAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_verticalObstaclePosY");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _moveDistanceAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_moveDistance");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _jumpDistanceAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_jumpDistance");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _jumpDurationAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_jumpDuration");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _noteJumpMovementSpeedAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_noteJumpMovementSpeed");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _noteJumpStartBeatOffsetAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_noteJumpStartBeatOffset");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _noteLinesDistanceAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_noteLinesDistance");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _baseLinesYPosAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_baseLinesYPos");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _moveStartPosAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_moveStartPos");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _moveEndPosAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_moveEndPos");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _jumpEndPosAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_jumpEndPos");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _noteLinesCountAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_noteLinesCount");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _centerPosAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_centerPos");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _forwardVecAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_forwardVec");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.Accessor _rightVecAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, Vector3>.GetAccessor("_rightVec");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _startHalfJumpDurationInBeatsAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_startHalfJumpDurationInBeats");
            private static readonly FieldAccessor<BeatmapObjectSpawnMovementData, float>.Accessor _maxHalfJumpDistanceAccessor = FieldAccessor<BeatmapObjectSpawnMovementData, float>.GetAccessor("_maxHalfJumpDistance");
            internal static float _startBPM { get => _startBPMAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _topObstaclePosY { get => _topObstaclePosYAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _jumpOffsetY { get => _jumpOffsetYAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _verticalObstaclePosY { get => _verticalObstaclePosYAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _moveDistance { get => _moveDistanceAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _jumpDistance { get => _jumpDistanceAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _jumpDuration { get => _jumpDurationAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _noteJumpMovementSpeed { get => _noteJumpMovementSpeedAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _noteJumpStartBeatOffset { get => _noteJumpStartBeatOffsetAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _noteLinesDistance { get => _noteLinesDistanceAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _baseLinesYPos { get => _baseLinesYPosAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _moveStartPos { get => _moveStartPosAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _moveEndPos { get => _moveEndPosAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _jumpEndPos { get => _jumpEndPosAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _noteLinesCount { get => _noteLinesCountAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _centerPos { get => _centerPosAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _forwardVec { get => _forwardVecAccessor(ref beatmapObjectSpawnMovementData); }
            internal static Vector3 _rightVec { get => _rightVecAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _startHalfJumpDurationInBeats { get => _startHalfJumpDurationInBeatsAccessor(ref beatmapObjectSpawnMovementData); }
            internal static float _maxHalfJumpDistance { get => _maxHalfJumpDistanceAccessor(ref beatmapObjectSpawnMovementData); }
        }
    }
}