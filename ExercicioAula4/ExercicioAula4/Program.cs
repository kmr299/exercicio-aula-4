﻿using System;
using Dapper;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;

namespace ExercicioAula4
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Listagem de músicas");
            Console.WriteLine("====================================================");

            while (true)
            {
                Console.WriteLine("1 - Listar artistas");
                Console.WriteLine("2 - Listar albuns");
                Console.WriteLine("3 - Listar gêneros");
                Console.WriteLine("4 - Listar músicas");


                Console.WriteLine("\nDigite uma opção:");

                if (int.TryParse(Console.ReadLine(), out int opcao))
                {

                    var databasePath = string.Format("Data Source={0};Version=3;", Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "database.db"));
                    
                    using (var conn = new SQLiteConnection(databasePath))
                    {
                        switch (opcao)
                        {
                            case 1:

                                Console.Clear();

                                var result = conn.Query<dynamic>("SELECT ArtistId as Id, Name as Name FROM artists");

                                foreach(dynamic i in result)
                                {
                                    Console.WriteLine("| {0} | {1} |", i.Id, i.Name);
                                }


                                break;
                        }
                    }

                    Console.WriteLine("Pressione qualquer botão para continuar");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }
    }
}