FROM python:3.11-slim

WORKDIR /app

# Instalacja zależności systemowych
RUN apt-get update && apt-get install -y \
    gcc \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Kopiowanie i instalacja zależności Python
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Tworzenie folderu na modele
RUN mkdir -p models

# Kopiowanie modelu PRZED kopiowaniem reszty kodu
COPY Model.pkl models/Model.pkl

# Kopiowanie reszty kodu aplikacji
COPY app.py .
COPY model.py .

EXPOSE 5000

# Uruchomienie aplikacji
CMD ["gunicorn", "--bind", "0.0.0.0:5000", "--workers", "1", "--timeout", "120", "app:app"]
