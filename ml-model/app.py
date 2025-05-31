from flask import Flask, request, jsonify
import numpy as np
import pandas as pd
from model import HealthPredictor
import logging

app = Flask(__name__)
logging.basicConfig(level=logging.INFO)

# Inicjalizacja modelu
try:
    predictor = HealthPredictor()
    app.logger.info("Model załadowany pomyślnie")
except Exception as e:
    app.logger.error(f"Błąd ładowania modelu: {str(e)}")
    predictor = None

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({
        "status": "healthy", 
        "model_loaded": predictor is not None,
        "model_type": "XGBoost Classifier"
    })

@app.route('/predict', methods=['POST'])
def predict():
    try:
        if predictor is None:
            return jsonify({"error": "Model nie został załadowany"}), 500
            
        data = request.json
        app.logger.info(f"Otrzymane dane: {data}")
        
        # Mapowanie danych z frontendu na format modelu
        features = {
            "Age": float(data.get('age', 25)),
            "Gender": "Male" if data.get('gender') == 'male' else "Female",
            "Height": float(data.get('height', 170)) / 100,  # cm na metry
            "Weight": float(data.get('weight', 70)),
            "FCVC": float(data.get('vegetableConsumption', 2)),
            "NCP": float(data.get('numberOfMeals', 3)),
            "CH2O": float(data.get('waterConsumption', 2)),
            "FAF": float(data.get('physicalActivityFrequency', 1)),
            "TUE": float(data.get('technologyTime', 1)),
            "family_history_with_overweight": "yes" if data.get('familyHistoryOverweight') else "no",
            "FAVC": "yes" if data.get('highCalorieFood') else "no",
            "CAEC": "Sometimes",  # Domyślna wartość
            "SMOKE": "yes" if data.get('smoking') else "no",
            "SCC": "yes" if data.get('calorieMonitoring') else "no",
            "CALC": "Sometimes",  # Domyślna wartość
            "MTRANS": map_transport(data.get('transportation', 1))
        }
        
        app.logger.info(f"Zmapowane features: {features}")
        
        # Predykcja
        predictions = predictor.predict(features)
        
        app.logger.info(f"Predykcja: {predictions}")
        
        return jsonify({
            "predictions": predictions.tolist() if hasattr(predictions, 'tolist') else [str(predictions)]
        })
        
    except Exception as e:
        app.logger.error(f"Błąd predykcji: {str(e)}")
        return jsonify({"error": str(e)}), 500

def map_transport(transport_value):
    """Mapuje wartość transportu z frontendu"""
    transport_map = {
        0: "Walking",
        1: "Public_Transportation", 
        2: "Automobile",
        3: "Bike"
    }
    return transport_map.get(transport_value, "Public_Transportation")

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5000, debug=False)
