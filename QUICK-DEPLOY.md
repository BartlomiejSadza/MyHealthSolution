# 🚀 Quick Deploy na Azure - 5 minut

## Prerequisites

1. **Azure Account** - Potrzebujesz aktywną subskrypcję Azure
2. **Azure CLI** - Zainstalowane i skonfigurowane
3. **Docker** - Uruchomiony lokalnie

## 1-klik deployment

```bash
# 1. Zaloguj się do Azure
az login

# 2. (Opcjonalnie) Przetestuj lokalnie
./test-before-deploy.sh

# 3. Deploy na Azure
./deploy-azure.sh
```

## To wszystko! 🎉

Po 10-15 minutach otrzymasz:

- ✅ Działającą aplikację w chmurze Azure
- 🌐 Publiczny URL do aplikacji
- 💰 Koszt ~$20-50/miesiąc

## URLs po deploymencie

- **Aplikacja główna**: `https://myhealth-frontend-xxx.azurecontainerapps.io`
- **API**: `https://myhealth-api-xxx.azurecontainerapps.io`
- **Monitoring**: Azure Portal → rg-myhealth

## Troubleshooting

Jeśli coś nie działa:

```bash
# Sprawdź logi
az containerapp logs show --name myhealth-frontend --resource-group rg-myhealth
az containerapp logs show --name myhealth-api --resource-group rg-myhealth
az containerapp logs show --name myhealth-ml-model --resource-group rg-myhealth

# Sprawdź status
az containerapp list --resource-group rg-myhealth --output table
```

## Usuwanie

```bash
# Usuń wszystkie zasoby (UWAGA: nieodwracalne!)
az group delete --name rg-myhealth --yes --no-wait
```

---

**Koszt estymowany**: ~$48/miesiąc  
**Czas deployment**: ~15 minut  
**Trudność**: ⭐⭐☆☆☆
