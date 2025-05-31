using MyHealth.Api.Models;
using System.Text;

namespace MyHealth.Api.Services
{
    public partial class AdvancedHealthAnalyzer
    {
        public AdvancedHealthAnalysisResult AnalyzeHealth(HealthRequest request, List<AssessmentItem> assessments)
        {
            try
            {
                // Sprawdzenie podstawowych danych
                if (request == null)
                    throw new ArgumentNullException(nameof(request), "Request cannot be null");
                
                if (request.DataFrame_Split == null)
                    throw new ArgumentNullException(nameof(request.DataFrame_Split), "DataFrame_Split cannot be null");
                
                if (request.DataFrame_Split.Data == null || request.DataFrame_Split.Data.Length == 0)
                    throw new ArgumentException("DataFrame_Split.Data cannot be null or empty");
                
                if (request.DataFrame_Split.Data[0] == null || request.DataFrame_Split.Data[0].Length < 17)
                    throw new ArgumentException($"DataFrame_Split.Data[0] must have at least 17 elements, but has {request.DataFrame_Split.Data[0]?.Length ?? 0}");

                var prediction = assessments?.FirstOrDefault()?.Prediction ?? "Unknown";
                
                var result = new AdvancedHealthAnalysisResult
                {
                    Prediction = prediction
                };

                try { result.PersonalProfile = BuildPersonalProfile(request); }
                catch (Exception ex) { throw new Exception($"Error in BuildPersonalProfile: {ex.Message}", ex); }

                try { result.BmiAnalysis = AnalyzeBMI(request); }
                catch (Exception ex) { throw new Exception($"Error in AnalyzeBMI: {ex.Message}", ex); }

                try { result.NutritionalAnalysis = AnalyzeNutrition(request); }
                catch (Exception ex) { throw new Exception($"Error in AnalyzeNutrition: {ex.Message}", ex); }

                try { result.PhysicalActivityAnalysis = AnalyzePhysicalActivity(request); }
                catch (Exception ex) { throw new Exception($"Error in AnalyzePhysicalActivity: {ex.Message}", ex); }

                try { result.LifestyleAnalysis = AnalyzeLifestyle(request); }
                catch (Exception ex) { throw new Exception($"Error in AnalyzeLifestyle: {ex.Message}", ex); }

                try { result.RiskFactorAnalysis = AnalyzeRiskFactors(request); }
                catch (Exception ex) { throw new Exception($"Error in AnalyzeRiskFactors: {ex.Message}", ex); }

                try { result.Recommendations = GenerateRecommendations(request, prediction); }
                catch (Exception ex) { throw new Exception($"Error in GenerateRecommendations: {ex.Message}", ex); }

                try { result.ActionPlan = GenerateActionPlan(request, prediction); }
                catch (Exception ex) { throw new Exception($"Error in GenerateActionPlan: {ex.Message}", ex); }

                try { result.HealthScore = CalculateHealthScore(request); }
                catch (Exception ex) { throw new Exception($"Error in CalculateHealthScore: {ex.Message}", ex); }

                try { result.DetailedDescription = GenerateDetailedDescription(request, prediction); }
                catch (Exception ex) { throw new Exception($"Error in GenerateDetailedDescription: {ex.Message}", ex); }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AdvancedHealthAnalyzer.AnalyzeHealth: {ex.Message}", ex);
            }
        }

        private PersonalProfile BuildPersonalProfile(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                
                // Sprawdzenie czy wszystkie potrzebne indeksy istnieją
                if (data.Length < 17)
                    throw new ArgumentException($"Data array must have at least 17 elements, but has {data.Length}");

                var age = Convert.ToInt32(data[1]?.ToString() ?? "0"); // Age
                var gender = data[9]?.ToString() ?? "Unknown"; // Gender
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7"); // Height
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70"); // Weight

                var bmi = weight / (height * height);
                var ageGroup = GetAgeGroup(age);
                var bmiCategory = GetBMICategory(bmi);

                return new PersonalProfile
                {
                    Age = age,
                    Gender = gender,
                    Height = height,
                    Weight = weight,
                    BMI = Math.Round(bmi, 1),
                    AgeGroup = ageGroup,
                    BMICategory = bmiCategory,
                    IdealWeightRange = CalculateIdealWeightRange(height),
                    MetabolicAge = CalculateMetabolicAge(request)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in BuildPersonalProfile: {ex.Message}. Data: {string.Join(",", request.DataFrame_Split.Data[0] ?? new object[0])}", ex);
            }
        }

        private BMIAnalysis AnalyzeBMI(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");
                var gender = data[9]?.ToString() ?? "Male";

                var bmi = weight / (height * height);
                var category = GetBMICategory(bmi);
                var idealWeight = CalculateIdealWeight(height, gender);
                var weightDifference = weight - idealWeight;

                var analysis = new BMIAnalysis
                {
                    CurrentBMI = Math.Round(bmi, 1),
                    Category = category,
                    IdealBMI = 22.0, // Środek zdrowego zakresu
                    IdealWeight = Math.Round(idealWeight, 1),
                    WeightDifference = Math.Round(weightDifference, 1),
                    HealthRisk = GetBMIHealthRisk(bmi, age),
                    DetailedExplanation = GenerateBMIExplanation(bmi, category, age, gender),
                    Recommendations = GenerateBMIRecommendations(bmi, weightDifference, age, gender)
                };

                return analysis;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AnalyzeBMI: {ex.Message}", ex);
            }
        }

        private NutritionalAnalysis AnalyzeNutrition(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                
                var fcvc = Convert.ToDouble(data[4]?.ToString() ?? "2"); // Vegetable consumption
                var ncp = Convert.ToDouble(data[5]?.ToString() ?? "3"); // Number of meals
                var ch2o = Convert.ToDouble(data[6]?.ToString() ?? "2"); // Water consumption
                var favc = data[11]?.ToString() ?? "no"; // High caloric food
                var caec = data[12]?.ToString() ?? "Sometimes"; // Eating between meals
                var calc = data[15]?.ToString() ?? "no"; // Alcohol consumption

                var nutritionScore = CalculateNutritionScore(fcvc, ncp, ch2o, favc, caec, calc);

                return new NutritionalAnalysis
                {
                    VegetableConsumption = AnalyzeVegetableConsumption(fcvc),
                    MealFrequency = AnalyzeMealFrequency(ncp),
                    HydrationLevel = AnalyzeHydration(ch2o),
                    CalorieIntake = AnalyzeCalorieIntake(favc, caec),
                    AlcoholConsumption = AnalyzeAlcoholConsumption(calc),
                    NutritionScore = nutritionScore,
                    DetailedNutritionPlan = GenerateNutritionPlan(fcvc, ncp, ch2o, favc, caec, calc),
                    SupplementRecommendations = GenerateSupplementRecommendations(request)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AnalyzeNutrition: {ex.Message}", ex);
            }
        }

        private PhysicalActivityAnalysis AnalyzePhysicalActivity(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1"); // Physical activity frequency
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2"); // Technology use
                var mtrans = data[16]?.ToString() ?? "Public_Transportation"; // Transportation

                var activityLevel = GetActivityLevel(faf);
                var sedentaryRisk = CalculateSedentaryRisk(tue, mtrans);

                return new PhysicalActivityAnalysis
                {
                    ActivityFrequency = faf,
                    ActivityLevel = activityLevel,
                    TechnologyTime = tue,
                    TransportationType = mtrans,
                    SedentaryRisk = sedentaryRisk,
                    CaloriesBurnedWeekly = (int)EstimateCaloriesBurned(faf, request),
                    FitnessRecommendations = GenerateFitnessRecommendations(faf, tue, mtrans, request),
                    ExercisePlan = GenerateExercisePlan(faf, request)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AnalyzePhysicalActivity: {ex.Message}", ex);
            }
        }

        private LifestyleAnalysis AnalyzeLifestyle(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                
                var smoke = data[13]?.ToString() ?? "no";
                var scc = data[14]?.ToString() ?? "no"; // Calorie monitoring
                var familyHistory = data[10]?.ToString() ?? "no";
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");

                return new LifestyleAnalysis
                {
                    SmokingStatus = AnalyzeSmokingStatus(smoke),
                    HealthMonitoring = AnalyzeHealthMonitoring(scc),
                    FamilyHistory = AnalyzeFamilyHistory(familyHistory),
                    LifestyleScore = CalculateLifestyleScore(request),
                    StressLevel = EstimateStressLevel(request),
                    SleepQuality = EstimateSleepQuality(request),
                    LifestyleRecommendations = GenerateLifestyleRecommendations(request)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in AnalyzeLifestyle: {ex.Message}", ex);
            }
        }

        private RiskFactorAnalysis AnalyzeRiskFactors(HealthRequest request)
        {
            var riskFactors = new List<RiskFactor>();
            var protectiveFactors = new List<string>();

            // Analiza czynników ryzyka
            AnalyzeWeightRisk(request, riskFactors);
            AnalyzeNutritionalRisks(request, riskFactors);
            AnalyzeActivityRisks(request, riskFactors);
            AnalyzeLifestyleRisks(request, riskFactors);
            AnalyzeGeneticRisks(request, riskFactors);

            // Analiza czynników ochronnych
            AnalyzeProtectiveFactors(request, protectiveFactors);

            var overallRisk = CalculateOverallRisk(riskFactors);

            return new RiskFactorAnalysis
            {
                RiskFactors = riskFactors,
                ProtectiveFactors = protectiveFactors,
                OverallRiskLevel = overallRisk,
                RiskScore = CalculateRiskScore(riskFactors),
                PreventionStrategies = GeneratePreventionStrategies(riskFactors),
                MonitoringRecommendations = GenerateMonitoringRecommendations(riskFactors)
            };
        }

        private List<Recommendation> GenerateRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();

            // Rekomendacje dietetyczne
            recommendations.AddRange(GenerateDietaryRecommendations(request, prediction));

            // Rekomendacje aktywności fizycznej
            recommendations.AddRange(GenerateActivityRecommendations(request, prediction));

            // Rekomendacje medyczne
            recommendations.AddRange(GenerateMedicalRecommendations(request, prediction));

            // Rekomendacje psychologiczne
            recommendations.AddRange(GeneratePsychologicalRecommendations(request));

            return recommendations.OrderByDescending(r => r.Priority).ToList();
        }

        private ActionPlan GenerateActionPlan(HealthRequest request, string prediction)
        {
            return new ActionPlan
            {
                ImmediateActions = GenerateImmediateActions(request, prediction),
                ShortTermGoals = GenerateShortTermGoals(request, prediction),
                LongTermGoals = GenerateLongTermGoals(request, prediction),
                WeeklySchedule = GenerateWeeklySchedule(request),
                MonthlyMilestones = GenerateMonthlyMilestones(request, prediction),
                ProgressTracking = GenerateProgressTracking(request)
            };
        }

        private int CalculateHealthScore(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var score = 100; // Zaczynamy od 100 punktów

                // Odejmujemy punkty za czynniki ryzyka
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                // BMI
                if (bmi < 18.5 || bmi > 25) score -= 15;
                else if (bmi > 30) score -= 30;

                // Aktywność fizyczna
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                if (faf < 1) score -= 20;
                else if (faf < 2) score -= 10;

                // Palenie
                var smoke = data[13]?.ToString() ?? "no";
                if (smoke.ToLower() == "yes") score -= 25;

                // Alkohol
                var calc = data[15]?.ToString() ?? "no";
                if (calc.ToLower() == "frequently") score -= 15;
                else if (calc.ToLower() == "always") score -= 25;

                // Nawyki żywieniowe
                var favc = data[11]?.ToString() ?? "no";
                if (favc.ToLower() == "yes") score -= 10;

                var fcvc = Convert.ToDouble(data[4]?.ToString() ?? "2");
                if (fcvc < 2) score -= 10;

                var ch2o = Convert.ToDouble(data[6]?.ToString() ?? "2");
                if (ch2o < 2) score -= 10;

                return Math.Max(0, Math.Min(100, score));
            }
            catch (Exception ex)
            {
                return 50; // Wartość domyślna w przypadku błędu
            }
        }

        private string GenerateDetailedDescription(HealthRequest request, string prediction)
        {
            var sb = new StringBuilder();
            var profile = BuildPersonalProfile(request);

            sb.AppendLine($"🏥 **KOMPLEKSOWA ANALIZA ZDROWOTNA**");
            sb.AppendLine($"═══════════════════════════════════════");
            sb.AppendLine();

            // Profil osobisty
            sb.AppendLine($"👤 **PROFIL OSOBISTY**");
            sb.AppendLine($"• Wiek: {profile.Age} lat ({profile.AgeGroup})");
            sb.AppendLine($"• Płeć: {(profile.Gender == "Male" ? "Mężczyzna" : "Kobieta")}");
            sb.AppendLine($"• Wzrost: {profile.Height:F1} m");
            sb.AppendLine($"• Waga: {profile.Weight:F1} kg");
            sb.AppendLine($"• BMI: {profile.BMI:F1} ({profile.BMICategory})");
            sb.AppendLine($"• Wiek metaboliczny: ~{profile.MetabolicAge} lat");
            sb.AppendLine();

            // Analiza BMI
            var bmiAnalysis = AnalyzeBMI(request);
            sb.AppendLine($"⚖️ **ANALIZA MASY CIAŁA**");
            sb.AppendLine($"• Aktualne BMI: {bmiAnalysis.CurrentBMI}");
            sb.AppendLine($"• Kategoria: {bmiAnalysis.Category}");
            sb.AppendLine($"• Idealna waga: {bmiAnalysis.IdealWeight:F1} kg");
            sb.AppendLine($"• Różnica: {(bmiAnalysis.WeightDifference > 0 ? "+" : "")}{bmiAnalysis.WeightDifference:F1} kg");
            sb.AppendLine($"• Ryzyko zdrowotne: {bmiAnalysis.HealthRisk}");
            sb.AppendLine();
            sb.AppendLine(bmiAnalysis.DetailedExplanation);
            sb.AppendLine();

            // Analiza żywienia
            var nutritionAnalysis = AnalyzeNutrition(request);
            sb.AppendLine($"🥗 **ANALIZA ŻYWIENIA**");
            sb.AppendLine($"• Ocena żywienia: {nutritionAnalysis.NutritionScore}/100 punktów");
            sb.AppendLine($"• Spożycie warzyw: {nutritionAnalysis.VegetableConsumption}");
            sb.AppendLine($"• Częstotliwość posiłków: {nutritionAnalysis.MealFrequency}");
            sb.AppendLine($"• Poziom nawodnienia: {nutritionAnalysis.HydrationLevel}");
            sb.AppendLine($"• Spożycie kalorii: {nutritionAnalysis.CalorieIntake}");
            sb.AppendLine($"• Alkohol: {nutritionAnalysis.AlcoholConsumption}");
            sb.AppendLine();

            // Analiza aktywności
            var activityAnalysis = AnalyzePhysicalActivity(request);
            sb.AppendLine($"🏃 **ANALIZA AKTYWNOŚCI FIZYCZNEJ**");
            sb.AppendLine($"• Poziom aktywności: {activityAnalysis.ActivityLevel}");
            sb.AppendLine($"• Częstotliwość ćwiczeń: {activityAnalysis.ActivityFrequency}/5");
            sb.AppendLine($"• Czas przy technologii: {activityAnalysis.TechnologyTime} godz/dzień");
            sb.AppendLine($"• Transport: {activityAnalysis.TransportationType}");
            sb.AppendLine($"• Ryzyko siedzącego trybu życia: {activityAnalysis.SedentaryRisk}");
            sb.AppendLine($"• Spalane kalorie tygodniowo: ~{activityAnalysis.CaloriesBurnedWeekly} kcal");
            sb.AppendLine();

            // Analiza stylu życia
            var lifestyleAnalysis = AnalyzeLifestyle(request);
            sb.AppendLine($"🌟 **ANALIZA STYLU ŻYCIA**");
            sb.AppendLine($"• Status palenia: {lifestyleAnalysis.SmokingStatus}");
            sb.AppendLine($"• Monitorowanie zdrowia: {lifestyleAnalysis.HealthMonitoring}");
            sb.AppendLine($"• Historia rodzinna: {lifestyleAnalysis.FamilyHistory}");
            sb.AppendLine($"• Ocena stylu życia: {lifestyleAnalysis.LifestyleScore}/100");
            sb.AppendLine($"• Szacowany poziom stresu: {lifestyleAnalysis.StressLevel}");
            sb.AppendLine($"• Jakość snu: {lifestyleAnalysis.SleepQuality}");
            sb.AppendLine();

            // Analiza czynników ryzyka
            var riskAnalysis = AnalyzeRiskFactors(request);
            sb.AppendLine($"⚠️ **CZYNNIKI RYZYKA**");
            sb.AppendLine($"• Ogólny poziom ryzyka: {riskAnalysis.OverallRiskLevel}");
            sb.AppendLine($"• Ocena ryzyka: {riskAnalysis.RiskScore}/100");
            sb.AppendLine();

            if (riskAnalysis.RiskFactors.Any())
            {
                sb.AppendLine("**Zidentyfikowane czynniki ryzyka:**");
                foreach (var risk in riskAnalysis.RiskFactors.Take(5))
                {
                    sb.AppendLine($"• {risk.Name} (Poziom: {risk.Level}) - {risk.Description}");
                }
                sb.AppendLine();
            }

            if (riskAnalysis.ProtectiveFactors.Any())
            {
                sb.AppendLine("**Czynniki ochronne:**");
                foreach (var factor in riskAnalysis.ProtectiveFactors.Take(3))
                {
                    sb.AppendLine($"• {factor}");
                }
                sb.AppendLine();
            }

            // Rekomendacje
            var recommendations = GenerateRecommendations(request, prediction);
            sb.AppendLine($"💡 **GŁÓWNE REKOMENDACJE**");
            foreach (var rec in recommendations.Take(5))
            {
                sb.AppendLine($"• **{rec.Category}**: {rec.Description}");
            }
            sb.AppendLine();

            // Plan działania
            var actionPlan = GenerateActionPlan(request, prediction);
            sb.AppendLine($"📋 **PLAN DZIAŁANIA**");
            sb.AppendLine();
            sb.AppendLine("**Działania natychmiastowe (1-7 dni):**");
            foreach (var action in actionPlan.ImmediateActions.Take(3))
            {
                sb.AppendLine($"• {action}");
            }
            sb.AppendLine();

            sb.AppendLine("**Cele krótkoterminowe (1-3 miesiące):**");
            foreach (var goal in actionPlan.ShortTermGoals.Take(3))
            {
                sb.AppendLine($"• {goal}");
            }
            sb.AppendLine();

            sb.AppendLine("**Cele długoterminowe (6-12 miesięcy):**");
            foreach (var goal in actionPlan.LongTermGoals.Take(3))
            {
                sb.AppendLine($"• {goal}");
            }
            sb.AppendLine();

            // Ocena końcowa
            var healthScore = CalculateHealthScore(request);
            sb.AppendLine($"🎯 **OCENA KOŃCOWA**");
            sb.AppendLine($"• Ogólna ocena zdrowia: {healthScore}/100 punktów");
            sb.AppendLine($"• Kategoria: {GetHealthCategory(healthScore)}");
            sb.AppendLine($"• Prognoza: {GetHealthPrognosis(request, prediction)}");

            return sb.ToString();
        }

        // Helper methods (będę kontynuować implementację...)
        private string GetAgeGroup(int age)
        {
            return age switch
            {
                < 18 => "Nastolatek",
                < 25 => "Młody dorosły",
                < 35 => "Dorosły",
                < 50 => "Średni wiek",
                < 65 => "Dojrzały",
                _ => "Senior"
            };
        }

        private string GetBMICategory(double bmi)
        {
            return bmi switch
            {
                < 16 => "Znaczna niedowaga",
                < 18.5 => "Niedowaga",
                < 25 => "Waga prawidłowa",
                < 30 => "Nadwaga",
                < 35 => "Otyłość I stopnia",
                < 40 => "Otyłość II stopnia",
                _ => "Otyłość III stopnia"
            };
        }

        private (double min, double max) CalculateIdealWeightRange(double height)
        {
            var minBMI = 18.5;
            var maxBMI = 24.9;
            return (minBMI * height * height, maxBMI * height * height);
        }

        private int CalculateMetabolicAge(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var smoke = data[13]?.ToString() ?? "no";
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                var metabolicAge = age;

                // Czynniki zwiększające wiek metaboliczny
                if (bmi > 25) metabolicAge += (int)((bmi - 25) * 2);
                if (faf < 2) metabolicAge += 5;
                if (smoke.ToLower() == "yes") metabolicAge += 10;

                // Czynniki zmniejszające wiek metaboliczny
                if (faf > 3) metabolicAge -= 3;
                if (bmi >= 18.5 && bmi <= 24.9) metabolicAge -= 2;

                return Math.Max(age - 10, Math.Min(age + 20, metabolicAge));
            }
            catch (Exception ex)
            {
                return 25; // Wartość domyślna w przypadku błędu
            }
        }

        // Będę kontynuować implementację pozostałych metod...
    }
} 