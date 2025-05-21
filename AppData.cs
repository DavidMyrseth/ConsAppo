using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Teku
{
    public static class AppData
    {
        public static List<Consultation> Consultations { get; private set; } = new List<Consultation>();
        private static readonly string ConsultationsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "consultations.txt");

        static AppData()
        {
            LoadConsultations();
        }

        public static void LoadConsultations()
        {
            Consultations = new List<Consultation>();

            if (File.Exists(ConsultationsFilePath))
            {
                foreach (var line in File.ReadAllLines(ConsultationsFilePath))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Consultations.Add(Consultation.FromString(line));
                    }
                }
            }
            else
            {
                // Default consultations
                Consultations.Add(new Consultation { Teacher = "Mr. Smith", Room = "101", Time = "10:00-12:00", Day = "Monday" });
                Consultations.Add(new Consultation { Teacher = "Ms. Johnson", Room = "202", Time = "14:00-16:00", Day = "Tuesday" });
                SaveConsultations();
            }
        }

        public static void SaveConsultations()
        {
            File.WriteAllLines(ConsultationsFilePath, Consultations.Select(c => c.ToString()));
        }
    }
}