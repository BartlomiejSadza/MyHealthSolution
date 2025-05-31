using MyHealth.Api.Models;
using System.Text;

namespace MyHealth.Api.Services
{
    public partial class AdvancedHealthAnalyzer
    {
        // Lifestyle Analysis Methods
        private string AnalyzeSmokingStatus(string smoke)
        {
            return smoke switch
            {
                "no" => "Niepalący - doskonale dla zdrowia układu oddechowego i sercowo-naczyniowego",
                "yes" => "Palący - znacznie zwiększone ryzyko nowotworów, chorób serca i płuc",
                _ => "Status nieznany"
            };
        }

        private string AnalyzeHealthMonitoring(string scc)
        {
            return scc switch
            {
                "yes" => "Aktywne monitorowanie - świetny nawyk dla kontroli zdrowia",
                "no" => "Brak monitorowania - rozważ śledzenie podstawowych parametrów",
                _ => "Status nieznany"
            };
        }

        private string AnalyzeFamilyHistory(string familyHistory)
        {
            return familyHistory switch
            {
                "yes" => "Obciążenie genetyczne - zwiększona czujność i profilaktyka",
                "no" => "Brak obciążeń rodzinnych - korzystny czynnik genetyczny",
                _ => "Historia nieznana"
            };
        }

        private int CalculateLifestyleScore(HealthRequest request)
        {
            var score = 100;
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var scc = request.DataframeSplit.Data[0][14].ToString();
            var calc = request.DataframeSplit.Data[0][15].ToString();
            var tue = Convert.ToDouble(request.DataframeSplit.Data[0][8]);
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);

            // Palenie
            if (smoke == "yes") score -= 30;

            // Monitorowanie zdrowia
            if (scc == "no") score -= 10;

            // Alkohol
            if (calc == "Frequently") score -= 15;
            else if (calc == "Always") score -= 25;

            // Czas przy technologii
            if (tue > 8) score -= 15;
            else if (tue > 6) score -= 10;

            // Aktywność fizyczna
            if (faf < 1) score -= 20;
            else if (faf < 2) score -= 10;

            return Math.Max(0, score);
        }

        private string EstimateStressLevel(HealthRequest request)
        {
            var stressFactors = 0;
            var tue = Convert.ToDouble(request.DataframeSplit.Data[0][8]);
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var caec = request.DataframeSplit.Data[0][12].ToString();

            if (tue > 8) stressFactors += 2;
            if (faf < 2) stressFactors += 2;
            if (smoke == "yes") stressFactors += 2;
            if (caec == "Frequently") stressFactors += 1;

            return stressFactors switch
            {
                0 => "Niski - dobra równowaga życiowa",
                <= 2 => "Umiarkowany - wprowadź techniki relaksacyjne",
                <= 4 => "Podwyższony - wymagane zarządzanie stresem",
                _ => "Wysoki - pilnie potrzebna interwencja"
            };
        }

        private string EstimateSleepQuality(HealthRequest request)
        {
            var sleepFactors = 0;
            var tue = Convert.ToDouble(request.DataframeSplit.Data[0][8]);
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var calc = request.DataframeSplit.Data[0][15].ToString();

            if (tue > 6) sleepFactors += 1;
            if (faf < 2) sleepFactors += 1;
            if (smoke == "yes") sleepFactors += 2;
            if (calc == "Frequently" || calc == "Always") sleepFactors += 1;

            return sleepFactors switch
            {
                0 => "Prawdopodobnie dobra - zdrowe nawyki",
                <= 2 => "Umiarkowana - możliwe drobne problemy",
                <= 4 => "Obniżona - wprowadź higienę snu",
                _ => "Prawdopodobnie słaba - konsultacja ze specjalistą"
            };
        }

        private List<string> GenerateLifestyleRecommendations(HealthRequest request)
        {
            var recommendations = new List<string>();
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var tue = Convert.ToDouble(request.DataframeSplit.Data[0][8]);
            var calc = request.DataframeSplit.Data[0][15].ToString();

            if (smoke == "yes")
            {
                recommendations.Add("PRIORYTET: Rzuć palenie - skonsultuj się z lekarzem");
                recommendations.Add("Rozważ terapię nikotynową lub alternatywne metody");
            }

            if (tue > 6)
            {
                recommendations.Add("Ogranicz czas ekranowy do maksymalnie 6 godzin dziennie");
                recommendations.Add("Wprowadź 'digital detox' - 1 dzień w tygodniu bez technologii");
            }

            if (calc != "no")
            {
                recommendations.Add("Ogranicz alkohol do maksymalnie 2 jednostek tygodniowo");
            }

            recommendations.Add("Wprowadź codzienną medytację lub techniki oddechowe");
            recommendations.Add("Utrzymuj regularny rytm snu (7-9 godzin)");
            recommendations.Add("Spędzaj więcej czasu na świeżym powietrzu");

            return recommendations;
        }

        // Risk Factor Analysis Methods
        private void AnalyzeWeightRisk(HealthRequest request, List<RiskFactor> riskFactors)
        {
            var height = Convert.ToDouble(request.DataframeSplit.Data[0][2]);
            var weight = Convert.ToDouble(request.DataframeSplit.Data[0][3]);
            var bmi = weight / (height * height);

            if (bmi < 18.5)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Niedowaga",
                    Level = "Średnie",
                    Description = "BMI poniżej normy może prowadzić do niedoborów żywieniowych",
                    Impact = "Osłabiona odporność, problemy hormonalne, osteoporoza",
                    PreventionTips = new List<string> { "Zwiększ spożycie kalorii", "Dodaj białko do diety", "Trening siłowy" }
                });
            }
            else if (bmi > 30)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Otyłość",
                    Level = "Wysokie",
                    Description = "BMI powyżej 30 znacznie zwiększa ryzyko chorób przewlekłych",
                    Impact = "Cukrzyca, choroby serca, nowotwory, bezdech senny",
                    PreventionTips = new List<string> { "Deficyt kaloryczny", "Zwiększ aktywność", "Konsultacja dietetyka" }
                });
            }
            else if (bmi > 25)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Nadwaga",
                    Level = "Umiarkowane",
                    Description = "BMI 25-30 zwiększa ryzyko rozwoju chorób metabolicznych",
                    Impact = "Zwiększone ciśnienie, insulinooporność, problemy stawowe",
                    PreventionTips = new List<string> { "Kontrola porcji", "Regularna aktywność", "Monitorowanie wagi" }
                });
            }
        }

        private void AnalyzeNutritionalRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            var fcvc = Convert.ToDouble(request.DataframeSplit.Data[0][4]);
            var ch2o = Convert.ToDouble(request.DataframeSplit.Data[0][6]);
            var favc = request.DataframeSplit.Data[0][11].ToString();

            if (fcvc < 2)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Niedobór warzyw w diecie",
                    Level = "Średnie",
                    Description = "Niedostateczne spożycie warzyw prowadzi do niedoborów witamin",
                    Impact = "Niedobory witamin, minerałów, błonnika, zwiększone ryzyko nowotworów",
                    PreventionTips = new List<string> { "5 porcji warzyw dziennie", "Różnorodność kolorów", "Warzywa w każdym posiłku" }
                });
            }

            if (ch2o < 2)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Niewystarczające nawodnienie",
                    Level = "Niskie",
                    Description = "Zbyt małe spożycie wody wpływa na funkcjonowanie organizmu",
                    Impact = "Problemy z nerkami, zmęczenie, problemy skórne, zaparcia",
                    PreventionTips = new List<string> { "8 szklanek wody dziennie", "Woda przed posiłkami", "Monitoruj kolor moczu" }
                });
            }

            if (favc == "yes")
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Wysokokaloryczna dieta",
                    Level = "Średnie",
                    Description = "Częste spożycie wysokokalorycznej żywności",
                    Impact = "Przyrost masy ciała, cukrzyca, choroby serca",
                    PreventionTips = new List<string> { "Zamień na zdrowe alternatywy", "Kontroluj porcje", "Czytaj etykiety" }
                });
            }
        }

        private void AnalyzeActivityRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);
            var tue = Convert.ToDouble(request.DataframeSplit.Data[0][8]);

            if (faf < 2)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Siedzący tryb życia",
                    Level = "Wysokie",
                    Description = "Brak regularnej aktywności fizycznej",
                    Impact = "Choroby serca, cukrzyca, osteoporoza, depresja, skrócenie życia",
                    PreventionTips = new List<string> { "150 min aktywności tygodniowo", "Codzienne spacery", "Ćwiczenia siłowe 2x/tydzień" }
                });
            }

            if (tue > 8)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Nadmierne użycie technologii",
                    Level = "Średnie",
                    Description = "Ponad 8 godzin dziennie przy ekranach",
                    Impact = "Problemy wzroku, ból pleców/szyi, zaburzenia snu, izolacja społeczna",
                    PreventionTips = new List<string> { "Przerwy co godzinę", "Ergonomiczne stanowisko", "Digital detox" }
                });
            }
        }

        private void AnalyzeLifestyleRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var calc = request.DataframeSplit.Data[0][15].ToString();

            if (smoke == "yes")
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Palenie tytoniu",
                    Level = "Bardzo wysokie",
                    Description = "Palenie jest główną przyczyną chorób przewlekłych",
                    Impact = "Nowotwory, choroby serca, udar, POCHP, przedwczesna śmierć",
                    PreventionTips = new List<string> { "Natychmiastowe rzucenie", "Terapia nikotynowa", "Wsparcie psychologiczne" }
                });
            }

            if (calc == "Frequently" || calc == "Always")
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Nadmierne spożycie alkoholu",
                    Level = "Wysokie",
                    Description = "Częste lub stałe spożycie alkoholu",
                    Impact = "Choroby wątroby, nowotwory, uzależnienie, problemy społeczne",
                    PreventionTips = new List<string> { "Ogranicz do 2 jednostek/tydzień", "Dni bez alkoholu", "Szukaj pomocy" }
                });
            }
        }

        private void AnalyzeGeneticRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            var familyHistory = request.DataframeSplit.Data[0][10].ToString();
            var age = Convert.ToInt32(request.DataframeSplit.Data[0][1]);

            if (familyHistory == "yes")
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Obciążenie genetyczne",
                    Level = "Średnie",
                    Description = "Historia nadwagi/otyłości w rodzinie",
                    Impact = "Zwiększone predyspozycje do problemów z wagą i chorób metabolicznych",
                    PreventionTips = new List<string> { "Regularne badania", "Profilaktyka dietetyczna", "Aktywny styl życia" }
                });
            }

            if (age > 50)
            {
                riskFactors.Add(new RiskFactor
                {
                    Name = "Wiek powyżej 50 lat",
                    Level = "Niskie",
                    Description = "Naturalny proces starzenia zwiększa niektóre ryzyka",
                    Impact = "Spowolnienie metabolizmu, utrata masy mięśniowej, problemy hormonalne",
                    PreventionTips = new List<string> { "Regularne badania", "Trening siłowy", "Suplementacja" }
                });
            }
        }

        private void AnalyzeProtectiveFactors(HealthRequest request, List<string> protectiveFactors)
        {
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);
            var smoke = request.DataframeSplit.Data[0][13].ToString();
            var fcvc = Convert.ToDouble(request.DataframeSplit.Data[0][4]);
            var scc = request.DataframeSplit.Data[0][14].ToString();
            var calc = request.DataframeSplit.Data[0][15].ToString();

            if (faf >= 3)
                protectiveFactors.Add("Regularna aktywność fizyczna - silna ochrona przed chorobami przewlekłymi");

            if (smoke == "no")
                protectiveFactors.Add("Niepalenie - znacząco zmniejsza ryzyko nowotworów i chorób serca");

            if (fcvc >= 3)
                protectiveFactors.Add("Wysokie spożycie warzyw - bogate źródło antyoksydantów i błonnika");

            if (scc == "yes")
                protectiveFactors.Add("Świadome monitorowanie zdrowia - wczesne wykrywanie problemów");

            if (calc == "no")
                protectiveFactors.Add("Abstynencja alkoholowa - ochrona wątroby i układu nerwowego");
        }

        private string CalculateOverallRisk(List<RiskFactor> riskFactors)
        {
            if (!riskFactors.Any()) return "Bardzo niskie";

            var highRiskCount = riskFactors.Count(r => r.Level == "Bardzo wysokie" || r.Level == "Wysokie");
            var mediumRiskCount = riskFactors.Count(r => r.Level == "Średnie");

            if (highRiskCount >= 2) return "Bardzo wysokie";
            if (highRiskCount >= 1) return "Wysokie";
            if (mediumRiskCount >= 3) return "Wysokie";
            if (mediumRiskCount >= 1) return "Umiarkowane";
            return "Niskie";
        }

        private int CalculateRiskScore(List<RiskFactor> riskFactors)
        {
            var score = 0;
            foreach (var risk in riskFactors)
            {
                score += risk.Level switch
                {
                    "Bardzo wysokie" => 25,
                    "Wysokie" => 20,
                    "Średnie" => 15,
                    "Niskie" => 10,
                    _ => 5
                };
            }
            return Math.Min(100, score);
        }

        private List<string> GeneratePreventionStrategies(List<RiskFactor> riskFactors)
        {
            var strategies = new List<string>();

            if (riskFactors.Any(r => r.Name.Contains("Otyłość") || r.Name.Contains("Nadwaga")))
            {
                strategies.Add("Program redukcji masy ciała z deficytem 500-750 kcal dziennie");
                strategies.Add("Zwiększenie aktywności fizycznej do 300 minut tygodniowo");
            }

            if (riskFactors.Any(r => r.Name.Contains("Palenie")))
            {
                strategies.Add("Natychmiastowy program rzucania palenia z wsparciem medycznym");
            }

            if (riskFactors.Any(r => r.Name.Contains("Siedzący")))
            {
                strategies.Add("Wprowadzenie codziennej aktywności - minimum 30 minut spaceru");
            }

            if (riskFactors.Any(r => r.Name.Contains("dieta") || r.Name.Contains("żywienie")))
            {
                strategies.Add("Konsultacja z dietetykiem i opracowanie spersonalizowanego planu");
            }

            strategies.Add("Regularne badania kontrolne co 6-12 miesięcy");
            strategies.Add("Edukacja zdrowotna i budowanie świadomości");

            return strategies;
        }

        private List<string> GenerateMonitoringRecommendations(List<RiskFactor> riskFactors)
        {
            var monitoring = new List<string>();

            monitoring.Add("Cotygodniowe ważenie się (w tym samym dniu i czasie)");
            monitoring.Add("Miesięczne pomiary obwodów (talia, biodra, ramiona)");

            if (riskFactors.Any(r => r.Level == "Wysokie" || r.Level == "Bardzo wysokie"))
            {
                monitoring.Add("Badania krwi co 3 miesiące (glukoza, lipidogram, CRP)");
                monitoring.Add("Pomiar ciśnienia krwi 2x w tygodniu");
            }
            else
            {
                monitoring.Add("Badania krwi co 6-12 miesięcy");
                monitoring.Add("Pomiar ciśnienia krwi raz w miesiącu");
            }

            monitoring.Add("Dziennik żywieniowy przez pierwsze 4 tygodnie");
            monitoring.Add("Aplikacja do śledzenia aktywności fizycznej");

            return monitoring;
        }

        // Recommendation Generation Methods
        private List<Recommendation> GenerateDietaryRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            var height = Convert.ToDouble(request.DataframeSplit.Data[0][2]);
            var weight = Convert.ToDouble(request.DataframeSplit.Data[0][3]);
            var bmi = weight / (height * height);

            if (bmi > 25)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Dieta",
                    Description = "Wprowadź deficyt kaloryczny 500-750 kcal dziennie dla bezpiecznej redukcji wagi",
                    Priority = 10,
                    Timeframe = "Natychmiast - długoterminowo",
                    ExpectedBenefit = "Redukcja 0.5-1 kg tygodniowo, poprawa parametrów metabolicznych",
                    ActionSteps = new List<string>
                    {
                        "Oblicz dzienne zapotrzebowanie kaloryczne",
                        "Zaplanuj 5-6 mniejszych posiłków dziennie",
                        "Zwiększ spożycie białka do 1.2-1.6g/kg masy ciała",
                        "Ogranicz węglowodany proste i tłuszcze nasycone"
                    }
                });
            }

            recommendations.Add(new Recommendation
            {
                Category = "Nawyki żywieniowe",
                Description = "Wprowadź zasadę talerza: 1/2 warzywa, 1/4 białko, 1/4 węglowodany złożone",
                Priority = 8,
                Timeframe = "1-2 tygodnie na wdrożenie",
                ExpectedBenefit = "Lepsze nasycenie, kontrola glikemii, większa różnorodność składników",
                ActionSteps = new List<string>
                {
                    "Kup większe talerze dla warzyw",
                    "Przygotuj warzywa z wyprzedzeniem",
                    "Wybieraj białko chude (ryby, drób, rośliny strączkowe)",
                    "Zastąp białe pieczywo pełnoziarnistym"
                }
            });

            return recommendations;
        }

        private List<Recommendation> GenerateActivityRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);

            if (faf < 2)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Aktywność fizyczna",
                    Description = "Rozpocznij program aktywności od 150 minut umiarkowanej aktywności tygodniowo",
                    Priority = 9,
                    Timeframe = "Stopniowe wprowadzanie przez 4-6 tygodni",
                    ExpectedBenefit = "Poprawa kondycji, redukcja ryzyka chorób, lepsze samopoczucie",
                    ActionSteps = new List<string>
                    {
                        "Tydzień 1-2: 15 minut spaceru dziennie",
                        "Tydzień 3-4: 25 minut aktywności dziennie",
                        "Tydzień 5-6: 30 minut aktywności + 2x trening siłowy",
                        "Znajdź aktywność, która sprawia Ci przyjemność"
                    }
                });
            }

            return recommendations;
        }

        private List<Recommendation> GenerateMedicalRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            var age = Convert.ToInt32(request.DataframeSplit.Data[0][1]);
            var bmi = Convert.ToDouble(request.DataframeSplit.Data[0][3]) / Math.Pow(Convert.ToDouble(request.DataframeSplit.Data[0][2]), 2);

            if (bmi > 30 || age > 50)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Badania medyczne",
                    Description = "Wykonaj kompleksowe badania krwi i konsultację lekarską",
                    Priority = 7,
                    Timeframe = "W ciągu 2-4 tygodni",
                    ExpectedBenefit = "Wczesne wykrycie problemów, spersonalizowane zalecenia",
                    ActionSteps = new List<string>
                    {
                        "Umów wizytę u lekarza rodzinnego",
                        "Wykonaj morfologię, lipidogram, glukozę",
                        "Sprawdź ciśnienie krwi i BMI",
                        "Omów wyniki i plan działania z lekarzem"
                    }
                });
            }

            return recommendations;
        }

        private List<Recommendation> GeneratePsychologicalRecommendations(HealthRequest request)
        {
            var recommendations = new List<Recommendation>();

            recommendations.Add(new Recommendation
            {
                Category = "Zdrowie psychiczne",
                Description = "Wprowadź codzienne praktyki mindfulness i zarządzania stresem",
                Priority = 6,
                Timeframe = "Codziennie przez 21 dni na wyrobienie nawyku",
                ExpectedBenefit = "Redukcja stresu, lepsza kontrola emocji, poprawa jakości snu",
                ActionSteps = new List<string>
                {
                    "5-10 minut medytacji rano",
                    "Prowadź dziennik wdzięczności",
                    "Praktykuj głębokie oddychanie w stresie",
                    "Zaplanuj czas na hobby i relaks"
                }
            });

            return recommendations;
        }

        // Action Plan Generation Methods
        private List<string> GenerateImmediateActions(HealthRequest request, string prediction)
        {
            var actions = new List<string>();
            var bmi = Convert.ToDouble(request.DataframeSplit.Data[0][3]) / Math.Pow(Convert.ToDouble(request.DataframeSplit.Data[0][2]), 2);
            var smoke = request.DataframeSplit.Data[0][13].ToString();

            actions.Add("Zainstaluj aplikację do śledzenia kalorii i aktywności");
            actions.Add("Kup wagę i rozpocznij codzienne ważenie się");
            actions.Add("Zaplanuj posiłki na najbliższe 3 dni");

            if (smoke == "yes")
                actions.Add("PILNE: Skontaktuj się z lekarzem w sprawie rzucenia palenia");

            if (bmi > 30)
                actions.Add("Umów wizytę u lekarza i dietetyka w ciągu 2 tygodni");

            actions.Add("Rozpocznij od 15-minutowego spaceru dzisiaj");

            return actions;
        }

        private List<string> GenerateShortTermGoals(HealthRequest request, string prediction)
        {
            var goals = new List<string>();
            var bmi = Convert.ToDouble(request.DataframeSplit.Data[0][3]) / Math.Pow(Convert.ToDouble(request.DataframeSplit.Data[0][2]), 2);

            if (bmi > 25)
                goals.Add("Redukcja wagi o 2-4 kg w ciągu pierwszych 2 miesięcy");

            goals.Add("Osiągnięcie 150 minut aktywności fizycznej tygodniowo");
            goals.Add("Wprowadzenie 5 porcji warzyw i owoców dziennie");
            goals.Add("Redukcja czasu ekranowego o 25%");
            goals.Add("Wykonanie kompleksowych badań krwi");

            return goals;
        }

        private List<string> GenerateLongTermGoals(HealthRequest request, string prediction)
        {
            var goals = new List<string>();
            var bmi = Convert.ToDouble(request.DataframeSplit.Data[0][3]) / Math.Pow(Convert.ToDouble(request.DataframeSplit.Data[0][2]), 2);

            if (bmi > 25)
                goals.Add("Osiągnięcie i utrzymanie zdrowej wagi (BMI 18.5-24.9)");

            goals.Add("Budowa nawyku regularnej aktywności fizycznej (300+ min/tydzień)");
            goals.Add("Osiągnięcie optymalnych parametrów krwi (glukoza, cholesterol)");
            goals.Add("Redukcja ryzyka chorób przewlekłych o 50%");
            goals.Add("Poprawa jakości życia i energii o 40%");

            return goals;
        }

        private Dictionary<string, List<string>> GenerateWeeklySchedule(HealthRequest request)
        {
            return new Dictionary<string, List<string>>
            {
                ["Poniedziałek"] = new List<string> { "30 min spacer", "Planowanie posiłków", "Zakupy zdrowej żywności" },
                ["Wtorek"] = new List<string> { "Trening siłowy 20 min", "Przygotowanie zdrowych przekąsek" },
                ["Środa"] = new List<string> { "45 min aktywność cardio", "Medytacja 10 min" },
                ["Czwartek"] = new List<string> { "Joga/stretching 30 min", "Przygotowanie posiłków na piątek" },
                ["Piątek"] = new List<string> { "Trening całego ciała 40 min", "Relaks i hobby" },
                ["Sobota"] = new List<string> { "Długa aktywność na świeżym powietrzu", "Przygotowanie posiłków na tydzień" },
                ["Niedziela"] = new List<string> { "Aktywny odpoczynek", "Planowanie następnego tygodnia", "Pomiary i ocena postępów" }
            };
        }

        private List<string> GenerateMonthlyMilestones(HealthRequest request, string prediction)
        {
            return new List<string>
            {
                "Miesiąc 1: Wdrożenie podstawowych nawyków żywieniowych i aktywności",
                "Miesiąc 2: Osiągnięcie regularności w ćwiczeniach i diecie",
                "Miesiąc 3: Pierwsze widoczne rezultaty i kontrolne badania",
                "Miesiąc 6: Znacząca poprawa parametrów zdrowotnych",
                "Miesiąc 12: Osiągnięcie docelowej wagi i optymalnego zdrowia"
            };
        }

        private List<string> GenerateProgressTracking(HealthRequest request)
        {
            return new List<string>
            {
                "Codzienny pomiar wagi (rano, na czczo)",
                "Tygodniowe zdjęcia postępów",
                "Miesięczne pomiary obwodów ciała",
                "Kwartalne badania krwi",
                "Cotygodniowa ocena samopoczucia (1-10)",
                "Miesięczna analiza dziennika żywieniowego",
                "Półroczna konsultacja z lekarzem/dietetykiem"
            };
        }

        // Final Helper Methods
        private string GetHealthCategory(int healthScore)
        {
            return healthScore switch
            {
                >= 90 => "Doskonałe zdrowie",
                >= 80 => "Bardzo dobre zdrowie",
                >= 70 => "Dobre zdrowie",
                >= 60 => "Przeciętne zdrowie",
                >= 50 => "Poniżej przeciętnej",
                _ => "Wymaga pilnej interwencji"
            };
        }

        private string GetHealthPrognosis(HealthRequest request, string prediction)
        {
            var age = Convert.ToInt32(request.DataframeSplit.Data[0][1]);
            var bmi = Convert.ToDouble(request.DataframeSplit.Data[0][3]) / Math.Pow(Convert.ToDouble(request.DataframeSplit.Data[0][2]), 2);
            var faf = Convert.ToDouble(request.DataframeSplit.Data[0][7]);
            var smoke = request.DataframeSplit.Data[0][13].ToString();

            var prognosisFactors = 0;
            if (bmi >= 18.5 && bmi <= 24.9) prognosisFactors++;
            if (faf >= 3) prognosisFactors++;
            if (smoke == "no") prognosisFactors++;
            if (age < 50) prognosisFactors++;

            return prognosisFactors switch
            {
                4 => "Doskonała - przy utrzymaniu obecnych nawyków możesz cieszyć się długim i zdrowym życiem",
                3 => "Bardzo dobra - niewielkie zmiany mogą znacząco poprawić Twoje zdrowie",
                2 => "Dobra - wprowadzenie zdrowych nawyków przyniesie wymierne korzyści",
                1 => "Umiarkowana - wymagane znaczące zmiany stylu życia",
                _ => "Wymagana natychmiastowa interwencja - wysokie ryzyko powikłań zdrowotnych"
            };
        }
    }
} 