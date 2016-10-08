using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LibGit2Sharp;

namespace WindowsFormsApplication1
{
    public class GitRepositoryLogger
    {
        private const string DATE_FORMAT = "ddd dd MMM HH:mm:ss yyyy K";
        private string _log;
        private string _path;
        private readonly int _maxCommitsAmount;
        private Repository _repo;
        private DateTime _fromDate;
        private DateTime _toDate;

        public GitRepositoryLogger(string path, int maxCommitsAmount, DateTime fromDate, DateTime toDate)
        {
            _path = path;
            _maxCommitsAmount = maxCommitsAmount;
            _fromDate = fromDate;
            _toDate = toDate;
        }

        public string GetLog()
        {
            _log = "";
            _repo = new Repository(_path);
            using (_repo)
            {
                foreach (Commit c in TakeDesiredCommits())
                {
                    LogCommit(c);
                }
            }
            return _log;
        }

        private IEnumerable<Commit> TakeDesiredCommits()
        {
            int commitsNum = _repo.Commits.Count();
            int takeAmount;
            if (_maxCommitsAmount == -1)
            {
                takeAmount = commitsNum;
            }
            else
            {
                takeAmount = Math.Min(commitsNum, _maxCommitsAmount);
            }
            return _repo.Commits.Where(commit => commit.Author.When > _fromDate && commit.Author.When < _toDate).Take(takeAmount);
        }

        private void LogCommit(Commit commit)
        {
            int filesChanged = 0;
            LogFilesChangedInCommit(commit);
            LogCommitDetails(commit);
        }

        private void LogCommitDetails(Commit commit)
        {
            string commitLog = new Dictionary<string, object>()
            {
                {"Type", "Commit"},
                {"CommitId", commit.Id},
                {"Author", commit.Author.Name},
                {"time", GetDateOfCommit(commit)},
                {"IsMergeCommit",commit.ParentsCount > 1},
                {"Message", commit.MessageShort}
            }.ToSplunkLogEndLine();
            _log += commitLog;
        }

        private void LogFilesChangedInCommit(Commit commit)
        {
            foreach (var parent in commit.Parents)
            {
                var changes = _repo.Diff.Compare(parent.Tree, commit.Tree);
                foreach (TreeEntryChanges change in changes)
                {
                    string fileLog = new Dictionary<string, object>()
                    {
                        {"Type", "File"},
                        {"ComitId", commit.Id},
                        {"ChangeStatus", change.Status},
                        {"OldPath", change.OldPath},
                        {"LinesAdded", change.LinesAdded},
                        {"LinesDeleted", change.LinesDeleted},
                        {"time", GetDateOfCommit(commit)},
                        {"LinesChanged", change.LinesAdded + change.LinesDeleted}
                    }.ToSplunkLogEndLine();
                    _log += fileLog;
                }
            }
        }

        private static string GetDateOfCommit(Commit commit)
        {
            return commit.Author.When.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
        }
    }
}