using System;
using System.Collections.Generic;

public class Room
{
    public string RoomNumber { get; set; }
    public List<Reservation> Reservations { get; set; }

    public Room(string roomNumber)
    {
        RoomNumber = roomNumber;
        Reservations = new List<Reservation>();
    }
}

public class Reservation
{
    public string User { get; set; }
    public Room Room { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public Reservation(string user, Room room, DateTime startTime, DateTime endTime)
    {
        User = user;
        Room = room;
        StartTime = startTime;
        EndTime = endTime;
    }
}

public class Booking
{
    public string BookingID { get; set; }
    public Reservation Reservation { get; set; }

    public Booking(string bookingID, Reservation reservation)
    {
        BookingID = bookingID;
        Reservation = reservation;
    }
}

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public User(string username, string password, string role)
    {
        Username = username;
        Password = password;
        Role = role;
    }
}

public class Program
{
    static List<Room> rooms = new List<Room>();
    static List<Booking> bookings = new List<Booking>();
    static List<string> logs = new List<string>();
    static List<User> users = new List<User>();
    static User loggedInUser = null;

    static void Main(string[] args)
    {
        InitializeRooms();
        InitializeUsers();
        Console.WriteLine("Welcome to the Room Reservation System");

        while (true)
        {
            if (loggedInUser == null)
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Signup");
                Console.WriteLine("3. Exit");

                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Login();
                        break;
                    case "2":
                        Signup();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Select an option:");
                Console.WriteLine("1. View Rooms");
                if (loggedInUser.Role == "Admin" || loggedInUser.Role == "Manager")
                {
                    Console.WriteLine("2. Make a Reservation");
                    Console.WriteLine("3. Cancel a Reservation");
                }
                if (loggedInUser.Role == "Admin")
                {
                    Console.WriteLine("4. Add Room");
                    Console.WriteLine("5. Remove Room");
                    Console.WriteLine("6. View Logs");
                }
                if (loggedInUser.Role == "User")
                {
                    Console.WriteLine("7. Scan QR Code");
                }
                Console.WriteLine("8. View Dashboard");
                Console.WriteLine("9. Logout");

                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        ViewRooms();
                        break;
                    case "2":
                        if (loggedInUser.Role == "Admin" || loggedInUser.Role == "Manager")
                            MakeReservation();
                        break;
                    case "3":
                        if (loggedInUser.Role == "Admin" || loggedInUser.Role == "Manager")
                            CancelReservation();
                        break;
                    case "4":
                        if (loggedInUser.Role == "Admin")
                            AddRoom();
                        break;
                    case "5":
                        if (loggedInUser.Role == "Admin")
                            RemoveRoom();
                        break;
                    case "6":
                        if (loggedInUser.Role == "Admin")
                            ViewLogs();
                        break;
                    case "7":
                        if (loggedInUser.Role == "User")
                            ScanQRCode();
                        break;
                    case "8":
                        ViewDashboard();
                        break;
                    case "9":
                        Logout();
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }

    static void InitializeRooms()
    {
        rooms.Add(new Room("101"));
        rooms.Add(new Room("102"));
    }

    static void InitializeUsers()
    {
        users.Add(new User("admin", "Admin1", "Admin"));
        users.Add(new User("manager", "MAN1", "Manager"));
    }

    static void Login()
    {
        Console.WriteLine("Enter Username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter Password:");
        string password = Console.ReadLine();

        loggedInUser = users.Find(u => u.Username == username && u.Password == password);

        if (loggedInUser != null)
        {
            Console.WriteLine($"Welcome {loggedInUser.Username}");
        }
        else
        {
            Console.WriteLine("Invalid username or password.");
        }
    }

    static void Signup()
    {
        Console.WriteLine("Enter Username:");
        string username = Console.ReadLine();
        Console.WriteLine("Enter Password:");
        string password = Console.ReadLine();
        string role = "User";  

        users.Add(new User(username, password, role));
        Console.WriteLine("Signup successful. Please login.");
    }

    static void ViewRooms()
    {
        Console.WriteLine("Available Rooms:");
        foreach (var room in rooms)
        {
            Console.WriteLine($"Room Number: {room.RoomNumber}");
        }
    }

    static void MakeReservation()
    {
        Console.WriteLine("Enter Room Number:");
        string roomNumber = Console.ReadLine();
        Room room = rooms.Find(r => r.RoomNumber == roomNumber);

        if (room != null)
        {
            Console.WriteLine("Enter Start Time (yyyy-mm-dd   hh:mm):");
            DateTime startTime = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Enter End Time (yyyy-mm-dd   hh:mm):");
            DateTime endTime = DateTime.Parse(Console.ReadLine());

            var reservation = new Reservation(loggedInUser.Username, room, startTime, endTime);
            room.Reservations.Add(reservation);

            string bookingID = Guid.NewGuid().ToString();
            var booking = new Booking(bookingID, reservation);
            bookings.Add(booking);

            Log($"Reservation made. Room: {room.RoomNumber}, Start Time: {startTime}");

            Console.WriteLine($"Reservation made successfully with Booking ID: {bookingID}");
        }
        else
        {
            Console.WriteLine("Room not found.");
        }
    }

    static void CancelReservation()
    {
        Console.WriteLine("Enter Booking ID:");
        string bookingID = Console.ReadLine();
        var booking = bookings.Find(b => b.BookingID == bookingID);

        if (booking != null)
        {
            booking.Reservation.Room.Reservations.Remove(booking.Reservation);
            bookings.Remove(booking);

            Log($"Reservation cancelled. Booking ID: {bookingID}");

            Console.WriteLine("Reservation cancelled successfully.");
        }
        else
        {
            Console.WriteLine("Booking not found.");
        }
    }

    static void AddRoom()
    {
        Console.WriteLine("Enter Room Number:");
        string roomNumber = Console.ReadLine();

        if (rooms.Exists(r => r.RoomNumber == roomNumber))
        {
            Console.WriteLine("Room already exists.");
        }
        else
        {
            rooms.Add(new Room(roomNumber));
            Log($"Room added. Room Number: {roomNumber}");
            Console.WriteLine("Room added successfully.");
        }
    }

    static void RemoveRoom()
    {
        Console.WriteLine("Enter Room Number:");
        string roomNumber = Console.ReadLine();
        var room = rooms.Find(r => r.RoomNumber == roomNumber);

        if (room != null)
        {
            rooms.Remove(room);
            Log($"Room removed. Room Number: {roomNumber}");
            Console.WriteLine("Room removed successfully.");
        }
        else
        {
            Console.WriteLine("Room not found.");
        }
    }

    static void ViewDashboard()
    {
        Console.WriteLine("User Dashboard");
        Console.WriteLine($"Username: {loggedInUser.Username}");
        Console.WriteLine("Reservations:");

        foreach (var booking in bookings)
        {
            if (booking.Reservation.User == loggedInUser.Username || loggedInUser.Role == "Admin")
            {
                var reservation = booking.Reservation;
                Console.WriteLine($"Booking ID: {booking.BookingID}, Room: {reservation.Room.RoomNumber}, From: {reservation.StartTime} To: {reservation.EndTime}");
            }
        }
    }

    static void ViewLogs()
    {
        if (loggedInUser.Role == "Admin")
        {
            Console.WriteLine("Logs:");
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }
        else
        {
            Console.WriteLine("You do not have permission to view logs.");
        }
    }

    static void ScanQRCode()
    {
        Console.WriteLine("Enter Booking ID from QR Code:");
        string bookingID = Console.ReadLine();
        var booking = bookings.Find(b => b.BookingID == bookingID);

        if (booking != null)
        {
            var reservation = booking.Reservation;
            Console.WriteLine($"Booking ID: {booking.BookingID}, Room: {reservation.Room.RoomNumber}, From: {reservation.StartTime} To: {reservation.EndTime}");
        }
        else
        {
            Console.WriteLine("Booking not found.");
        }
    }

    static void Logout()
    {
        loggedInUser = null;
        Console.WriteLine("Logged out successfully.");
    }

    static void Log(string message)
    {
        logs.Add(message);
    }
}
