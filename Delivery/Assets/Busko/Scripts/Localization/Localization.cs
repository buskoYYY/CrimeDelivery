using Knot.Localization;
using UnityEngine;

namespace ArcadeBridge
{
    public class Localization : MonoBehaviour
    {
        public void SelectEnglishLanguage()
        {
            KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[0]);
        }

        public void SelectRussianLanguage()
        {
            KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[1]);
        }
    }
}
