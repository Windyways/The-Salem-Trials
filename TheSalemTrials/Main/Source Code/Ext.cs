using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppRewired.Glyphs;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static MelonLoader.MelonLaunchOptions;

namespace TheSalemTrials
{
    public static class Ext
    {
        public static bool IsDisguising(this Character c) => c.bluff;
        public static bool IsCorrupted(this Character c) => c.statuses.Contains(ECharacterStatus.Corrupted);
        public static bool IsLying(this Character c)
        {
            bool isLying = false;

            if (c.bluff != null)
                isLying = true;

            if (c.statuses.Contains(ECharacterStatus.Corrupted))
                isLying = true;

            if (c.statuses.Contains(ECharacterStatus.HealthyBluff))
                isLying = false;

            if (c.dataRef.role is Confessor)
                isLying = false;
            if (c.bluff != null)
                if (c.bluff.role is Confessor)
                    isLying = false;

            return isLying;
        }

        /// <summary>
        /// Used to ensure Lying characters do not say the Truth.
        /// </summary>
        /// <param name="trueNumber"></param>
        /// <param name="falseNumber"></param>
        /// <param name="minimum"></param>
        /// <returns></returns>
        public static int MakeNumberWrong(int trueNumber, int falseNumber, int minimum)
        {
            int returnVal = falseNumber;
            if (trueNumber != falseNumber) return falseNumber;
            if (falseNumber == minimum) returnVal++;
            else returnVal--;
            return returnVal;
        }
    }
}
