import csv
from faker import Faker
import random
from datetime import datetime
from enum import Enum

class ClassType(Enum):
    ECONOMY = 1
    BUSINESS = 2
    FIRST_CLASS = 3

class Status(Enum):
    BOOKED = 1
    CANCELLED = 2
    COMPLETED = 3
    

# Initialize a Faker generator
fake = Faker()

# Define a list to hold our fake flight and booking data
flights = []
bookings = []
passengers = []
# Generate fake data
num_flights = 100
num_passengers = 500
num_bookings = 200

# Generating flight data
for i in range(num_flights):
    departure_date = fake.date_between(start_date="+1d", end_date="+1y")
    flight = {
        "Id": i + 1,
        "DepartureCountry": fake.country(),
        "DestinationCountry": fake.country(),
        "DepartureDate": departure_date.strftime("%Y-%m-%d"),
        "DepartureAirport": fake.city(),
        "ArrivalAirport": fake.city(),
        "PriceEconomy": round(random.uniform(100, 1000), 2),
        "PriceBusiness": round(random.uniform(1000, 2000), 2),
        "PriceFirstClass": round(random.uniform(2000, 5000), 2),
        "AvailableSeatsEconomy": random.randint(10, 50),
        "AvailableSeatsBusiness": random.randint(5, 20),
        "AvailableSeatsFirstClass": random.randint(1, 10),
    }
    flights.append(flight)

# Generate fake passengers
for _ in range(num_passengers):
    passenger = {
        "ID": str(fake.uuid4()),
        "Name": fake.name(),
    }
    passengers.append(passenger)
    
    
# Generating booking data
for _ in range(num_bookings):
    flight = random.choice(flights)
    booking = {
        "Id": str(fake.uuid4()),
        "FlightId": random.choice(flights)["Id"],
        "ClassType": random.choice(list(ClassType)).name,
        "PassengerId": random.choice(passengers)["ID"],
        "Status": random.choice(list(Status)).name
    }
    bookings.append(booking)

# Save to CSV
def save_to_csv(data, fieldnames, filename):
    with open(filename, 'w', newline='') as csvfile:
        writer = csv.DictWriter(csvfile, fieldnames=fieldnames)
        writer.writeheader()
        writer.writerows(data)

# Field names and file names
flight_fields = [
    "Id", "DepartureCountry", "DestinationCountry",
    "DepartureDate", "DepartureAirport", "ArrivalAirport",
    "PriceEconomy", "PriceBusiness", "PriceFirstClass",
    "AvailableSeatsEconomy", "AvailableSeatsBusiness", "AvailableSeatsFirstClass"
]
booking_fields = ["Id", "PassengerId", "FlightId", "ClassType", "Status"]

# Writing to csv files
save_to_csv(flights, flight_fields, 'flights.csv')
save_to_csv(passengers, ["ID", "Name"], "passengers.csv")
save_to_csv(bookings, ["Id", "FlightId", "ClassType", "PassengerId", "Status"], "bookings.csv")

