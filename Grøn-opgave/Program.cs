using System;
using MySql.Data.MySqlClient;

namespace Grøn_opgave
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connString = "server = localhost; database = notes; user = root; password =;";

            using (MySqlConnection conn = new MySqlConnection(connString))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Forbundet til databasen.");

                    while (true)
                    {
                        DisplayMenu();

                        ConsoleKeyInfo pressedKey = Console.ReadKey();
                        Console.Clear();
                        switch (pressedKey.KeyChar)
                        {
                            case '1':
                                CreateNote(conn);
                                break;
                            case '2':
                                ViewNotes(conn);
                                break;
                            case '3':
                                DeleteNote(conn);
                                break;
                            case '4':
                                ExitProgram();
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fejl: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                    Console.WriteLine("Forbindelsen til databasen er lukket.");
                }
            }
            Console.ReadKey();
        }

        static void DisplayMenu()
        {
            Console.Clear();

            Console.WriteLine("Menu:");
            Console.WriteLine("1. Opret note");
            Console.WriteLine("2. Vis noter");
            Console.WriteLine("3. Slet note");
            Console.WriteLine("4. Afslut");
            Console.Write("Vælg en mulighed: ");
        }

        static void CreateNote(MySqlConnection conn)
        {
            Console.Write("Indtast notens titel: ");
            string title = Console.ReadLine();
            Console.Write("Indtast notens indhold: ");
            string content = Console.ReadLine();

            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(content))
            {
                string query = "INSERT INTO notes (title, content) VALUES (@title, @content)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@content", content);
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        Console.WriteLine("Note oprettet succesfuldt.");
                    }
                    else
                    {
                        Console.WriteLine("Fejl ved oprettelse af note.");
                    }
                }
            }
            else
            {
                Console.Write("\nTitel og indhold kan ikke være tomme. Tryk på en tast for at fortsætte...");
                Console.ReadKey();
            }
        }

        static void ViewNotes(MySqlConnection conn)
        {
            string query = "SELECT id, title, content FROM notes";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Noter:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["id"]}, Titel: {reader["title"]}, Indhold: {reader["content"]}");
                    }
                }
                Console.WriteLine("\nTryk på en tast for at fortsætte...");
                Console.ReadKey();
            }
        }

        static void DeleteNote(MySqlConnection conn)
        {
            Console.Write("Indtast ID på den note, der skal slettes: ");
            Console.ForegroundColor = ConsoleColor.Red;
            string id = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.White;
            
            string query = "DELETE FROM notes WHERE id = @id";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    Console.WriteLine("Note slettet succesfuldt.");
                }
                else
                {
                    Console.WriteLine("Fejl ved sletning af note. Tjek om ID'et er korrekt.");
                }
            }
        }

        static void ExitProgram()
        {
            Console.WriteLine("Afslutter programmet...");
            Environment.Exit(0);
        }
    }
}
