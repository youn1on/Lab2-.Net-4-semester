using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using Infrastructure.Models;

namespace Lab2;

public class Queries
{
    private XDocument _doc;

    public Queries(XDocument doc)
    {
        _doc = doc;
    }
    
    // 1. Знайдемо максимальний час подорожі між містами. 

    public TimeSpan GetMaximumTravelTime(int trainId)
    {
        return _doc.Root.Element("Schedules").Descendants("Schedule")
            .Where(schedule => schedule.Element("TrainId").Value == trainId.ToString())
            .Select(schedule => DateTime.Parse(schedule.Element("DateTimeOfArrival").Value) -
                                DateTime.Parse(schedule.Element("DateTimeOfDeparture").Value)).Max();
    }

    // 2. Знайдемо кількість місць у потягах.
    public IEnumerable<(Train, int)> GetNumberOfSeats()
    {
        return _doc.Root.Element("Wagons").Descendants("Wagon")
            .GroupBy(wagon => wagon.Element("InventaryNumberOfTrain").Value)
            .Select(wagons =>
                (GetTrainById(wagons.Key)!, wagons.Sum(w => int.Parse(w.Element("AmountOfSeats")?.Value))));
    }
 
 
     // 3. Знайдемо розклади з певного міста.
     public IEnumerable<Schedule> GetSchedulesFromTown(string townName)
     {
         XElement? town = _doc.Root.Element("Towns").Descendants("Town")
             .FirstOrDefault(t => t.Element("Name").Value == townName);
         
         return _doc.Root.Element("Schedules").Descendants("Schedule").Where(s => s.Element("TownFromId").Value == town?.Element("Id").Value)
             .OrderBy(s => DateTime.Parse(s.Element("DateTimeOfDeparture").Value)).Select(xe => xe.ToSchedule());
     }
 
     // 4. Знайдемо розклад між містами, час поїздки між якими найменший.
     public Schedule? GetSmallestTimeSchedule()
     {
         return _doc.Root.Element("Schedules").Descendants("Schedule").MinBy(s =>
             DateTime.Parse(s.Element("DateTimeOfArrival").Value) -
             DateTime.Parse(s.Element("DateTimeOfDeparture").Value)).ToSchedule();
     }
 
     // 5. Знайдемо найдовший час зупинки, місто, в якому ця зупинка відбувається та відповідний потяг.
     public IEnumerable<(Train, Town, double)> GetMaxStopTimeTown()
     {

         return _doc.Root.Element("Schedules").Descendants("Schedule").GroupBy(s =>
             s.Element("TrainId").Value).Select(g => (GetTrainById(g.Key), g.Select(s =>
             (s, g.Where(s2 => s2.Element("TownFromId").Value == s.Element("TownToId").Value
                               && DateTime.Parse(s2.Element("DateTimeOfDeparture").Value)
                                   .Subtract(DateTime.Parse(s.Element("DateTimeOfArrival").Value))
                               >= new TimeSpan()).MinBy(s2 => DateTime.Parse(s2.Element("DateTimeOfDeparture").Value)
                 .Subtract(DateTime.Parse(s.Element("DateTimeOfArrival").Value))))).Select(p
             => (p.Item1, p.Item2,
                 (double?)(p.Item2 is not null
                     ? DateTime.Parse(p.Item2!.Element("DateTimeOfDeparture")?.Value)
                         .Subtract(DateTime.Parse(p.Item1.Element("DateTimeOfArrival").Value)).TotalMinutes
                     : null))).MaxBy(p => p.Item3))).Select(i =>
             (i.Item1!, GetTownById(i.Item2.Item1.Element("TownToId").Value)!, i.Item2.Item3!.Value));
     }

     // 6. Знайдемо потяги з хоча б одним люксовим вагоном.
 
     public IEnumerable<Train> GetLuxTrains()
     {
         return _doc.Root.Element("Wagons").Descendants("Wagon")
             .Where(w => w.Element("Type").Value == WagonType.Sleeping.ToString())
             .Select(w => GetTrainById(w.Element("InventaryNumberOfTrain").Value)).Distinct();
     }
     
     // 7. Вибираємо унікальні потяги, які прибувають в Київ у вказану дату.
 
     public IEnumerable<Train> GetKyivTrainsByDate(DateTime date)
     {
         return (from t1 in _doc.Root.Element("Schedules").Descendants("Schedule")
             join t2 in _doc.Root.Element("Towns").Descendants("Town") on t1.Element("TownToId").Value equals
                 t2.Element("Id").Value
             where DateTime.Parse(t1.Element("DateTimeOfArrival").Value).Date == date.Date &&
                   t2.Element("Name").Value == "Kyiv"
             select GetTrainById(t1.Element("TrainId").Value)).Distinct();
     }
 
     // 8. Знайдемо потяги по відповідальній людині.
 
     public IEnumerable<Train> GetTrainsByResponsiblePerson(Person person)
     {
         return (from t1 in _doc.Root.Element("Trains").Descendants("Train")
             join t2 in _doc.Root.Element("ResponsiblePeople").Descendants("Person") on t1.Element("ResponsiblePersonId").Value equals
                 t2.Element("Id").Value
             where $"{t2.Element("Name").Value} {t2.Element("Surname").Value}" == person.Name + " " + person.Surname
             select GetTrainById(t1.Element("InventaryNumber").Value));
     }
     
     // 9. Знайдемо потяги, в яких більше n-ої кількості плацкартних вагонів.
 
     public IEnumerable<Train> GetTrainsWithMoreReservedWagonsThan(int n)
     {
         return _doc.Root.Element("Trains").Descendants("Train").Where(t =>
             _doc.Root.Element("Wagons").Descendants("Wagon").Count(w => w.Element("InventaryNumberOfTrain").Value ==
             t.Element("InventaryNumber").Value) > n).Select(xe => xe.ToTrain());
     }
 
     // 10. Знайдемо потяг з найбільшою кількістю місць.
 
     public Train GetTrainWithMostAmountOfSeats()
     {
         return _doc.Root.Element("Wagons").Descendants("Wagon")
             .GroupBy(wagon => wagon.Element("InventaryNumberOfTrain").Value).Select(wagons =>
                 (GetTrainById(wagons.Key), wagons.Sum(w => int.Parse(w.Element("AmountOfSeats").Value))))
             .MaxBy(p => p.Item2).Item1;
     }
     
     // 11. Знайдемо потяг з найбільшою кількістю зупинок.
 
     public Train GetTrainWithMostStops()
     {
         return _doc.Root.Element("Schedules").Descendants("Schedule").GroupBy(schedule => schedule.Element("TrainId").Value)
             .Select((g => (GetTrainById(g.Key), g.Select(el => el.Element("TownToId")).Distinct().Count())))
             .MaxBy(t => t.Item2).Item1;
     }
     
     
     // 12. Знайдемо, скільки потягів проходять через станцію.
 
     public IEnumerable<(Town, int)> GetTrainAmountThroughEachStation()
     {
         return _doc.Root.Element("Schedules").Descendants("Schedule")
             .GroupBy(schedule => schedule.Element("TownFromId").Value).Select(g =>
                 (GetTownById(g.Key), g.Select(el => el.Element("TrainId")).Distinct().Count()));
     }
 
     // 13. Список міст де наразі потяги не курсують.
 
     public IEnumerable<Town?> GetInactiveTowns()
     {
         return _doc.Root.Element("Schedules").Descendants("Schedule").GroupBy(s => s.Element("TownFromId").Value)
             .Where(g => !g.Any()).Select(g => GetTownById(g.Key));
     }
 
     // 14. Знайдемо відповідальних осіб, в яких ім'я починається на "M" та виведемо потяги, за які вони відповідають.
 
     public IEnumerable<(Person, Train)> GetResponsiblePersonStartsWithM()
     {
         return _doc.Root.Element("Trains").Descendants("Train")
             .Select(t => (GetPersonById(t.Element("ResponsiblePersonId").Value), t.ToTrain()))
             .Where(t => t.Item1!.Name.StartsWith("М"));
     }
     
     // 15. Отримаємо всі міста, через які їде певний потяг.
     
     public IEnumerable<Town> GetTownsByTrain(Train train)
     {
         return _doc.Root.Element("Schedules").Descendants("Schedule").Where(s => s.Element("TrainId").Value == train.Id.ToString())
             .Select(s => GetTownById(s.Element("TownToId").Value)).Distinct();
     }

     public Town? GetTownById(string id)
     {
         return _doc.Root?.Element("Towns")?.Descendants("Town").FirstOrDefault(t => t.Element("Id")?.Value == id)?.ToTown();
     }
     
     public Train? GetTrainById(string id)
     {
         return _doc.Root?.Element("Trains")?.Descendants("Train").FirstOrDefault(t => t.Element("InventaryNumber")?.Value == id)?.ToTrain();
     }
     
     public Person? GetPersonById(string id)
     {
         return _doc.Root?.Element("ResponsiblePeople")?.Descendants("Person").FirstOrDefault(t => t.Element("Id")?.Value == id)
             ?.ToPerson();
     }
     
     public Wagon? GetWagonById(string id)
     {
         return _doc.Root?.Element("Wagons")?.Descendants("Wagon").FirstOrDefault(t => t.Element("Id")?.Value == id)
             ?.ToWagon();
     }
     
     public Schedule? GetScheduleById(string id)
     {
         return _doc.Root?.Element("Schedules")?.Descendants("Schedule")
             .FirstOrDefault(t => t.Element("Id")?.Value == id)
             ?.ToSchedule();
     }
}