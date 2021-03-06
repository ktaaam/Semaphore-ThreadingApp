﻿// MonitorSample.cs
// This example shows use of the following methods of the C# lock keyword
// and the Monitor class 
// in threads:
//      Monitor.Pulse(Object)
//      Monitor.Wait(Object)
using System;
using System.Collections.Concurrent;
using System.Threading;

public class MonitorSample
{

    public static void Main(String[] args)
    {
        int result = 0;   // Result initialized to say there is no error
        MessageBox cell = new MessageBox();


        CellProd prod = new CellProd(cell, 20);  // Use cell for storage, 
                                                 // produce 20 items
        CellCons cons = new CellCons(cell, 20);  // Use cell for storage, 
                                                 // consume 20 items
        for (int i = 1; i <= 10; i++)
        {
            Thread producer = new Thread(new ThreadStart(prod.ThreadRun));
            Thread consumer = new Thread(new ThreadStart(cons.ThreadRun));
            // Threads producer and consumer have been created, 
            // but not started at this point.

            // Start the thread, passing the number.
            try
            {
                producer.Start();
                consumer.Start();


                // threads producer and consumer have finished at this point.
            }
            catch (ThreadStateException e)
            {
                Console.WriteLine(e);  // Display text of exception
                result = 1;            // Result says there was an error
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);  // This exception means that the thread
                                       // was interrupted during a Wait
                result = 1;            // Result says there was an error
            }
            // Even though Main returns void, this provides a return code to 
            // the parent process.
            Environment.ExitCode = result;



        }
    }





    public class CellProd
    {
        MessageBox cell;         // Field to hold cell object to be used
        int quantity = 1;  // Field for how many items to produce in cell

        public CellProd(MessageBox box, int request)
        {
            cell = box;          // Pass in what cell object to be used
            quantity = request;  // Pass in how many items to produce in cell
        }
        public void ThreadRun()
        {
            for (int looper = 1; looper <= quantity; looper++)
            {
                // cell.WriteToCell(looper);  // "producing"
                cell.put(" Queuing " + looper);
            }


        }
    }

    public class CellCons
    {
        MessageBox cell;         // Field to hold cell object to be used
        int quantity = 1;  // Field for how many items to consume from cell

        public CellCons(MessageBox box, int request)
        {
            cell = box;          // Pass in what cell object to be used
            quantity = request;  // Pass in how many items to consume from cell
        }
        public void ThreadRun()
        {
            int valReturned;
            for (int looper = 1; looper <= quantity; looper++)
            {
                // Consume the result by placing it in valReturned.
                // valReturned = cell.ReadFromCell();
                cell.get();
            }

        }
    }

    public class MessageBox
    {
        int cellContents;         // Cell contents
        bool readerFlag = false;  // State flag
        private static Semaphore pool = new Semaphore(0, 5);
        private static Semaphore poolSecond = new Semaphore(5, 5);
        ConcurrentQueue<string> myQ = new ConcurrentQueue<string>();

        public void put(String message)
        {
            pool.WaitOne();
            lock (myQ)
            {
                while (myQ.Count < 4)
                {
                    if (myQ.Count < 4)
                    {
                        myQ.Enqueue(message);
                        Console.WriteLine("****************************** PUTTING" + message);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        Console.WriteLine(myQ.Count);
                        Console.WriteLine("THE QUEUE IS FULL");
                    }


                }


            }

        }
        public string get()
        {

            string message = "There is nothing in queue";
            lock (myQ)
            {
                if (myQ.Count > 0)
                {
                    if (myQ.TryDequeue(out message))
                        Console.WriteLine("-------------------------------- GETTING" + message);
                }
                else
                {
                    Console.WriteLine("^^^^^^^^^^Empty");
                }
            }
            pool.Release();

            return message;
        }
    }
}
