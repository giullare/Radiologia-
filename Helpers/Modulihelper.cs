namespace RadiologiaAppNew.Helpers
{
    /// <summary>
    /// Definisce i 4 moduli del sistema e la logica di assegnazione default per ruolo.
    /// </summary>
    public static class ModuliHelper
    {
        // Costanti moduli
        public const string MOD1 = "MOD1"; // Apparecchiature Radiologiche
        public const string MOD2 = "MOD2"; // Risonanza Magnetica
        public const string MOD3 = "MOD3"; // Registri Medicina Nucleare
        public const string MOD4 = "MOD4"; // Radioterapia Metabolica Lu177

        public static readonly IReadOnlyList<string> TuttiIModuli =
            new[] { MOD1, MOD2, MOD3, MOD4 };

        public static readonly IReadOnlyDictionary<string, string> NomiModuli =
            new Dictionary<string, string>
            {
                [MOD1] = "Modulo 1 — Apparecchiature Radiologiche",
                [MOD2] = "Modulo 2 — Risonanza Magnetica",
                [MOD3] = "Modulo 3 — Registri Medicina Nucleare",
                [MOD4] = "Modulo 4 — Radioterapia Metabolica Lu177",
            };

        public static readonly IReadOnlyDictionary<string, string> IconeModuli =
            new Dictionary<string, string>
            {
                [MOD1] = "bi-radioactive",
                [MOD2] = "bi-magnet",
                [MOD3] = "bi-activity",
                [MOD4] = "bi-heart-pulse",
            };

        /// <summary>
        /// Restituisce i moduli di default in base alla lista di ruoli dell'utente.
        /// ADMIN_ORG e SUPER_ADMIN vedono tutto.
        /// </summary>
        public static List<string> GetDefaultModuliPerRuoli(IList<string> ruoli)
        {
            if (ruoli.Contains("ADMIN_ORG") || ruoli.Contains("SUPER_ADMIN"))
                return TuttiIModuli.ToList();

            var moduli = new HashSet<string>();

            foreach (var ruolo in ruoli)
            {
                switch (ruolo)
                {
                    case "EFM":
                    case "SFM":
                    case "RIR":
                        // Fisica medica: apparecchiature radiologiche + Lu177
                        moduli.Add(MOD1);
                        moduli.Add(MOD4);
                        break;

                    case "EDR":
                        // Radioprotezione: tutti i moduli tecnici
                        moduli.Add(MOD1);
                        moduli.Add(MOD2);
                        moduli.Add(MOD3);
                        break;

                    case "ES":
                    case "MA":
                    case "MR":
                        // Specialisti RM
                        moduli.Add(MOD2);
                        break;

                    case "RQ":
                        // Responsabile qualità: tutto in lettura
                        moduli.Add(MOD1);
                        moduli.Add(MOD2);
                        moduli.Add(MOD3);
                        moduli.Add(MOD4);
                        break;

                    case "OPERATORE":
                        // Operatore: solo registri MN
                        moduli.Add(MOD3);
                        break;

                    case "READER":
                        // Sola lettura: MOD1 di default
                        moduli.Add(MOD1);
                        break;
                }
            }

            return moduli.OrderBy(m => m).ToList();
        }

        /// <summary>
        /// Calcola i moduli effettivi da mostrare:
        /// se l'utente ha ModuliAbilitati espliciti li usa,
        /// altrimenti usa il default da ruolo.
        /// </summary>
        public static List<string> GetModuliEffettivi(
            string? moduliAbilitati,
            IList<string> ruoli)
        {
            if (!string.IsNullOrWhiteSpace(moduliAbilitati))
            {
                var espliciti = moduliAbilitati
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(m => m.Trim())
                    .Where(m => TuttiIModuli.Contains(m))
                    .ToList();

                if (espliciti.Any())
                    return espliciti;
            }

            return GetDefaultModuliPerRuoli(ruoli);
        }
    }
}