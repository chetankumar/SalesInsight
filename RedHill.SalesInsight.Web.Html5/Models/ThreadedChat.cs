using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.Models.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ThreadedChat
    {
        public int? ProjectId { get; set; }
        public long? QuoteId { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatConversationId { get; set; }

        public List<SelectListItem> Projects { get; set; }
        public List<SelectListItem> Quotes { get; set; }

        public List<ChatSubscriber> ChatSubscribers { get; set; }

        //public ThreadedChat(Guid userId, int? projectId = null, long? quoteId = null)
        //{
        //    this.UserId = userId;
        //    this.ProjectId = projectId;
        //    this.QuoteId = quoteId;
        //    this.Projects = new List<SelectListItem>();
        //    this.Quotes = new List<SelectListItem>();
        //    this.ChatConversationId = SIDAL.GetChatConversationId(projectId.GetValueOrDefault(), quoteId.GetValueOrDefault());
        //}

        public void LoadProjects()
        {
            var projects = SIDAL.GetProjects(this.UserId);
            foreach (var project in projects)
            {
                this.Projects.Add(new SelectListItem { Value = project.ProjectId.ToString(), Text = project.Name });
            }
        }

        public void LoadQuotes()
        {
            var quotes = SIDAL.GetQuotationsForProject(this.ProjectId.GetValueOrDefault());
            foreach (var quote in quotes)
            {
                this.Projects.Add(new SelectListItem { Value = quote.Id.ToString(), Text = quote.Id.ToString() });
            }
        }

        /// <summary>
        /// Creates a new instance of ThreadedChat with all the fields initialized and gets a new or existing ChatConversationId based on the Project and Quote combination.
        /// </summary>
        /// <param name="userId">The user who is initializing the chat</param>
        /// <param name="projectId"></param>
        /// <param name="quoteId"></param>
        /// <returns></returns>
        public static ThreadedChat Create(Guid userId, int projectId, long? quoteId)
        {
            ThreadedChat threadedChat = new ThreadedChat();
            threadedChat.UserId = userId;
            threadedChat.ProjectId = projectId;
            threadedChat.QuoteId = quoteId;
            threadedChat.Projects = new List<SelectListItem>();
            threadedChat.Quotes = new List<SelectListItem>();
            if (projectId > 0)
            {
                threadedChat.ChatConversationId = SIDAL.GetChatConversationId(projectId, quoteId, userId);
            }
            threadedChat.ChatSubscribers = new List<ChatSubscriber>();

            return threadedChat;
        }
    }
}