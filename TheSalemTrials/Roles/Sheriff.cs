using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Sheriff : Role
    {
        public Sheriff() : base(ClassInjector.DerivedConstructorPointer<Sheriff>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Sheriff(System.IntPtr ptr) : base(ptr)
        {

        }

        public override ActedInfo GetInfo(Character charRef)
        {
            int liars = CheckAdjacentLiars(charRef);
            string info = ConjourInfo(liars);
            ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));
            return newInfo;
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            var liars = 0;
            int evils = CheckAdjacentEvils(charRef);
            if (evils == 0) liars = 2;
            else liars = 0;

            string info = ConjourInfo(liars);
            ActedInfo newInfo = new ActedInfo(info, Characters.Instance.GetAdjacentCharacters(charRef));

            return newInfo;
        }

        public override void Act(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetInfo(charRef));
        }

        public override void BluffAct(ETriggerPhase trigger, Character charRef)
        {
            if (trigger != ETriggerPhase.Day) return;
            onActed?.Invoke(GetBluffInfo(charRef));
        }

        public int CheckAdjacentEvils(Character charRef) // Lover code.
        {
            Il2CppSystem.Collections.Generic.List<Character> adjacentCharacters = new Il2CppSystem.Collections.Generic.List<Character>();
            foreach (Character ch in Gameplay.CurrentCharacters)
                if (charRef == ch)
                {
                    adjacentCharacters = Characters.Instance.GetAdjacentCharacters(ch);
                    break;
                }

            int evils = 0;

            foreach (Character ch in adjacentCharacters)
            {
                if (ch.alignment == EAlignment.Evil)
                    evils++;
            }

            return evils;
        }

        public int CheckAdjacentLiars(Character charRef) // Lover code but modified.
        {
            Il2CppSystem.Collections.Generic.List<Character> adjacentCharacters = new Il2CppSystem.Collections.Generic.List<Character>();
            foreach (Character ch in Gameplay.CurrentCharacters)
                if (charRef == ch)
                {
                    adjacentCharacters = Characters.Instance.GetAdjacentCharacters(ch);
                    break;
                }

            int evils = 0;

            foreach (Character ch in adjacentCharacters)
            {
                if (ch.IsLying())
                    evils++;
            }

            return evils;
        }

        public string ConjourInfo(int evils)
        {
            string info = "";
            if (evils == 0)
                info = $"NO Liars\nadjacent to me";
            else if (evils == 1)
                info = $"{evils} Liar\nadjacent to me";
            else
                info = $"{evils} Liars\nadjacent to me";

            return info;
        }
    }
}
