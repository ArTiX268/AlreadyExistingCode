using Godot;

// Author : Aidan Bachelez

namespace ArTiX.Tools
{
	public static partial class Translation
	{
		public const string ENGLISH_ACRO = "en";
		public const string FRENCH_ACRO = "fr";

		static Translation()
		{
			TranslationServer.SetLocale(ENGLISH_ACRO);
        }

		public static void ToggleLanguage()
		{
			TranslationServer.SetLocale(TranslationServer.GetLocale() == ENGLISH_ACRO ? FRENCH_ACRO : ENGLISH_ACRO);
		}
	}
}