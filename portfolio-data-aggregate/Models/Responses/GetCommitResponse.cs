using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Models.Responses
{
    public class GetCommitResponse
    {
        public string sha { get; set; }
        public string node_id { get; set; }
        public Commit commit { get; set; }
        public string url { get; set; }
        public string html_url { get; set; }
        public string comments_url { get; set; }
        public User author { get; set; }
        public User committer { get; set; }
        public Parent[] parents { get; set; }

        public class Commit
        {
            public Person author { get; set; }
            public Person committer { get; set; }
            public string message { get; set; }
            public Tree tree { get; set; }
            public string url { get; set; }
            public int comment_count { get; set; }
            public Verification verification { get; set; }
        }

        public class Person
        {
            public string name { get; set; }
            public string email { get; set; }
            public DateTime date { get; set; }
        }


        public class Tree
        {
            public string sha { get; set; }
            public string url { get; set; }
        }

        public class Verification
        {
            public bool verified { get; set; }
            public string reason { get; set; }
            public object signature { get; set; }
            public object payload { get; set; }
        }

        public class User
        {
            public string login { get; set; }
            public int id { get; set; }
            public string node_id { get; set; }
            public string avatar_url { get; set; }
            public string gravatar_id { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string followers_url { get; set; }
            public string following_url { get; set; }
            public string gists_url { get; set; }
            public string starred_url { get; set; }
            public string subscriptions_url { get; set; }
            public string organizations_url { get; set; }
            public string repos_url { get; set; }
            public string events_url { get; set; }
            public string received_events_url { get; set; }
            public string type { get; set; }
            public bool site_admin { get; set; }
        }

        public class Parent
        {
            public string sha { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
        }

    }
}
