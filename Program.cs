using System.Xml.Linq;
using Infrastructure.DbContext;
using Infrastructure.Models;

namespace Lab2;

public static class Program
{
    public static void Main()
    {
        DbContext database = DbContext.InitializeDB();
        DbContextToXmlWriter.DbContextToXmlUsingSerializer(database, "databaseN.xml");

        XDocument doc = XDocument.Load("databaseN.xml");
        Queries queries = new Queries(doc);

        //1
        var maximumTravelTime = queries.GetMaximumTravelTime(1);
        Console.WriteLine(
            $"Максимальний час подорожi мiж мiстами для потягу {queries.GetTrainById("1")}:\n\t{maximumTravelTime.Hours}h {maximumTravelTime.Minutes}m\n");

        //2
        var seatsInTrains = queries.GetNumberOfSeats();
        Console.WriteLine("Кiлькiсть мiсць у кожному потязi:");
        foreach (var pair in seatsInTrains)
        {
            Console.WriteLine($"\t{pair.Item1} : {pair.Item2} мiсць");
        }

        Console.WriteLine();

        //3
        var schedulesFromTown = queries.GetSchedulesFromTown("Lviv");
        Console.WriteLine("Розклади з мiста Львова:");
        foreach (var schedule in schedulesFromTown)
        {
            Console.WriteLine(
                $"\tПотяг {queries.GetTrainById(schedule.TrainId.ToString())} вiдправляється з " +
                $"{queries.GetTownById(schedule.TownFromId.ToString())} {schedule.DateTimeOfDeparture} i " +
                $"прибуває в {queries.GetTownById(schedule.TownToId.ToString())} {schedule.DateTimeOfArrival}");
        }

        Console.WriteLine();

        //4
        var smallestTimeSchedule = queries.GetSmallestTimeSchedule();
        Console.WriteLine("Розклад мiж мiстами, час поїздки мiж якими найменший:");
        Console.WriteLine($"\tПотяг {queries.GetTrainById(smallestTimeSchedule.TrainId.ToString())} вiдправляється з " +
                          $"{queries.GetTownById(smallestTimeSchedule.TownFromId.ToString())} {smallestTimeSchedule.DateTimeOfDeparture} i " +
                          $"прибуває в {queries.GetTownById(smallestTimeSchedule.TownToId.ToString())} {smallestTimeSchedule.DateTimeOfArrival}");
        ;
        Console.WriteLine();

        //5 

        var maxStopTimeTown = queries.GetMaxStopTimeTown();
        Console.WriteLine("Максимальний час зупинки:");
        foreach (var element in maxStopTimeTown)
        {
            Console.WriteLine($"\tПотяг {element.Item1} стоїть у мiстi {element.Item2} протягом {element.Item3} хвилин.");
        }

        Console.WriteLine();

        //6

        var atLeastOneLuxWagonTrain = queries.GetLuxTrains();
        Console.WriteLine("Потяги з хоча б одним люксовим вагоном:");
        foreach (var luxTrain in atLeastOneLuxWagonTrain)
        {
            Console.WriteLine($"\t{luxTrain}");
        }

        Console.WriteLine();

        //7

        DateTime date = new DateTime(2023, 04, 29);
        var distinctTrainsKyivAtDate = queries.GetKyivTrainsByDate(date);
        Console.WriteLine($"Потяги, якi прибувають в Київ {date.Day}.{date.Month}.{date.Year}");
        foreach (var train in distinctTrainsKyivAtDate)
        {
            Console.WriteLine("\t"+train);
        }

        Console.WriteLine();

        //8
        Person person = queries.GetPersonById("1")!;
        var trainsByResponsible = queries.GetTrainsByResponsiblePerson(person);
        Console.WriteLine($"Потяги, за якi вiдповiдає людина {person}:");
        foreach (var train in trainsByResponsible)
        {
            Console.WriteLine("\t"+train);
        }

        Console.WriteLine();

        //9

        var trainsWithMoreThan5ReservedWagons = queries.GetTrainsWithMoreReservedWagonsThan(5);
        Console.WriteLine("Потяги з кiлькiстю плацкартних вагонiв бiльшою за 5:");
        foreach (var train in trainsWithMoreThan5ReservedWagons)
        {
            Console.WriteLine("\t"+train);
        }

        Console.WriteLine();

        //10

        var trainWithMostSeats = queries.GetTrainWithMostAmountOfSeats();
        Console.WriteLine($"Потяг з найбiльщою кiлькiстю мiсць:\n\t{trainWithMostSeats}\n");

        //11

        var trainWithMostAmountOfStops = queries.GetTrainWithMostStops();
        Console.WriteLine($"Потяг з найбiльшою кiлькiстю зупинок:\n\t{trainWithMostAmountOfStops}\n");

        //12

        var townAndAmountThroughEachStation = queries.GetTrainAmountThroughEachStation();
        Console.WriteLine("Кiлькiсть потягiв з кожного мiста");
        foreach (var townAmount in townAndAmountThroughEachStation)
        {
            Console.WriteLine($"\tМiсто: {townAmount.Item1} Кiлькiсть потягiв: {townAmount.Item2}");
        }

        Console.WriteLine();

        //13

        var inactiveTowns = queries.GetInactiveTowns();
        Console.WriteLine("Мiста, в яких потяги наразi не курсують:");
        if (!inactiveTowns.Any())
        {
            Console.WriteLine("\tТаких мiст наразi немає.");
        }

        foreach (var town in inactiveTowns)
        {
            Console.WriteLine("\t"+town);
        }

        Console.WriteLine();

        //14
        var personStartsWithMAndTrainResponsibleFor = queries.GetResponsiblePersonStartsWithM();
        Console.WriteLine("Отримаємо людей, iм`я яких починається на \"М\" та потяги, за якi вони вiдповiдають");
        foreach (var personTrain in personStartsWithMAndTrainResponsibleFor)
        {
            Console.WriteLine($"\t{personTrain.Item1} ({personTrain.Item2})\n");
        }
        Console.WriteLine();
        
        //15
        
        Train train2 = queries.GetTrainById("2");
        var townsTrainGoesThrough = queries.GetTownsByTrain(train2);
        Console.WriteLine($"Мiста, через якi проходить потяг: {train2}");
        foreach (var town in townsTrainGoesThrough)
        {
            Console.WriteLine("\t"+town);
        }
        Console.WriteLine();
    }
}