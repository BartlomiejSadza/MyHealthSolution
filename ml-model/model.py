import pickle
import pandas as pd
import numpy as np
import os
import logging

# Konfiguracja logowania
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

class HealthPredictor:
    def __init__(self):
        self.model = None
        self.use_dummy = False
        self.load_model()
    
    def load_model(self):
        """Ładuje model XGBoost z Databricks lub używa dummy modelu"""
        # Sprawdź różne możliwe ścieżki
        possible_paths = [
            'models/Model.pkl',
            '/app/models/Model.pkl',
            'Model.pkl',
            '/app/Model.pkl'
        ]
        
        logger.info("Sprawdzam dostępne pliki:")
        for root, dirs, files in os.walk('/app'):
            for file in files:
                if file.endswith('.pkl'):
                    logger.info(f"Znaleziony plik .pkl: {os.path.join(root, file)}")
        
        model_path = None
        for path in possible_paths:
            logger.info(f"Sprawdzam ścieżkę: {path}")
            if os.path.exists(path):
                model_path = path
                logger.info(f"Znaleziono model w: {path}")
                break
        
        if model_path is None:
            logger.warning("Model nie znaleziony - używam dummy modelu")
            self.use_dummy = True
            return
        
        try:
            logger.info(f"Próbuję załadować model z: {model_path}")
            
            # Sprawdź rozmiar pliku
            file_size = os.path.getsize(model_path)
            logger.info(f"Rozmiar pliku modelu: {file_size / (1024*1024):.1f} MB")
            
            # Jeśli plik jest bardzo duży, może być problemem
            if file_size > 50 * 1024 * 1024:  # 50MB
                logger.warning("Model jest bardzo duży - może być problem z ładowaniem")
            
            # Spróbuj załadować model - jeśli się nie uda, użyj dummy
            with open(model_path, 'rb') as f:
                loaded_object = pickle.load(f)
                
                logger.info(f"Model załadowany pomyślnie!")
                logger.info(f"Typ: {type(loaded_object)}")
                
                if hasattr(loaded_object, 'predict'):
                    self.model = loaded_object
                    logger.info("Model gotowy do predykcji")
                else:
                    logger.warning("Model nie ma metody predict - używam dummy")
                    self.use_dummy = True
                    
        except Exception as e:
            logger.error(f"Błąd ładowania modelu: {str(e)}")
            logger.warning("Przechodzę na dummy model")
            self.use_dummy = True
    
    def predict(self, features_dict):
        """Wykonuje predykcję"""
        logger.info(f"Otrzymane features: {features_dict}")
        
        if self.use_dummy or self.model is None:
            return self._dummy_predict(features_dict)
        
        try:
            # Spróbuj użyć prawdziwego modelu
            df = pd.DataFrame([features_dict])
            logger.info(f"DataFrame dla modelu: {df.shape}")
            
            predictions = self.model.predict(df)
            logger.info(f"Predykcja z prawdziwego modelu: {predictions}")
            return predictions
            
        except Exception as e:
            logger.error(f"Błąd predykcji z prawdziwego modelu: {str(e)}")
            logger.warning("Używam dummy predykcji")
            return self._dummy_predict(features_dict)
    
    def _dummy_predict(self, features_dict):
        """Dummy predykcja na podstawie BMI i innych czynników"""
        try:
            height = features_dict.get('Height', 1.7)
            weight = features_dict.get('Weight', 70)
            age = features_dict.get('Age', 25)
            activity = features_dict.get('FAF', 1)
            
            # Oblicz BMI
            bmi = weight / (height ** 2) if height > 0 else 25
            
            # Uwzględnij wiek i aktywność
            if age > 50:
                bmi_threshold_normal = 26  # Wyższy próg dla starszych
            else:
                bmi_threshold_normal = 25
                
            if activity > 2:  # Wysoka aktywność
                bmi_threshold_normal += 1
            
            # Klasyfikacja
            if bmi < 18.5:
                result = "Insufficient_Weight"
            elif bmi < bmi_threshold_normal:
                result = "Normal_Weight"
            elif bmi < 30:
                if bmi < 27:
                    result = "Overweight_Level_I"
                else:
                    result = "Overweight_Level_II"
            elif bmi < 35:
                result = "Obesity_Type_I"
            elif bmi < 40:
                result = "Obesity_Type_II"
            else:
                result = "Obesity_Type_III"
                
            logger.info(f"Dummy predykcja: BMI={bmi:.1f}, wiek={age}, aktywność={activity} -> {result}")
            return np.array([result])
            
        except Exception as e:
            logger.error(f"Błąd w dummy predykcji: {str(e)}")
            return np.array(["Normal_Weight"])
