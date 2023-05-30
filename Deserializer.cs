using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Infrastructure.Interfaces;
using Infrastructure.Models;

namespace Lab2;

public static class Deserializer
{
    public static Train? ToTrain(this XElement element)
    {
        return (Train?)Deserialize<Train>(element);
        /*XElement? inventaryNumberElement = element.Element("InventaryNumber");
        XElement? responsiblePersonIdElement = element.Element("ResponsiblePersonId");
        XElement? trainNumberElement = element.Element("TrainNumber");
        XElement? amountOfWagonsElement = element.Element("AmountOfWagons");

        if (inventaryNumberElement is null || !int.TryParse(inventaryNumberElement.Value, out var inventaryNumber) ||
            responsiblePersonIdElement is null ||
            !int.TryParse(responsiblePersonIdElement.Value, out var responsiblePersonId) ||
            trainNumberElement is null ||
            amountOfWagonsElement is null || !int.TryParse(amountOfWagonsElement.Value, out var amountOfWagons))
            return null;
        return new Train(inventaryNumber, responsiblePersonId, trainNumberElement.Value, amountOfWagons);*/
    }
    
    public static Town? ToTown(this XElement element)
    {
        return (Town?)Deserialize<Town>(element);
        /*XElement? idElement = element.Element("Id");
        XElement? nameElement = element.Element("Name");

        if (idElement is null || !int.TryParse(idElement.Value, out var id) ||
            nameElement is null)
            return null;
        return new Town(id, nameElement.Value);*/
    }
    
    public static Person? ToPerson(this XElement element)
    {
        return (Person?)Deserialize<Person>(element);
        /*XElement? idElement = element.Element("Id");
        XElement? nameElement = element.Element("Name");
        XElement? surnameElement = element.Element("Surname");
        XElement? patronymicElement = element.Element("Patronymic");

        if (idElement is null || !int.TryParse(idElement.Value, out var id) ||
            nameElement is null || surnameElement is null)
            return null;
        return new Person(id, nameElement.Value, surnameElement.Value, patronymicElement?.Value);*/
    }

    public static Wagon? ToWagon(this XElement element)
    {
        return (Wagon?)Deserialize<Wagon>(element);
        /*XElement? idElement = element.Element("Id");
        XElement? inventaryNumberElement = element.Element("InventaryNumberOfTrain");
        XElement? typeElement = element.Element("Type");
        XElement? amountOfSeatsElement = element.Element("AmountOfSeats");
        
        if (idElement is null || !int.TryParse(idElement.Value, out var id) ||
            inventaryNumberElement is null || !int.TryParse(inventaryNumberElement.Value, out var inventaryNumberOfTrain)
            || typeElement is null || !Enum.TryParse(typeElement.Value, out WagonType type) || amountOfSeatsElement is null
            || !int.TryParse(amountOfSeatsElement.Value, out var amountOfSeats))
            return null;
        return new Wagon(id, inventaryNumberOfTrain, type, amountOfSeats);*/
    }

    public static Schedule? ToSchedule(this XElement element)
    {
        return (Schedule?)Deserialize<Schedule>(element);
        /*XElement? idElement = element.Element("Id");
        XElement? townFromIdElement = element.Element("TownFromId");
        XElement? townToIdElement = element.Element("TownToId");
        XElement? dateTimeOfDepartureElement = element.Element("DateTimeOfDeparture");
        XElement? dateTimeOfArrivalElement = element.Element("DateTimeOfArrival");
        XElement? trainIdElement = element.Element("TrainId");

        if (idElement is null || !int.TryParse(idElement.Value, out var id) ||
            townFromIdElement is null || !int.TryParse(townFromIdElement.Value, out var townFromId)
            || townToIdElement is null || !int.TryParse(townToIdElement.Value, out var townToId) ||
            dateTimeOfDepartureElement is null ||
            !DateTime.TryParse(dateTimeOfDepartureElement.Value, out DateTime dateTimeOfDeparture) ||
            dateTimeOfArrivalElement is null ||
            !DateTime.TryParse(dateTimeOfArrivalElement.Value, out DateTime dateTimeOfArrival) ||
            trainIdElement is null ||
            !int.TryParse(trainIdElement.Value, out var trainId))
            return null;
        return new Schedule(id, townFromId, townToId, dateTimeOfDeparture, dateTimeOfArrival, trainId);*/
    }
    
    private static IDbModel? Deserialize<T>(XElement? node) where T : IDbModel
    {
        if (node == null)
            return null;

        var serializer = new XmlSerializer(typeof(T));
        return (T?)serializer.Deserialize(node.CreateReader());
    }
}