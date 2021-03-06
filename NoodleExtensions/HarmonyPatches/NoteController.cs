﻿using CustomJSONData;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;
using IPA.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using static NoodleExtensions.Plugin;

namespace NoodleExtensions.HarmonyPatches
{
    [NoodlePatch(typeof(NoteController))]
    [NoodlePatch("Update")]
    internal class NoteControllerUpdate
    {
        internal static NoteData _cachedNoteData;

        private static void Prefix(NoteData ____noteData)
        {
            _cachedNoteData = ____noteData;
        }
    }

    [NoodlePatch(typeof(NoteController))]
    [NoodlePatch("Init")]
    internal class NoteControllerInit
    {
        private static readonly FieldAccessor<NoteMovement, NoteJump>.Accessor _noteJumpAccessor = FieldAccessor<NoteMovement, NoteJump>.GetAccessor("_jump");
        private static readonly FieldAccessor<NoteMovement, NoteFloorMovement>.Accessor _noteFloorMovementAccessor = FieldAccessor<NoteMovement, NoteFloorMovement>.GetAccessor("_floorMovement");

        private static readonly FieldAccessor<NoteJump, Quaternion>.Accessor _endRotationAccessor = FieldAccessor<NoteJump, Quaternion>.GetAccessor("_endRotation");
        private static readonly FieldAccessor<NoteJump, Quaternion>.Accessor _middleRotationAccessor = FieldAccessor<NoteJump, Quaternion>.GetAccessor("_middleRotation");
        private static readonly FieldAccessor<NoteJump, Vector3[]>.Accessor _randomRotationsAccessor = FieldAccessor<NoteJump, Vector3[]>.GetAccessor("_randomRotations");
        private static readonly FieldAccessor<NoteJump, int>.Accessor _randomRotationIdxAccessor = FieldAccessor<NoteJump, int>.GetAccessor("_randomRotationIdx");
        private static readonly FieldAccessor<NoteJump, Quaternion>.Accessor _worldRotationJumpAccessor = FieldAccessor<NoteJump, Quaternion>.GetAccessor("_worldRotation");
        private static readonly FieldAccessor<NoteJump, Quaternion>.Accessor _inverseWorldRotationJumpAccessor = FieldAccessor<NoteJump, Quaternion>.GetAccessor("_inverseWorldRotation");

        private static readonly FieldAccessor<NoteFloorMovement, Quaternion>.Accessor _worldRotationFloorAccessor = FieldAccessor<NoteFloorMovement, Quaternion>.GetAccessor("_worldRotation");
        private static readonly FieldAccessor<NoteFloorMovement, Quaternion>.Accessor _inverseWorldRotationFloorAccessor = FieldAccessor<NoteFloorMovement, Quaternion>.GetAccessor("_inverseWorldRotation");

        private static void Postfix(NoteController __instance, NoteData noteData, NoteMovement ____noteMovement)
        {
            if (noteData is CustomNoteData customData)
            {
                dynamic dynData = customData.customData;

                float? cutDir = (float?)Trees.at(dynData, CUTDIRECTION);

                NoteJump noteJump = _noteJumpAccessor(ref ____noteMovement);
                NoteFloorMovement floorMovement = _noteFloorMovementAccessor(ref ____noteMovement);

                if (cutDir.HasValue)
                {
                    Quaternion rotation = Quaternion.Euler(0, 0, cutDir.Value);
                    _endRotationAccessor(ref noteJump) = rotation;
                    Vector3 vector = rotation.eulerAngles;
                    vector += _randomRotationsAccessor(ref noteJump)[_randomRotationIdxAccessor(ref noteJump)] * 20;
                    Quaternion midrotation = Quaternion.Euler(vector);
                    _middleRotationAccessor(ref noteJump) = midrotation;
                }
            }
        }

        private static readonly MethodInfo _getFlipYSide = SymbolExtensions.GetMethodInfo(() => GetFlipYSide(null, 0));

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            bool foundFlipYSide = false;
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (!foundFlipYSide &&
                    instructionList[i].opcode == OpCodes.Callvirt &&
                    ((MethodInfo)instructionList[i].operand).Name == "get_flipYSide")
                {
                    foundFlipYSide = true;

                    instructionList.Insert(i + 1, new CodeInstruction(OpCodes.Call, _getFlipYSide));
                    instructionList.Insert(i - 2, new CodeInstruction(OpCodes.Ldarg_0));
                    instructionList.Insert(i - 1, new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(NoteController), "_noteData")));
                }
            }
            if (!foundFlipYSide) Logger.Log("Failed to find Get_flipYSide call, ping Aeroluna!", IPA.Logging.Logger.Level.Error);
            return instructionList.AsEnumerable();
        }

        private static float GetFlipYSide(NoteData noteData, float @default)
        {
            float output = @default;
            if (noteData is CustomNoteData customData)
            {
                dynamic dynData = customData.customData;

                float? flipYSide = (float?)Trees.at(dynData, "flipYSide");
                if (flipYSide.HasValue) output = flipYSide.Value;
            }
            return output;
        }
    }
}