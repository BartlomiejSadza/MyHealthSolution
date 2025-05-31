namespace MyHealth.Api.Models
{
    public class AdvancedHealthAnalysisResult
    {
        public string Prediction { get; set; } = string.Empty;
        public PersonalProfile PersonalProfile { get; set; } = new();
        public BMIAnalysis BmiAnalysis { get; set; } = new();
        public NutritionalAnalysis NutritionalAnalysis { get; set; } = new();
        public PhysicalActivityAnalysis PhysicalActivityAnalysis { get; set; } = new();
        public LifestyleAnalysis LifestyleAnalysis { get; set; } = new();
        public RiskFactorAnalysis RiskFactorAnalysis { get; set; } = new();
        public List<Recommendation> Recommendations { get; set; } = new();
        public ActionPlan ActionPlan { get; set; } = new();
        public int HealthScore { get; set; }
        public string DetailedDescription { get; set; } = string.Empty;
    }

    public class PersonalProfile
    {
        public int Age { get; set; }
        public string Gender { get; set; } = string.Empty;
        public double Height { get; set; }
        public double Weight { get; set; }
        public double BMI { get; set; }
        public string AgeGroup { get; set; } = string.Empty;
        public string BMICategory { get; set; } = string.Empty;
        public (double min, double max) IdealWeightRange { get; set; }
        public int MetabolicAge { get; set; }
    }

    public class BMIAnalysis
    {
        public double CurrentBMI { get; set; }
        public string Category { get; set; } = string.Empty;
        public double IdealBMI { get; set; }
        public double IdealWeight { get; set; }
        public double WeightDifference { get; set; }
        public string HealthRisk { get; set; } = string.Empty;
        public string DetailedExplanation { get; set; } = string.Empty;
        public List<string> Recommendations { get; set; } = new();
    }

    public class NutritionalAnalysis
    {
        public string VegetableConsumption { get; set; } = string.Empty;
        public string MealFrequency { get; set; } = string.Empty;
        public string HydrationLevel { get; set; } = string.Empty;
        public string CalorieIntake { get; set; } = string.Empty;
        public string AlcoholConsumption { get; set; } = string.Empty;
        public int NutritionScore { get; set; }
        public string DetailedNutritionPlan { get; set; } = string.Empty;
        public List<string> SupplementRecommendations { get; set; } = new();
    }

    public class PhysicalActivityAnalysis
    {
        public double ActivityFrequency { get; set; }
        public string ActivityLevel { get; set; } = string.Empty;
        public double TechnologyTime { get; set; }
        public string TransportationType { get; set; } = string.Empty;
        public string SedentaryRisk { get; set; } = string.Empty;
        public int CaloriesBurnedWeekly { get; set; }
        public List<string> FitnessRecommendations { get; set; } = new();
        public string ExercisePlan { get; set; } = string.Empty;
    }

    public class LifestyleAnalysis
    {
        public string SmokingStatus { get; set; } = string.Empty;
        public string HealthMonitoring { get; set; } = string.Empty;
        public string FamilyHistory { get; set; } = string.Empty;
        public int LifestyleScore { get; set; }
        public string StressLevel { get; set; } = string.Empty;
        public string SleepQuality { get; set; } = string.Empty;
        public List<string> LifestyleRecommendations { get; set; } = new();
    }

    public class RiskFactorAnalysis
    {
        public List<RiskFactor> RiskFactors { get; set; } = new();
        public List<string> ProtectiveFactors { get; set; } = new();
        public string OverallRiskLevel { get; set; } = string.Empty;
        public int RiskScore { get; set; }
        public List<string> PreventionStrategies { get; set; } = new();
        public List<string> MonitoringRecommendations { get; set; } = new();
    }

    public class RiskFactor
    {
        public string Name { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Impact { get; set; } = string.Empty;
        public List<string> PreventionTips { get; set; } = new();
    }

    public class Recommendation
    {
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Timeframe { get; set; } = string.Empty;
        public string ExpectedBenefit { get; set; } = string.Empty;
        public List<string> ActionSteps { get; set; } = new();
    }

    public class ActionPlan
    {
        public List<string> ImmediateActions { get; set; } = new();
        public List<string> ShortTermGoals { get; set; } = new();
        public List<string> LongTermGoals { get; set; } = new();
        public Dictionary<string, List<string>> WeeklySchedule { get; set; } = new();
        public List<string> MonthlyMilestones { get; set; } = new();
        public List<string> ProgressTracking { get; set; } = new();
    }
} 