using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectNoteView
    {
        public int ProjectNoteId { get; set; }
        public string NoteText { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string FileKey { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string FileContentype { get; set; }
        public DateTime DatePosted { get; set; }
        public int ProjectId { get; set; }

        public ProjectNoteView()
        {
        }

        public ProjectNoteView(ProjectNote note,string username)
        {
            this.ProjectId = note.ProjectId;
            this.ProjectNoteId = note.ProjectNoteId;
            this.NoteText = note.NoteText;
            this.DatePosted = note.DatePosted;
            this.UserId = note.UserId;
            this.FileContentype = note.FileContentype;
            this.FileKey = note.FileKey;
            this.FileName = note.FileName;
            this.FileSize = note.FileSize;
            this.Username = username;
        }
    }
}