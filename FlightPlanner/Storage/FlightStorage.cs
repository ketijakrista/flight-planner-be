using System;
using System.Collections.Generic;
using System.Linq;
using FlightPlanner.Models;
using MoreLinq.Extensions;

namespace FlightPlanner.Storage
{
    public static class FlightStorage
    {
        private static List<Flight> _flights = new List<Flight>();
        private static int _id = 0;
        private static readonly object _lock = new object();


        public static Flight AddFlight(AddFlightRequest request)
        {
            lock (_lock)
            {
                var flight = new Flight
                {
                    Id = ++_id,
                    ArrivalTime = request.ArrivalTime,
                    Carrier = request.Carrier,
                    DepartureTime = request.DepartureTime,
                    From = request.From,
                    To = request.To
                };
                _flights.Add(flight);
            
                return flight;
            }
        }

        public static Flight GetFlightById(int id)
        {
            lock (_lock)
            {
                return _flights.SingleOrDefault(f => f.Id == id);
            }
        }

        public static void DeleteFlight(int id)
        {

            var flight = GetFlightById(id);
            lock (_lock)
            {
                if (flight != null)
                    _flights.Remove(flight);
            }
        }

        public static bool IsValid(AddFlightRequest request)
        {
            if (request == null)
                return false;
            if (string.IsNullOrEmpty(request.ArrivalTime) || string.IsNullOrEmpty(request.Carrier) ||
                string.IsNullOrEmpty(request.DepartureTime))
                return false;
            if (request.From == null || request.To == null)
                return false;
            if (string.IsNullOrEmpty(request.From.AirportName) || string.IsNullOrEmpty(request.From.City) ||
                string.IsNullOrEmpty(request.From.Country))
                return false;
            if (string.IsNullOrEmpty(request.To.AirportName) || string.IsNullOrEmpty(request.To.City) ||
                string.IsNullOrEmpty(request.To.Country))
                return false;
            if (string.Equals(request.From.AirportName.Trim(), request.To.AirportName.Trim(),
                    StringComparison.CurrentCultureIgnoreCase))
                return false;
            var arrivalDate = DateTime.Parse(request.ArrivalTime);
            var departureDate = DateTime.Parse(request.DepartureTime);
            if (arrivalDate <= departureDate)
                return false;

            return true;
        }

        public static bool Exists(AddFlightRequest request)
        {
            lock (_lock)
            {
                return _flights.Any(f => f.ArrivalTime == request.ArrivalTime && f.DepartureTime == request.DepartureTime &&
                                         string.Equals(f.From.AirportName, request.From.AirportName,
                                             StringComparison.CurrentCultureIgnoreCase) && string.Equals(f.To.AirportName,
                                             request.To.AirportName, StringComparison.CurrentCultureIgnoreCase));
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _flights.Clear();
                _id = 0;
            }
        }


        public static List<Airport> FindAirport(string search)
        {
            lock (_lock)
            {
                search = (search ?? "").ToLower().Trim();
                var airportsFrom = _flights
                    .Where(f => f.From.AirportName.ToLower().Contains(search) || f.From.City.ToLower().Contains(search) ||
                                f.From.Country.ToLower().Contains(search)).Select(f => f.From).ToList();
                var airportsTo = _flights
                    .Where(f => f.From.AirportName.ToLower().Contains(search) || f.From.City.ToLower().Contains(search) ||
                                f.From.Country.ToLower().Contains(search)).Select(f => f.From).ToList();
                var airports = airportsFrom;
                airports.AddRange(airportsTo);
                airports = airports.DistinctBy(a => a.AirportName).ToList();

                return airports;
            }
        }

        public static SearchResult FindFlights(SearchFlightRequest request)
        {
            lock (_lock)
            {
                var result = new SearchResult();
                var flights = _flights.Where(f =>
                    f.DepartureTime.Contains(request.DepartureDate) &&
                    f.From.AirportName.ToLower().Contains(request.From.ToLower().Trim()) &&
                    f.To.AirportName.ToLower().Contains(request.To.ToLower().Trim())).ToList();
                result.Items = flights;
                result.TotalItems = flights.Count;
                return result;
            }
        }


        public static bool SearchRequestIsValid(SearchFlightRequest request)
        {
            if (request == null)
                return false;
            if (string.IsNullOrEmpty(request.DepartureDate))
                return false;
            if (string.IsNullOrEmpty(request.From))
                return false;
            if (string.IsNullOrEmpty(request.To))
                return false;
            if (request.From.Trim().Equals(request.To.Trim(), StringComparison.CurrentCultureIgnoreCase))
                return false;
            return true;
        }
    }
}
