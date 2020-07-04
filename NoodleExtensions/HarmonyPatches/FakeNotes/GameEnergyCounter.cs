﻿namespace NoodleExtensions.HarmonyPatches
{
    using HarmonyLib;

    [NoodlePatch(typeof(GameEnergyCounter))]
    [NoodlePatch("HandleNoteWasCutEvent")]
    internal static class GameEnergyCounterHandleNoteWasCutEvent
    {
        [HarmonyPriority(Priority.High)]
#pragma warning disable SA1313
        private static bool Prefix(INoteController noteController)
#pragma warning restore SA1313
        {
            return FakeNoteHelper.GetFakeNote(noteController);
        }
    }

    [NoodlePatch(typeof(GameEnergyCounter))]
    [NoodlePatch("HandleNoteWasMissedEvent")]
    internal static class GameEnergyCounterHandleNoteWasMissedEvent
    {
        [HarmonyPriority(Priority.High)]
#pragma warning disable SA1313
        private static bool Prefix(INoteController noteController)
#pragma warning restore SA1313
        {
            return FakeNoteHelper.GetFakeNote(noteController);
        }
    }
}