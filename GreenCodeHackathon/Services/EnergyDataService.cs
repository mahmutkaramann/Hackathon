using GreenCodeHackathon.Data;
using GreenCodeHackathon.Models;
using GreenCodeHackathon.Services;
using Microsoft.EntityFrameworkCore;

namespace EnergyDashboard.Services
{
    public class EnergyDataService : IEnergyDataService
    {
        private readonly EnergyDbContext _db;

        public EnergyDataService(EnergyDbContext db)
        {
            _db = db;
        }

        public List<EnergyPrediction> GetTodaysPredictions()
        {
            try
            {
                // Yerel bugünü UTC aralığına çevir
                var localToday = DateTime.Today;
                var utcFrom = localToday.ToUniversalTime();
                var utcTo = localToday.AddDays(1).ToUniversalTime();

                return _db.EnergyPredictions
                    .Where(e => e.Timestamp >= utcFrom && e.Timestamp < utcTo)
                    .OrderBy(e => e.Timestamp)
                    .ToList();
            }
            catch { return new List<EnergyPrediction>(); }
        }

        public BatteryStatus? GetLatestBatteryStatus()
        {
            try
            {
                return _db.BatteryStatuses
                    .OrderByDescending(b => b.Timestamp)
                    .FirstOrDefault();
            }
            catch { return null; }
        }

        public List<BatteryStatus> GetBatteryHistory(int hours = 24)
        {
            try
            {
                var utcFrom = DateTime.UtcNow.AddHours(-hours);
                return _db.BatteryStatuses
                    .Where(b => b.Timestamp >= utcFrom)
                    .OrderBy(b => b.Timestamp)
                    .ToList();
            }
            catch { return new List<BatteryStatus>(); }
        }


        public WeatherData? GetLatestWeather()
        {
            try
            {
                return _db.WeatherData
                    .OrderByDescending(w => w.Timestamp)
                    .FirstOrDefault();
            }
            catch { return null; }
        }

        public List<EnergyPrediction> GetPredictionsByDateRange(
            DateTime from, DateTime to)
        {
            try
            {
                return _db.EnergyPredictions
                    .Where(e => e.Timestamp >= from && e.Timestamp <= to)
                    .OrderBy(e => e.Timestamp)
                    .ToList();
            }
            catch { return new List<EnergyPrediction>(); }
        }

        public double GetTotalProductionToday()
        {
            try
            {
                var utcFrom = DateTime.Today.ToUniversalTime();
                var utcTo = DateTime.Today.AddDays(1).ToUniversalTime();
                return _db.EnergyPredictions
                    .Where(e => e.Timestamp >= utcFrom && e.Timestamp < utcTo)
                    .Sum(e => e.PredictedKw);
            }
            catch { return 0; }
        }

        public double GetAverageConfidence()
        {
            try
            {
                if (!_db.EnergyPredictions.Any()) return 0;
                return _db.EnergyPredictions.Average(e => e.Confidence);
            }
            catch { return 0; }
        }
    }
}