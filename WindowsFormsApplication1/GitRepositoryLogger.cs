using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace WindowsFormsApplication1
{
    public class GitRepositoryLogger
    {
        private StringBuilder _log;
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
            _log = new StringBuilder();
            _repo = new Repository(_path);
            using (_repo)
            {
                foreach (Commit c in TakeDesiredCommits())
                {
                    LogCommit(c);
                }
            }
            return _log.ToString();
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
            LogFilesChangedInCommit(commit);
            LogCommitDetails(commit);
        }

        private void LogCommitDetails(Commit commit)
        {
            string commitLog = GetCommitDetails(commit).ToSplunkLogEndLine(commit.Author.When);
            _log.Append(commitLog);
        }

        private IDictionary<string, object> GetSharedCommitDetails(Commit commit)
        {
            return new Dictionary<string, object>()
            {
                {"CommitId", commit.Id},
                {"Author", commit.Author.Name},
                {"IsMergeCommit",commit.ParentsCount > 1},
            };
        }

        private IDictionary<string, object> GetCommitDetails(Commit commit)
        {
            return new Dictionary<string, object>()
            {
                {"Type", "Commit"},
                {"Message", commit.MessageShort}
            }.Merge(GetSharedCommitDetails(commit));
        }

        private void LogFilesChangedInCommit(Commit commit)
        {
            foreach (var parent in commit.Parents)
            {
                var fileChanges = _repo.Diff.Compare(parent.Tree, commit.Tree);
                foreach (TreeEntryChanges fileChange in fileChanges)
                {
                    string fileLog = GetFileChangedDetails(commit, fileChange).ToSplunkLogEndLine(commit.Author.When);
                    _log.Append(fileLog);
                }
            }
        }

        private IDictionary<string, object> GetFileChangedDetails(Commit commit, TreeEntryChanges change)
        {
            return new Dictionary<string, object>()
            {
                {"Type", "File"},
                {"ChangeStatus", change.Status},
                {"OldPath", change.OldPath},
                {"LinesAdded", change.LinesAdded},
                {"LinesDeleted", change.LinesDeleted},
                {"LinesChanged", change.LinesAdded + change.LinesDeleted}
            }.Merge(GetSharedCommitDetails(commit));
        }
    }
}