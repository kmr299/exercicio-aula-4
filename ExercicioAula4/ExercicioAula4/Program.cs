using System;
using Dapper;
using System.IO;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ExercicioAula4
{

    class Program
    {
        static void Main(string[] args)
        {
            var t = MainAsync();
            t.Wait();
        }

        private static string _databasePath = string.Format("Data Source={0};Version=3;", Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "database.db"));

        static async Task MainAsync()
        {
            Console.WriteLine("Listagem de músicas");
            Console.WriteLine("====================================================");

            while (true)
            {
                Console.WriteLine("1 - Listar artistas");
                Console.WriteLine("2 - Listar artistas e albuns");
                Console.WriteLine("3 - Listar gêneros");
                Console.WriteLine("4 - Listar músicas");

                Console.WriteLine("\nDigite uma opção:");

                if (int.TryParse(Console.ReadLine(), out int opcao))
                {
                    using (var conn = new SQLiteConnection(_databasePath))
                    {
                        Console.Clear();
                        switch (opcao)
                        {
                            case 1:
                                var result = await conn.QueryAsync<dynamic>("SELECT ArtistId as Id, Name as Name FROM artists");

                                foreach (dynamic i in result)
                                    Console.WriteLine("| {0} | {1} | {2}", i.Id, i.Name, i.Teste);

                                break;
                            case 2:
                                await ImprimirArtistasEAlbunsAsync();
                                break;
                        }
                    }

                    Console.WriteLine("Pressione qualquer botão para continuar");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static async Task ImprimirArtistasEAlbunsAsync()
        {
            using (var conn = new SQLiteConnection(_databasePath))
            {
                var artistas = await conn.QueryAsync<dynamic>("SELECT ArtistId as Id, Name as Nome FROM Artists");

                var listaTarefas =
                    new Dictionary<dynamic, Task<IEnumerable<dynamic>>>();

                foreach (dynamic artista in artistas)
                {
                    int id = int.TryParse(artista.Id.ToString(), out int res) ? res : 0;
                    var tarefa = BuscarAlbunsAsync(id);
                    listaTarefas.Add(artista.Id, tarefa);
                }

                foreach (dynamic artista in artistas)
                    artista.Albuns = await listaTarefas[artista.Id];

                Parallel.ForEach(artistas, artista =>
                {
                    Console.WriteLine("{0} | {1}", artista.Id, artista.Nome);
                    Parallel.ForEach((IEnumerable<dynamic>)artista.Albuns, (album) =>
                    {
                        Console.WriteLine($"     {album.Id} | {album.Titulo}");
                    });
                });
            }
        }

        private static async Task<IEnumerable<dynamic>> BuscarAlbunsAsync(int artistaId)
        {
            using (var conn = new SQLiteConnection(_databasePath))
            {
                return await conn.QueryAsync<dynamic>(
                    "SELECT " +
                    "   AlbumId as Id, " +
                    "   Title as Titulo, " +
                    "   ArtistId as artistaId " +
                    "FROM Albums " +
                    "WHERE ArtistId = @artistaId", new { artistaId });
            }
        }
    }
}
