using Il2Cpp;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;

namespace TheSalemTrials
{
    public class Sibyl : Role 
    {
        public Sibyl() : base(ClassInjector.DerivedConstructorPointer<Sibyl>())
        {
            ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
        }
        public Sibyl(System.IntPtr ptr) : base(ptr)
        {

        }
        public override ActedInfo GetInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> displaychars = new Il2CppSystem.Collections.Generic.List<Character>();
            displaychars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            displaychars = Characters.Instance.FilterHiddenCharacters(displaychars);
            displaychars.Remove(charRef);

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterHiddenCharacters(chars);
            chars = Characters.Instance.FilterAlignmentCharacters(chars, EAlignment.Evil);
            chars.Remove(charRef);

            int revealedEvils = chars.Count;

            string info = ConjourInfo(revealedEvils);
            ActedInfo newInfo = new ActedInfo(info, displaychars);
            return newInfo;
        }

        public override ActedInfo GetBluffInfo(Character charRef)
        {
            Il2CppSystem.Collections.Generic.List<Character> displaychars = new Il2CppSystem.Collections.Generic.List<Character>();
            displaychars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            displaychars = Characters.Instance.FilterHiddenCharacters(displaychars);
            displaychars.Remove(charRef);

            Il2CppSystem.Collections.Generic.List<Character> chars = new Il2CppSystem.Collections.Generic.List<Character>();
            chars = Characters.Instance.FilterAliveCharacters(Gameplay.CurrentCharacters);
            chars = Characters.Instance.FilterHiddenCharacters(chars);
            chars = Characters.Instance.FilterAlignmentCharacters(chars, EAlignment.Evil);
            chars.Remove(charRef);

            int revealedEvils = Ext.MakeNumberWrong(chars.Count, 2, 1);

            string info = ConjourInfo(revealedEvils);
            ActedInfo newInfo = new ActedInfo(info, displaychars);

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

        public string ConjourInfo(int hiddenEvils)
        {
            if (hiddenEvils == 0) return $"There are {hiddenEvils}\nHidden Evils.";
            else if (hiddenEvils == 1) return $"There is {hiddenEvils}\nHidden Evil.";
            return $"There are {hiddenEvils}\nHidden Evils.";
        }
    }
}
