namespace CryptoTrainer
{
    /// <summary>
    /// Szyfr Vigenère’a dla alfabetu A–Z. Wynik zapisujemy wielkimi literami.
    /// Spacje i inne znaki spoza alfabetu pozostają na miejscu i nie przesuwają klucza.
    /// </summary>
    public static class SzyfrVigenere
    {
        public static string Szyfruj(string tekst, string klucz)
        {
            return Przeksztalc(tekst, klucz, odszyfruj: false);
        }

        public static string Odszyfruj(string tekst, string klucz)
        {
            return Przeksztalc(tekst, klucz, odszyfruj: true);
        }

        private static string Przeksztalc(string tekst, string klucz, bool odszyfruj)
        {
            ArgumentNullException.ThrowIfNull(tekst);
            ArgumentNullException.ThrowIfNull(klucz);

            if (klucz.Length == 0 || klucz.Any(c => !JestLitera(c)))
                throw new ArgumentException("Klucz musi zawierać wyłącznie litery A–Z i nie może być pusty.", nameof(klucz));

            klucz = klucz.ToUpperInvariant();
            char[] wynik = tekst.ToCharArray();
            int indeksKlucza = 0;

            for (int i = 0; i < wynik.Length; i++)
            {
                if (!JestLitera(wynik[i]))
                    continue;

                int litera = char.ToUpperInvariant(wynik[i]) - 'A';
                int przesuniecie = klucz[indeksKlucza] - 'A';
                wynik[i] = (char)('A' + (odszyfruj
                    ? (litera - przesuniecie + 26) % 26
                    : (litera + przesuniecie) % 26));

                indeksKlucza = (indeksKlucza + 1) % klucz.Length;
            }

            return new string(wynik);
        }

        private static bool JestLitera(char znak)
        {
            return znak is >= 'A' and <= 'Z' or >= 'a' and <= 'z';
        }
    }
}
