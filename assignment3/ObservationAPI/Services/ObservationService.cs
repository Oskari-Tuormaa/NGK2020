using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ObservationAPI.Models;
using ObservationAPI.Helpers;

namespace ObservationAPI.Services
{
    public interface IObservationService
    {
        Observation Upload(Observation newObservation);
        Observation Delete(int id);
        IEnumerable<Observation> GetAll();
        IEnumerable<Observation> GetLatest();
        IEnumerable<Observation> GetDate(DateTime date);
        IEnumerable<Observation> GetPeriod(DateTime date1, DateTime date2);
    }

    public class ObservationService : IObservationService
    {
        private readonly AppSettings _appSettings;

        public ObservationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Observation Upload(Observation newObservation)
        {
            if (!File.Exists(_appSettings.ObservationDir))
                File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    File.ReadAllText(_appSettings.ObservationDir));

            // Assign lowest available id to observation
            int id = 1;
            while (observations.Find(x => x.Id == id) != null)
                id++;
            newObservation.Id = id;

            observations.Add(newObservation);

            File.WriteAllText(_appSettings.ObservationDir,
                    JsonConvert.SerializeObject(observations));
            return newObservation;
        }

        public Observation Delete(int id)
        {
            if (!File.Exists(_appSettings.ObservationDir))
                File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    File.ReadAllText(_appSettings.ObservationDir));

            Observation toRemove = observations.Find(x => x.Id == id);

            if (toRemove == null)
                return null;

            observations.Remove(toRemove);

            File.WriteAllText(_appSettings.ObservationDir,
                    JsonConvert.SerializeObject(observations));

            return toRemove;
        }

        public IEnumerable<Observation> GetAll()
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return observations;
        }

        public IEnumerable<Observation> GetLatest()
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            observations.Sort((x, y) => DateTime.Compare(y.Date, x.Date));

            return observations.Take(3);
        }

        public IEnumerable<Observation> GetDate(DateTime date)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return observations.FindAll(x => x.Date.Date.Equals(date.Date));
        }

        public IEnumerable<Observation> GetPeriod(DateTime date1, DateTime date2)
        {
            if (!System.IO.File.Exists(_appSettings.ObservationDir))
                System.IO.File.WriteAllText(_appSettings.ObservationDir, "[]");

            List<Observation> observations = JsonConvert.DeserializeObject<List<Observation>>(
                    System.IO.File.ReadAllText(_appSettings.ObservationDir));

            return observations.FindAll(x => x.Date >= date1 && x.Date <= date2);
        }
    }
}


