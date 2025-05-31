using MyHealth.Api.Models;
using System.Text;

namespace MyHealth.Api.Services
{
    public partial class AdvancedHealthAnalyzer
    {
        public AdvancedHealthAnalysisResult AnalyzeHealth(HealthRequest request, List<AssessmentItem> assessments)
        {
            var prediction = assessments.FirstOrDefault()?.Prediction ?? "Unknown";
            
            var result = new AdvancedHealthAnalysisResult
            {
                Prediction = prediction,
                PersonalProfile = BuildPersonalProfile(request),
                BmiAnalysis = AnalyzeBMI(request),
                NutritionalAnalysis = AnalyzeNutrition(request),
                PhysicalActivityAnalysis = AnalyzePhysicalActivity(request),
                LifestyleAnalysis = AnalyzeLifestyle(request),
                RiskFactorAnalysis = AnalyzeRiskFactors(request),
                Recommendations = GenerateRecommendations(request, prediction),
                ActionPlan = GenerateActionPlan(request, prediction),
                HealthScore = CalculateHealthScore(request),
                DetailedDescription = GenerateDetailedDescription(request, prediction)
            };

            return result;
        }

        private PersonalProfile BuildPersonalProfile(HealthRequest request)
        {
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1].ToString()); // Age
            var gender = request.DataFrame_Split.Data[0][9].ToString(); // Gender
            var height = Convert.ToDouble(request.DataFrame_Split.Data[0][2].ToString()); // Height
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3].ToString()); // Weight

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

        private BMIAnalysis AnalyzeBMI(HealthRequest request)
        {
            var height = Convert.ToDouble(request.DataFrame_Split.Data[0][2].ToString());
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3].ToString());
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1].ToString());
            var gender = request.DataFrame_Split.Data[0][9].ToString();

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

        private NutritionalAnalysis AnalyzeNutrition(HealthRequest request)
        {
            var fcvc = Convert.ToDouble(request.DataFrame_Split.Data[0][4].ToString()); // Vegetable consumption
            var ncp = Convert.ToDouble(request.DataFrame_Split.Data[0][5].ToString()); // Number of meals
            var ch2o = Convert.ToDouble(request.DataFrame_Split.Data[0][6].ToString()); // Water consumption
            var favc = request.DataFrame_Split.Data[0][11].ToString(); // High caloric food
            var caec = request.DataFrame_Split.Data[0][12].ToString(); // Eating between meals
            var calc = request.DataFrame_Split.Data[0][15].ToString(); // Alcohol consumption

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

        private PhysicalActivityAnalysis AnalyzePhysicalActivity(HealthRequest request)
        {
            var faf = Convert.ToDouble(request.DataFrame_Split.Data[0][7].ToString()); // Physical activity frequency
            var tue = Convert.ToDouble(request.DataFrame_Split.Data[0][8].ToString()); // Technology use
            var mtrans = request.DataFrame_Split.Data[0][16].ToString(); // Transportation

            var activityLevel = GetActivityLevel(faf);
            var sedentaryRisk = CalculateSedentaryRisk(tue, mtrans);

            return new PhysicalActivityAnalysis
            {
                ActivityFrequency = faf,
                ActivityLevel = activityLevel,
                TechnologyTime = tue,
                TransportationType = mtrans,
                SedentaryRisk = sedentaryRisk,
                CaloriesBurnedWeekly = EstimateCaloriesBurned(faf, request),
                FitnessRecommendations = GenerateFitnessRecommendations(faf, tue, mtrans, request),
                ExercisePlan = GenerateExercisePlan(faf, request)
            };
        }

        private LifestyleAnalysis AnalyzeLifestyle(HealthRequest request)
        {
            var smoke = request.DataFrame_Split.Data[0][13].ToString();
            var scc = request.DataFrame_Split.Data[0][14].ToString(); // Calorie monitoring
            var familyHistory = request.DataFrame_Split.Data[0][10].ToString();
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1].ToString());

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
            var score = 100; // Zaczynamy od 100 punktów

            // Odejmujemy punkty za czynniki ryzyka
            var height = Convert.ToDouble(request.DataFrame_Split.Data[0][2].ToString());
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3].ToString());
            var bmi = weight / (height * height);

            // BMI
            if (bmi < 18.5 || bmi > 25) score -= 15;
            else if (bmi > 30) score -= 30;

            // Aktywność fizyczna
            var faf = Convert.ToDouble(request.DataFrame_Split.Data[0][7].ToString());
            if (faf < 1) score -= 20;
            else if (faf < 2) score -= 10;

            // Palenie
            var smoke = request.DataFrame_Split.Data[0][13].ToString();
            if (smoke == "yes") score -= 25;

            // Alkohol
            var calc = request.DataFrame_Split.Data[0][15].ToString();
            if (calc == "Frequently") score -= 15;
            else if (calc == "Always") score -= 25;

            // Nawyki żywieniowe
            var favc = request.DataFrame_Split.Data[0][11].ToString();
            if (favc == "yes") score -= 10;

            var fcvc = Convert.ToDouble(request.DataFrame_Split.Data[0][4].ToString());
            if (fcvc < 2) score -= 10;

            var ch2o = Convert.ToDouble(request.DataFrame_Split.Data[0][6].ToString());
            if (ch2o < 2) score -= 10;

            return Math.Max(0, Math.Min(100, score));
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
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1].ToString());
            var faf = Convert.ToDouble(request.DataFrame_Split.Data[0][7].ToString());
            var smoke = request.DataFrame_Split.Data[0][13].ToString();
            var height = Convert.ToDouble(request.DataFrame_Split.Data[0][2].ToString());
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3].ToString());
            var bmi = weight / (height * height);

            var metabolicAge = age;

            // Czynniki zwiększające wiek metaboliczny
            if (bmi > 25) metabolicAge += (int)((bmi - 25) * 2);
            if (faf < 2) metabolicAge += 5;
            if (smoke == "yes") metabolicAge += 10;

            // Czynniki zmniejszające wiek metaboliczny
            if (faf > 3) metabolicAge -= 3;
            if (bmi >= 18.5 && bmi <= 24.9) metabolicAge -= 2;

            return Math.Max(age - 10, Math.Min(age + 20, metabolicAge));
        }

        // Będę kontynuować implementację pozostałych metod...
    }
} 