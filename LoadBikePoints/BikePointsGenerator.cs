namespace LoadBikePoints
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using System.Linq;
    using Microsoft.Azure.WebJobs.Host;
    using System;

    public static class BikePointsGenerator
    {
        public static List<BikePoint> CreateBikePoints(string xmlInString, TraceWriter tw)
        {
            var bikePoints = new List<BikePoint>();

            try
            {
                var elements = XElement.Parse(xmlInString);
                var stations = elements.Elements().Where(x => x.Name == "station").ToList();
                bikePoints = (from station in stations
                              select new BikePoint
                              {
                                  Id = station.Element("id").Value,
                                  Name = station.Element("name").Value,
                                  Lat = decimal.Parse(station.Element("lat").Value),
                                  Long = decimal.Parse(station.Element("long").Value),
                                  NbBikes = int.Parse(station.Element("nbBikes").Value),
                                  TerminalName = station.Element("terminalName").Value,
                                  Installed = bool.Parse(station.Element("installed").Value),
                                  Locked = bool.Parse(station.Element("locked").Value),
                                  NbEmptyDocks = int.Parse(station.Element("nbEmptyDocks").Value),
                                  NbDocks = int.Parse(station.Element("nbDocks").Value),
                                  RowKey = station.Element("id").Value,
                                  PartitionKey = "BikePoint"
                              }).ToList();
            }
            catch (Exception error)
            {
                tw.Warning(error.Message);
            }
            return bikePoints;
        }
    }
}
