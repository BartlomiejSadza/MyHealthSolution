# MyHealth Assistant 🏥

AI-powered health assessment application that evaluates health status based on lifestyle factors.

## 🚀 Features

- **Complete Health Assessment Form** - 17 comprehensive health parameters
- **AI-Powered Analysis** - Uses Databricks ML model for health predictions
- **Polish Language Support** - Fully localized interface and responses
- **Modern UI** - Built with Next.js and Tailwind CSS
- **RESTful API** - .NET Core backend with Swagger documentation
- **Real-time Results** - Instant health analysis with detailed observations

## 🏗️ Architecture

### Backend (MyHealth.Api)

- **.NET 8 Web API** with minimal APIs
- **Databricks Integration** for ML model predictions
- **CORS enabled** for frontend communication
- **Health Service Layer** for business logic
- **Swagger/OpenAPI** documentation

### Frontend (health-frontend)

- **Next.js 15** with Turbopack
- **TypeScript** for type safety
- **Tailwind CSS** for styling
- **Responsive Design** for all devices
- **Form Validation** and error handling

## 📋 Health Parameters

The application analyzes these health factors:

- **Demographics**: Age, Gender, Height, Weight
- **Nutrition**: Vegetable consumption, meal frequency, water intake
- **Lifestyle**: Physical activity, technology usage, transportation
- **Habits**: Family history, smoking, calorie monitoring, alcohol consumption

## 🎯 Health Predictions

The AI model provides predictions for:

- ✅ **Normal Weight**
- ⚠️ **Overweight Level I & II**
- 🔴 **Obesity Type I, II & III**

Each prediction includes:

- Detailed description in Polish
- Specific health observations
- Personalized recommendations

## ⚙️ Setup

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- Databricks account with ML model endpoint

### Backend Setup

1. Clone the repository
2. Copy `MyHealth.Api/appsettings.json.template` to `MyHealth.Api/appsettings.json`
3. Configure your Databricks endpoint and token in `appsettings.json`
4. Run the API:
   ```bash
   cd MyHealth.Api
   dotnet run
   ```

### Frontend Setup

1. Install dependencies:
   ```bash
   cd health-frontend
   npm install
   ```
2. Start the development server:
   ```bash
   npm run dev
   ```

## 🌐 Usage

1. Open `http://localhost:3000` (or 3001 if 3000 is occupied)
2. Fill out the comprehensive health form
3. Click "Wyślij formularz" (Submit Form)
4. View your personalized health analysis

## 🔒 Security

- Sensitive configuration data excluded from repository
- CORS properly configured for frontend-backend communication
- Environment-specific configuration support

## 🤝 Contributing

Feel free to submit issues and enhancement requests!

## 📄 License

This project is for educational and demonstration purposes.
