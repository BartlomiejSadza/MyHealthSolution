#!/bin/bash

# Test aplikacji przed deploymentem na Azure
set -e

echo "🧪 Testowanie aplikacji przed deploymentem na Azure..."

# 1. Test lokalnego buildu wszystkich kontenerów
echo "🔨 Testuję build wszystkich obrazów..."

echo "Building ML Model..."
docker build -t myhealth-ml-model:test ./ml-model

echo "Building API..."  
docker build -t myhealth-api:test ./MyHealth.Api

echo "Building Frontend..."
docker build -t myhealth-frontend:test ./health-frontend

echo "✅ Wszystkie obrazy zbudowane pomyślnie!"

# 2. Test docker-compose
echo "🐳 Testuję docker-compose..."
docker compose up -d

# Czekaj na uruchomienie
echo "⏳ Czekam na uruchomienie kontenerów..."
sleep 30

# 3. Test health checks
echo "🏥 Sprawdzam health checks..."

# Test ML Model
echo "Testing ML Model health..."
curl -f http://localhost:5002/health || echo "❌ ML Model health check failed"

# Test API health  
echo "Testing API health..."
curl -f http://localhost:5144/api/health || echo "❌ API health check failed"

# Test Frontend
echo "Testing Frontend..."
curl -f http://localhost:3001 || echo "❌ Frontend test failed"

# 4. Test end-to-end prediction
echo "🎯 Testuję predykcję end-to-end..."
curl -X POST http://localhost:5144/api/health/assess-simple \
  -H "Content-Type: application/json" \
  -d '{
    "age": 25,
    "gender": "Female", 
    "height": 1.7,
    "weight": 70,
    "fcvc": 2,
    "ncp": 3,
    "ch2o": 2,
    "faf": 1,
    "tue": 1,
    "familyHistoryWithOverweight": "no",
    "favc": "no",
    "caec": "Sometimes",
    "smoke": "no",
    "scc": "no",
    "calc": "Sometimes",
    "mtrans": "Public_Transportation"
  }' || echo "❌ End-to-end test failed"

echo ""
echo "🧹 Sprzątam po testach..."
docker compose down

echo ""
echo "✅ TESTY ZAKOŃCZONE!"
echo ""
echo "🚀 Jeśli wszystko przeszło, możesz teraz deployować na Azure:"
echo "chmod +x deploy-azure.sh && ./deploy-azure.sh" 