using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
namespace ConsoleApplication5
{
    class Program
    {
        private static Repository _repo;

        static void Main(string[] args)
        {
            using (_repo = new Repository(@"C:\Users\tom\Documents\rafsodeld35"))
            {
                Tree prevTree = null;
                var RFC2822Format = "ddd dd MMM HH:mm:ss yyyy K";
                foreach (Commit c in _repo.Commits.Take(15))
                {
                    Console.WriteLine(string.Format("commit {0}", c.Id));

                    foreach (var parent in c.Parents)
                    {
                        int changedFilesNum = 0;
                        var changes = _repo.Diff.Compare(parent.Tree, c.Tree);
                        foreach (TreeEntryChanges change in changes)
                        {
                            Console.WriteLine($"Change: {change.Status} {change.OldPath} {change.LinesAdded} {change.LinesDeleted}");
                            changedFilesNum++;
                        }
                        Console.WriteLine($"Changes: {changedFilesNum}");
                    }
                    Console.WriteLine(string.Format("Author: {0} <{1}>", c.Author.Name, c.Author.Email));
                    Console.WriteLine("Date:   {0}", c.Author.When.ToString(RFC2822Format, CultureInfo.InvariantCulture));
                    Console.WriteLine();
                    Console.WriteLine(c.Message);
                    Console.WriteLine();
                    Console.ReadLine();
                }
            }
        }
    }
}
