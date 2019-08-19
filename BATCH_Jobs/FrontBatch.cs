using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsApplication1;
using System.Threading;

using WindowsApplication1.Action;

namespace BATCH_Jobs
{
    class FGAFrontBATCH
    {

        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Importation FACTSET en cours ");
            //Thread oThread = new Thread(new ThreadStart(ConsoleSpiner.Go));
            //oThread.Start();

            //BaseActionImportation objet = new BaseActionImportation();
            //objet.batchImportation(args);

            // Le traitement est terminé : arreter proprement la gauge
            //ConsoleSpiner.go = false;
            //oThread.Join();
            //Console.WriteLine();
            //Console.WriteLine("Importation FACTSET terminée pour la date " + objet.datee);

            //Console.ReadKey();
        }
    }

    /// <summary>
    /// Utilitaire pour la gauge
    /// </summary>
    public static class ConsoleSpiner
    {
        public static bool go;
        static int counter;
        static ConsoleSpiner()
        {
            counter = 0;
            go = true;
        }
        public static void Go()
        {
            Console.Write("Merci de patienter quelques instants: ");
            while (go)
            {
                Turn();
                Thread.Sleep(100);
            }
            Console.WriteLine();
        }

        public static void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("-"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }

    }
}
