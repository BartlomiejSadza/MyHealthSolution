import type { AssessmentItem } from "../types";

type Props = {
  items: AssessmentItem[] | null;
};

const getPredictionColor = (prediction: string) => {
  switch (prediction) {
    case "Normal_Weight":
      return "bg-green-100 border-green-300 text-green-800";
    case "Overweight_Level_I":
      return "bg-yellow-100 border-yellow-300 text-yellow-800";
    case "Overweight_Level_II":
      return "bg-orange-100 border-orange-300 text-orange-800";
    case "Obesity_Type_I":
    case "Obesity_Type_II":
    case "Obesity_Type_III":
      return "bg-red-100 border-red-300 text-red-800";
    default:
      return "bg-gray-100 border-gray-300 text-gray-800";
  }
};

const translatePrediction = (prediction: string) => {
  const translations: Record<string, string> = {
    "Normal_Weight": "Waga prawidłowa",
    "Overweight_Level_I": "Nadwaga I stopnia",
    "Overweight_Level_II": "Nadwaga II stopnia", 
    "Obesity_Type_I": "Otyłość I stopnia",
    "Obesity_Type_II": "Otyłość II stopnia",
    "Obesity_Type_III": "Otyłość III stopnia"
  };
  return translations[prediction] || prediction;
};

export default function Results({ items }: Props) {
  if (!items) return null;
  
  return (
    <div className="space-y-4">
      {items.map((item) => (
        <div 
          key={`${item.prediction}-${item.description.substring(0, 20)}`} 
          className={`border-2 rounded-lg p-6 ${getPredictionColor(item.prediction)}`}
        >
          <div className="mb-4">
            <h3 className="text-xl font-bold mb-2">
              📊 {translatePrediction(item.prediction)}
            </h3>
            <p className="text-lg leading-relaxed">
              {item.description}
            </p>
          </div>
          
          {item.observations && item.observations.length > 0 && (
            <div>
              <h4 className="font-semibold mb-3 text-lg">🔍 Szczegółowa analiza:</h4>
              <ul className="space-y-2">
                {item.observations.map((observation, obsIndex) => (
                  <li 
                    key={`${observation.substring(0, 30)}-${obsIndex}`} 
                    className="flex items-start space-x-2"
                  >
                    <span className="text-blue-600 font-bold mt-1">•</span>
                    <span className="flex-1">{observation}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      ))}
    </div>
  );
}
