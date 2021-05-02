using Common.Data;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DatabaseGenerator.Data
{
    public class DbInitializer
    {
        private static Random r = new Random();

        private static string[] subjectArray =
        {
            "Bevezetés a számításelméletbe 1", "Digitális technika", "A programozás alapjai 1", "Analízis 1 informatikusoknak", "Bevezető fizika", "Bevezető matematika", "Fizika 1i", "Mérnök leszek", "Analízis 2 informatikusoknak", "Analízis szigorlat informatikusoknak", "A programozás alapjai 2", "Bevezetés a számításelméletbe 2", "Fizika 2i", "Rendszermodellezés", "Számítógép-architektúrák", "A programozás alapjai 3", "Adatbázisok", "Kódolástechnika", "Kommunikációs hálózatok 1", "Rendszerelmélet", "Szoftvertechnológia", "Valószínűségszámítás", "Algoritmuselmélet", "Kommunikációs hálózatok 2", "Menedzsment és vállalkozásgazdaságtan", "Operációs rendszerek", "Számítógépes grafika", "Szoftver projekt laboratórium", "Szoftvertechnikák", "IT eszközök technológiája", "Mesterséges intelligencia", "Mikro- és makroökonómia", "Mobil- és webes szoftverek", "Üzleti jog", "Információs rendszerek üzemeltetése", "IT biztonság", "Beszédinformációs rendszerek", "Deklaratív programozás", "Mobil kommunikációs hálózatok", "Hálózatok építése és üzemeltetése", "Médiaalkalmazások és -hálózatok a gyakorlatban", "Hálózatba kapcsolt erőforrás platformok és alkalmazásaik", "Infokommunikáció laboratórium 1", "Infokommunikáció laboratórium 2", "Informatikai rendszertervezés", "Ipari informatika", "Alkalmazásfejlesztési környezetek", "Intelligens elosztott rendszerek", "Rendszertervezés laboratórium 1", "Rendszertervezés laboratórium 2", "Adatvezérelt rendszerek", "Objektumorientált szoftvertervezés", "Integrációs és ellenőrzési technikák", "Szoftverfejlesztés laboratórium 1", "Szoftverfejlesztés laboratórium 2", "Termelésinformatika", "Vállalatirányítási rendszerek", "Adatelemzés", "Gazdálkodási információmenedzsment", "Vállalati rendszerek programozása laboratórium", "Vállalati jelentéskészítés laboratórium"
        };
        private static List<string> subjectList = subjectArray.ToList();
        public static void Initialize(SchoolContext context)
        {
            context.Database.EnsureCreated();

            // DB has been initialized
            if (context.Students.Any())
            {
                return;
            }

            var students = new List<Student>();
            for (int i = 0; i < 100; i++)
            {
                students.Add(new Student
                {
                    Name = Name(),
                    DayOfBirth = DayOfBirth().ToString("yyyy.MM.dd."),
                    Neptun = Neptun(),
                    QRCode = GetQRCode()
                });
            }
            foreach (Student s in students)
            {
                context.Students.Add(s);
            }
            context.SaveChanges();

            Console.WriteLine("Created Students");

            var courses = new List<Course>();
            var fromList = new List<DateTime>();
            var toList = new List<DateTime>();
            for (int i = 0; i < 183; i++)
            {
                DateTime from = GetTime();
                DateTime to = from.AddHours(1.5);
                fromList.Add(from);
                toList.Add(to);
                string type = GetCourseType();
                List<Student> studentList = new List<Student>();
                Random r = new Random();
                int until = r.Next(10);
                for (int j = 0; j < until; j++)
                {
                    studentList.Add(students.ElementAt(r.Next(students.Count)));
                }
                courses.Add(new Course
                {
                    Name = type + GetCourseNumber(),
                    Type = type,
                    From = from.ToString("HH:mm:ss"),
                    To = to.ToString("HH:mm:ss"),
                    Day = GetDayOfWeek(),
                    Students = studentList
                });
            }
            foreach (Course c in courses)
            {
                context.Courses.Add(c);
            }
            context.SaveChanges();

            Console.WriteLine("Created Courses");

            var subjects = new List<Subject>();
            for (int i = 0; i < 61; i++)
            {
                var courseList = new List<Course>();
                for (int j = 0; j < 3; j++)
                {
                    courseList.Add(courses.ElementAt(i * 3 + j));
                }
                subjects.Add(new Subject { Name = GetSubjectName(), Courses = courseList });
            }
            foreach (Subject s in subjects)
            {
                context.Subjects.Add(s);
            }
            context.SaveChanges();

            Console.WriteLine("Created Subjects");

            var attendances = new List<Attendance>();
            for (int i = 0; i < students.Count * 3; i++)
            {
                int shiftTime = r.Next(16);
                int courseRand = r.Next(courses.Count);
                attendances.Add(new Attendance
                {
                    Student = students[r.Next(students.Count)],
                    Course = courses[courseRand],
                    Date = GetDate().ToString("yyyy.MM.dd."),
                    CheckInTime = fromList[courseRand].AddMinutes(shiftTime).ToString("HH:mm:ss"),
                    CheckOutTime = toList[courseRand].Add(new TimeSpan(0, -shiftTime, 0)).ToString("HH:mm:ss")
                });
            }
            foreach (Attendance a in attendances)
            {
                context.Attendances.Add(a);
            }
            context.SaveChanges();

            Console.WriteLine("Created Attendances");
        }

        private static byte[] GetQRCode()
        {
            string currentDirectory = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
            string path = Path.Combine(currentDirectory, @"QRCode.png");
            return File.ReadAllBytes(path);
        }

        private static string Name()
        {
            string[] lastNames =
            {
                "Nagy", "Lukács", "Jónás", "Kovács", "Gulyás", "Szücs", "Tóth", "Biró", "Hajdu", "Szabó", "Király", "Halász", "Horváth", "Balog", "Máté", "Varga", "László", "Székely", "Kiss", "Bogdán", "Gáspár", "Molnár", "Jakab", "Kozma", "Németh", "Katona", "Pásztor", "Farkas", "Sándor", "Bakos", "Balogh", "Váradi", "Dudás", "Papp", "Boros", "Virág", "Lakatos", "Fazekas", "Major", "Takács", "Kelemen", "Orbán", "Juhász", "Antal", "Hegedüs", "Oláh", "Orosz", "Barna", "Mészáros", "Somogyi", "Novák", "Simon", "Fülöp", "Soós", "Rácz", "Veres", "Tamás", "Fekete", "Budai", "Nemes", "Szilágyi", "Vincze", "Pataki", "Török", "Hegedűs", "Balla", "Fehér", "Deák", "Faragó", "Balázs", "Pap", "Kerekes", "Gál", "Bálint", "Barta", "Kis", "Illés", "Péter", "Szűcs", "Pál", "Borbély", "Orsós", "Vass", "Csonka", "Kocsis", "Szőke", "Mezei", "Fodor", "Fábián", "Sárközi", "Pintér", "Vörös", "Berki", "Szalai", "Lengyel", "Márton", "Sipos", "Bognár", "Magyar", "Bodnár"
            };

            string[] firstNames =
            {
                "László", "István", "József", "János", "Zoltán", "Sándor", "Gábor", "Ferenc", "Attila", "Péter", "Tamás", "Zsolt", "Tibor", "András", "Csaba", "Imre", "Lajos", "György", "Balázs", "Gyula", "Mihály", "Károly", "Róbert", "Béla", "Dávid", "Dániel", "Ádám", "Krisztián", "Miklós", "Norbert", "Bence", "Máté", "Pál", "Szabolcs", "Roland", "Gergő", "Antal", "Bálint", "Richárd", "Márk", "Levente", "Gergely", "Ákos", "Viktor", "Árpád", "Géza", "Márton", "Kristóf", "Jenő", "Kálmán", "Patrik", "Martin", "Milán", "Barnabás", "Dominik", "Marcell", "Ernő", "Mátyás", "Endre", "Áron", "Dezső", "Botond", "Nándor", "Zsombor", "Szilárd", "Erik", "Olivér", "Alex", "Vilmos", "Ottó", "Benedek", "Dénes", "Kornél", "Bertalan", "Benjámin", "Zalán", "Kevin", "Adrián", "Rudolf", "Albert", "Vince", "Ervin", "Győző", "Zsigmond", "Andor", "Gusztáv", "Szilveszter", "Iván", "Noel", "Barna", "Elemér", "Arnold", "Csongor", "Ábel" ,"Krisztofer", "Emil", "Tivadar", "Hunor", "Bendegúz", "Henrik", "Mária", "Erzsébet", "Katalin", "Ilona", "Éva", "Anna", "Zsuzsanna", "Margit", "Judit", "Ágnes", "Julianna", "Andrea", "Ildikó", "Erika", "Krisztina", "Irén", "Eszter", "Magdolna", "Mónika", "Edit", "Gabriella", "Szilvia", "Anita", "Anikó", "Viktória", "Márta", "Rozália", "Tímea", "Piroska", "Ibolya", "Klára", "Tünde", "Dóra", "Zsófia", "Gizella", "Veronika", "Alexandra", "Csilla", "Terézia", "Nikolett", "Melinda", "Adrienn", "Réka", "Beáta", "Marianna", "Nóra", "Renáta", "Vivien", "Barbara", "Enikő", "Bernadett", "Rita", "Brigitta", "Edina", "Hajnalka", "Gyöngyi", "Jolán", "Petra", "Orsolya", "Etelka", "Boglárka", "Borbála", "Noémi", "Valéria", "Teréz", "Annamária", "Fanni", "Kitti", "Nikoletta", "Emese", "Aranka", "Laura", "Lilla", "Róza", "Klaudia", "Anett", "Kinga", "Zita", "Beatrix", "Zsanett", "Rózsa", "Emma", "Dorina", "Hanna", "Lili", "Sára", "Irma", "Bianka", "Júlia", "Györgyi", "Henrietta", "Diána", "Luca", "Mariann", "Bettina", "Dorottya", "Virág", "Jázmin", "Sarolta", "Evelin"
            };
            return lastNames[r.Next(lastNames.Length)] + " " + firstNames[r.Next(firstNames.Length)];
        }

        private static DateTime DayOfBirth()
        {
            DateTime from = new DateTime(1995, 1, 1);
            DateTime to = new DateTime(2004, 1, 1);
            int range = (to - from).Days;
            return from.AddDays(r.Next(range)).Date;
        }

        private static DateTime GetDate()
        {
            DateTime from = new DateTime(2021, 1, 1);
            DateTime to = DateTime.Now;
            int range = (to - from).Days;
            return from.AddDays(r.Next(range)).Date;
        }

        private static string Neptun()
        {
            int size = 6;
            string characters = "ABCDEFGHIJKLMNOPQRST0123456789";
            string str = "";
            for (int i = 0; i < size; i++)
            {
                str += characters[r.Next(characters.Length)];
            }
            return str;
        }

        private static DateTime GetTime()
        {
            int from = r.Next(4, 10) * 2;
            return DateTime.Parse(from + ":15:00");
        }

        private static int GetDayOfWeek()
        {
            return r.Next(7) + 1;
        }

        private static string GetCourseType()
        {
            int randMod = r.Next() % 5;
            if (randMod == 0)
            {
                return "E";
            }
            else if (randMod > 0 && randMod < 3)
            {
                return "Gy";
            }
            else
            {
                return "L";
            }
        }

        private static int GetCourseNumber()
        {
            return r.Next(10);
        }

        private static string GetSubjectName()
        {
            if (subjectList.Count == 0)
            {
                return "Tárgy" + r.Next();
            }
            string rand = subjectList.ElementAt(r.Next(subjectList.Count));
            subjectList.Remove(rand);
            return rand;
        }
    }
}
