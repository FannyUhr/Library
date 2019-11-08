using DataAccess;
using DataInterface;
using System;

namespace Library
{
    class Program
    {
        static void Main(string[] args)
        {
            IAisleManager aisleManager = new AisleManager();
            aisleManager.AddAisle(1); //A-F
            aisleManager.AddAisle(2); //G-L
            aisleManager.AddAisle(3); //M-R
            aisleManager.AddAisle(4); //S-Y
            aisleManager.AddAisle(5); //Z-Ö
            
            IShelfManager shelfManager = new ShelfManager();
            var shelf1 = shelfManager.AddShelf(1, 1); //A-B
            shelfManager.AddShelf(2, 1); //C-D
            shelfManager.AddShelf(3, 1); //E-F
            var shelf4 = shelfManager.AddShelf(1, 2); //G-H
            var shelf5 = shelfManager.AddShelf(2, 2); //I-J
            var shelf6 = shelfManager.AddShelf(3, 2); //K-L
            var shelf7 = shelfManager.AddShelf(1, 3); //M-N
            shelfManager.AddShelf(2, 3); //O-P
            shelfManager.AddShelf(3, 3); //Q-R
            var shelf10 = shelfManager.AddShelf(1, 4); //S-T
            var shelf11 = shelfManager.AddShelf(2, 4); //U-VW
            shelfManager.AddShelf(3, 4); //X-Y
            shelfManager.AddShelf(1, 5); //Z-Å
            shelfManager.AddShelf(2, 5); //Ä-Ö

            IBorrowManager borrowManager = new BorrowManager();
            var borrow1 = borrowManager.AddBorrow(000001, "Circe", new DateTime(2019 - 11 - 01), new DateTime(2019 - 12 - 30));
            var borrow2 = borrowManager.AddBorrow(000002, "Flights", new DateTime(2019 - 10 - 30), new DateTime(2019 - 11 - 29));

            IBookManager bookManager = new BookManager();
            bookManager.AddBook("9781526610140", "Circe", "Madeline Miller", 2019, 199, 4, shelf7, true, borrow1);
            bookManager.AddBook("9780525534204", "Flights", "Olga Tokarczuk", 2019, 120, 5, shelf10, true, borrow1);
            bookManager.AddBook("9781782118640", "How to Stop Time", "Matt Haig", 2017, 80, 3, shelf4, false, new Borrow());
            bookManager.AddBook("9789129690835", "Trollkarlen från Övärälden", "Ursula K. Le Guin", 2014, 60, 1, shelf6, false, new Borrow());
            bookManager.AddBook("9780356508191", "The Fifth Season", "N. K. Jemisin", 2016, 70, 3, shelf5, true, new Borrow());
            bookManager.AddBook("9789100170851", "De tre följeslagarna", "Stephen King", 2017, 80, 4, shelf6, true, new Borrow());
            var bookNeverwhere1 = bookManager.AddBook("9780747266686", "Neverwhere", "Neil Gaiman", 2013, 50, 1, shelf4, false, new Borrow());
            var bookNeverwhere2 = bookManager.AddBook("9780747266686", "Neverwhere", "Neil Gaiman", 2015, 50, 4, shelf4, false, new Borrow());
            bookManager.AddBook("9789188945532", "Dödsviskaren", "Lovisa Wistrand", 2019, 60, 4, shelf11, false, new Borrow());
            bookManager.AddBook("9781409150763", "King's Cage", "Victoria Aveyard", 2017, 70, 3, shelf1, true, new Borrow());
            bookManager.AddBook("9780099740919", "Handmaid's Tale", "Margaret Atwood", 2003, 40, 2, shelf1, true, new Borrow());

            ICustomerManager customerManager = new CustomerManager();
            customerManager.AddCustomer(000001, "Fanny Uhr", "1996-07-22", "Peppargatan 13", 0, null, false);
            customerManager.AddCustomer(000002, "Patrik Palmér", "1995-05-30", "Peppargatan 13", 0, null, false);
            customerManager.AddCustomer(000003, "Leif Eriksson", "1966-04-18", "Logvägen 16", 0, null, false);
            customerManager.AddCustomer(000004, "Greta Larsson", "1953-09-14", "Storgatan 14", 60, null, false);
            var guardian1 = customerManager.AddCustomer(000005, "Peter Andersson", "1973-07-14", "Linfrögatan 22", 0, null, true);
            customerManager.AddCustomer(000006, "Maja Bergman", "2000-07-30", "Drottninggatan 29B", 0, null, false);
            customerManager.AddCustomer(000007, "Lisa Norén", "1983-04-24", "Bergslagsgatan 13", 0, null, false);
            customerManager.AddCustomer(000008, "Pontus Andersson", "2008-02-16", "Linfrögatan 22", 0, guardian1, false);
            var guardian2 = customerManager.AddCustomer(000009, "Sara Magnusson", "1987-12-03", "Lägervägen 12", 0, null, true);
            customerManager.AddCustomer(000010, "Tindra Magnusson", "2010-03-01", "Lägervägen 12", 0, guardian2, false);
            customerManager.AddCustomer(000011, "Siri Larsson", "1991-06-30", "Paprikagatan 24", 30, null, false);
            customerManager.AddCustomer(000012, "Richard Svensson", "1994-11-15", "Anders Wedbergsgatan 3", 0, null, false);
            customerManager.AddCustomer(000013, "Ingrid Malm", "1954-01-03", "Storsjögatan 14B", 0, null, false);
            customerManager.AddCustomer(000014, "Hans Hammar", "1952-04-30", "Malmgatan 4C", 0, null, false);
            customerManager.AddCustomer(000015, "Lovisa Carlsson", "1993-05-06", "Stenbäcksvägen 24", 0, null, false);
            var guardian3 = customerManager.AddCustomer(000016, "Fredrik Malmberg", "1982-02-16", "Bergslagsgatan 11", 0, null, true);
            customerManager.AddCustomer(000017, "Emil Malmberg", "2011-04-17", "Bergslagsgatan 11", 0, guardian3, false);
            customerManager.AddCustomer(000018, "Birgitta Lindén", "1965-08-22", "Hellmansgatan 26", 0, null, false);
            customerManager.AddCustomer(000019, "Anders Lindén", "1963-09-28", "Hellmansgatan 26", 0, null, false);
            customerManager.AddCustomer(000020, "Maja Svensson", "1996-10-24", "Kamomillvägen 13", 0, null, false);
           
            Console.WriteLine("Klart");
            Console.ReadLine();
        }
    }
}
