using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Infrastructure.DbContext;
using Infrastructure.Models;

namespace Lab2;

public class DbContextToXmlWriter
{
    public static void DbContextToXml(DbContext database, string filename = "database.xml")
    {
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        using (XmlWriter writer = XmlWriter.Create(filename, settings))
        {
            writer.WriteStartElement("DbContext");
            writer.WriteStartElement("ResponsiblePeople");
            foreach (Person person in database.ResponsiblePeople)
            {
                writer.WriteStartElement("Person");
                writer.WriteElementString("Id", person.Id!.ToString());
                writer.WriteElementString("Name", person.Name);
                writer.WriteElementString("Surname", person.Surname);
                writer.WriteElementString("Patronymic", person.Patronymic);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
            writer.WriteStartElement("Schedules");
            foreach (Schedule schedule in database.Schedules)
            {
                writer.WriteStartElement("Schedule");
                writer.WriteElementString("Id", schedule.Id.ToString());
                writer.WriteElementString("TownFromId", schedule.TownFromId.ToString());
                writer.WriteElementString("TownToId", schedule.TownToId.ToString());
                writer.WriteElementString("DateTimeOfDeparture", schedule.DateTimeOfDeparture.ToString());
                writer.WriteElementString("DateTimeOfArrival", schedule.DateTimeOfArrival.ToString());
                writer.WriteElementString("TrainId", schedule.TrainId.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
            writer.WriteStartElement("Towns");
            foreach (Town town in database.Towns)
            {
                writer.WriteStartElement("Town");
                writer.WriteElementString("Id", town.Id.ToString());
                writer.WriteElementString("Name", town.Name);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
            writer.WriteStartElement("Trains");
            foreach (Train train in database.Trains)
            {
                writer.WriteStartElement("Train");
                writer.WriteElementString("InventaryNumber", train.InventaryNumber.ToString());
                writer.WriteElementString("ResponsiblePersonId", train.ResponsiblePersonId.ToString());
                writer.WriteElementString("TrainNumber", train.TrainNumber);
                writer.WriteElementString("AmountOfWagons", train.AmountOfWagons.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            
            writer.WriteStartElement("Wagons");
            foreach (Wagon wagon in database.Wagons)
            {
                writer.WriteStartElement("Wagon");
                writer.WriteElementString("Id", wagon.Id.ToString());
                writer.WriteElementString("InventaryNumberOfTrain", wagon.InventaryNumberOfTrain.ToString());
                writer.WriteElementString("Type", wagon.Type.ToString());
                writer.WriteElementString("AmountOfSeats", wagon.AmountOfSeats.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
    
    public static void DbContextToXmlUsingSerializer(DbContext database, string filename = "databaseN.xml")
    {
        XmlSerializer formatter = new XmlSerializer(typeof(DbContext));
        using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
        using (var streamWriter = XmlWriter.Create(fs, new()
               {
                   Encoding = Encoding.UTF8,
                   Indent = true
               }))
        {
            formatter.Serialize(streamWriter, database);
        }
    }
}