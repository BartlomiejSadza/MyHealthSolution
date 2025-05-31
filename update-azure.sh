#!/bin/bash

# Szybka aktualizacja MyHealth Solution na Azure
set -e

# Konfiguracja
RESOURCE_GROUP="rg-myhealth-fixed"
REGISTRY_NAME="myhealthfixed1637"
LOCATION="eastus"

# Kolory dla outputu
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🔄 Aktualizacja MyHealth Solution na Azure...${NC}"

# Sprawdź parametry
if [ $# -eq 0 ]; then
    echo -e "${YELLOW}Użycie:${NC}"
    echo "./update-azure.sh [frontend|api|ml-model|all]"
    echo ""
    echo -e "${YELLOW}Przykłady:${NC}"
    echo "./update-azure.sh frontend    # Aktualizuj tylko frontend"
    echo "./update-azure.sh api         # Aktualizuj tylko API"
    echo "./update-azure.sh ml-model    # Aktualizuj tylko ML Model"
    echo "./update-azure.sh all         # Aktualizuj wszystko"
    exit 1
fi

COMPONENT=$1
TIMESTAMP=$(date +%Y%m%d-%H%M%S)

# Pobierz ACR server
ACR_SERVER=$(az acr show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP --query "loginServer" --output tsv)

# Login do ACR
echo -e "${BLUE}🔐 Loguję do Azure Container Registry...${NC}"
az acr login --name $REGISTRY_NAME

update_frontend() {
    echo -e "${GREEN}🌐 Aktualizuję Frontend...${NC}"
    
    # Build i push
    docker build -t $ACR_SERVER/frontend:$TIMESTAMP ./health-frontend
    docker push $ACR_SERVER/frontend:$TIMESTAMP
    
    # Update Container App
    az containerapp update \
        --name myhealth-frontend \
        --resource-group $RESOURCE_GROUP \
        --image $ACR_SERVER/frontend:$TIMESTAMP
    
    echo -e "${GREEN}✅ Frontend zaktualizowany!${NC}"
}

update_api() {
    echo -e "${GREEN}⚙️ Aktualizuję API...${NC}"
    
    # Build i push
    docker build -t $ACR_SERVER/api:$TIMESTAMP ./MyHealth.Api
    docker push $ACR_SERVER/api:$TIMESTAMP
    
    # Update Container App
    az containerapp update \
        --name myhealth-api \
        --resource-group $RESOURCE_GROUP \
        --image $ACR_SERVER/api:$TIMESTAMP
    
    echo -e "${GREEN}✅ API zaktualizowane!${NC}"
}

update_ml_model() {
    echo -e "${GREEN}🤖 Aktualizuję ML Model...${NC}"
    
    # Build i push
    docker build -t $ACR_SERVER/ml-model:$TIMESTAMP ./ml-model
    docker push $ACR_SERVER/ml-model:$TIMESTAMP
    
    # Update Container App
    az containerapp update \
        --name myhealth-ml-model \
        --resource-group $RESOURCE_GROUP \
        --image $ACR_SERVER/ml-model:$TIMESTAMP
    
    echo -e "${GREEN}✅ ML Model zaktualizowany!${NC}"
}

# Wykonaj aktualizację
case $COMPONENT in
    "frontend")
        update_frontend
        ;;
    "api")
        update_api
        ;;
    "ml-model")
        update_ml_model
        ;;
    "all")
        update_ml_model
        sleep 10  # Czekaj żeby ML Model się uruchomił
        update_api
        sleep 10  # Czekaj żeby API się uruchomił
        update_frontend
        ;;
    *)
        echo -e "${RED}❌ Nieznany komponent: $COMPONENT${NC}"
        echo "Dostępne opcje: frontend, api, ml-model, all"
        exit 1
        ;;
esac

# Pokaż status
echo ""
echo -e "${BLUE}📊 Status aplikacji:${NC}"
az containerapp list --resource-group $RESOURCE_GROUP --output table

echo ""
echo -e "${GREEN}🎉 Aktualizacja zakończona!${NC}"

# Pokaż URLs
FRONTEND_URL=$(az containerapp show --name myhealth-frontend --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" --output tsv)
echo -e "${BLUE}🌐 URL aplikacji: https://$FRONTEND_URL${NC}"

echo ""
echo -e "${YELLOW}💡 Sprawdź logi w razie problemów:${NC}"
echo "az containerapp logs show --name myhealth-$COMPONENT --resource-group $RESOURCE_GROUP --follow" 