using Microsoft.Bot.Builder.Ai.LUIS;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ConferenceBot.Model
{
    public interface ISessionService
    {
        List<Session> GetSessions(string[] topics, string[] speakerNames, DateTimeSpec[] times);
        List<string> GetSpeakerNames(string speakerInput);
        List<Session> GetSessions(string speakerName);
    }
    public class SessionService : ISessionService
    {
        public static string GetJsonString()
        {
            // Sessions are real, times and days are made up for demo purposes
            return @"[{'Title':'Keynote','Speaker':'Scott Hunter','Day':'Thursday','Time':'8:30','Room':'Room 1'},{'Title':'Welcome Session','Speaker':'Jeffrey Fritz','Day':'Wednesday','Time':'8:30','Room':'Room 1'},{'Title':'Aurelia: Blazing Fast Web Apps','Speaker':'Ashley Grant','Day':'Wednesday','Time':'10:00','Room':'Room 1'},{'Title':'AI for the Masses: Democratizing Access to AI with Microsoft Cognitive Services','Speaker':'Stephen Bohlen','Day':'Wednesday','Time':'11:00','Room':'Room 1'},{'Title':'Zen and the Art of Asynchronous JavaScript','Speaker':'Ashley Grant','Day':'Wednesday','Time':'13:00','Room':'Room 1'},{'Title':'Improving code quality with Static Analyzers','Speaker':'Jim Wooley','Day':'Wednesday','Time':'14:00','Room':'Room 1'},{'Title':'Getting pushy with SignalR and Reactive Extensions','Speaker':'Jim Wooley','Day':'Wednesday','Time':'15:00','Room':'Room 1'},{'Title':'How Microsoft Does DevOps','Speaker':'Paul Hacker','Day':'Wednesday','Time':'16:00','Room':'Room 1'},{'Title':'Raising the Value of Your Unit Tests','Speaker':'Richard Taylor','Day':'Wednesday','Time':'17:00','Room':'Room 1'},{'Title':'Patterns for Building a Chatbot that doesn’t Suck','Speaker':'Stephen Bohlen','Day':'Thursday','Time':'10:00','Room':'Room 1'},{'Title':'Developing ASP.NET Core 2.1 Web APIs for your Web and Mobile Apps ','Speaker':'Chris Woodruff','Day':'Thursday','Time':'11:00','Room':'Room 1'},{'Title':'Learn to Love Lambdas (and LINQ, Too!)','Speaker':'Jeremy Clark','Day':'Thursday','Time':'13:00','Room':'Room 1'},{'Title':'Clean Code: Homicidal Maniacs Read Code, Too!','Speaker':'Jeremy Clark','Day':'Thursday','Time':'14:00','Room':'Room 1'},{'Title':'Using WebUSB to create modern interfaces for hardware and robotics','Speaker':'Suz Hinton','Day':'Thursday','Time':'15:00','Room':'Room 1'},{'Title':'Who Needs Visual Studio? A look at .NET Core on Linux','Speaker':'Chris Gomez','Day':'Thursday','Time':'16:00','Room':'Room 1'},{'Title':'Living on the Edge with Web Assembly (Blazor and C#)','Speaker':'Todd Snyder','Day':'Thursday','Time':'17:00','Room':'Room 1'},{'Title':'Confessions of an Imposter: You\'re not alone','Speaker':'Ivana Veliskova','Day':'Friday','Time':'10:00','Room':'Room 1'},{'Title':'DevOps with Kubernetes and Helm','Speaker':'Jessica Deen','Day':'Friday','Time':'11:00','Room':'Room 1'},{'Title':'Say Yes to NoSQL for the .NET SQL Developer','Speaker':'Jeremy Likness','Day':'Friday','Time':'13:00','Room':'Room 1'},{'Title':'Twitch Bots and Live Streaming','Speaker':'Brendan Enrick','Day':'Friday','Time':'14:00','Room':'Room 1'},{'Title':'Making Use of New C# Features','Speaker':'Brendan Enrick','Day':'Friday','Time':'15:00','Room':'Room 1'},{'Title':'Mocking .NET Without Hurting Its Feelings','Speaker':'John Wright','Day':'Friday','Time':'16:00','Room':'Room 1'},{'Title':'\tYou won’t believe what’s hidden in your data. How visualization transforms how you see information','Speaker':'Walt Ritscher','Day':'Friday','Time':'17:00','Room':'Room 1'},{'Title':'View Components and Tag Helpers in .NET Core','Speaker':'Anna Bateman','Day':'Wednesday','Time':'10:00','Room':'Room 2'},{'Title':'CSS in a component-based world','Speaker':'Alice Brosey','Day':'Wednesday','Time':'11:00','Room':'Room 2'},{'Title':'Simply Containers: For times when Orchestration is Overkill','Speaker':'Chris Houdeshell','Day':'Wednesday','Time':'13:00','Room':'Room 2'},{'Title':'Gaining Value from Telemetry','Speaker':'Chris Houdeshell','Day':'Wednesday','Time':'14:00','Room':'Room 2'},{'Title':'Building Better REST APIs in ASP.NET and ASP.NET Core','Speaker':'Jonathan \'J.\' Tower','Day':'Wednesday','Time':'15:00','Room':'Room 2'},{'Title':'.NET Framework Improvements, Tips, and Tricks','Speaker':'Jeffrey Fritz','Day':'Wednesday','Time':'16:00','Room':'Room 2'},{'Title':'Build the Realtime Web with SignalR Core and Azure SignalR service','Speaker':'Jeffrey Fritz','Day':'Wednesday','Time':'17:00','Room':'Room 2'},{'Title':'Building a Serverless Microservice with Microsoft Azure','Speaker':'Jason Farrell','Day':'Thursday','Time':'10:00','Room':'Room 2'},{'Title':'Getting Started with Blazor: the .NET WebAssembly SPA','Speaker':'Jason Farrell','Day':'Thursday','Time':'11:00','Room':'Room 2'},{'Title':'Dev Oops!  Common mistakes implementing DevOps and how to avoid them','Speaker':'Esteban Garcia','Day':'Thursday','Time':'13:00','Room':'Room 2'},{'Title':'Deliver what the client wants instead of what they ask for! Getting your clients to tell you what they need.','Speaker':'Angel Thomas','Day':'Thursday','Time':'14:00','Room':'Room 2'},{'Title':'git gone wild: how to recover from common git mistakes.','Speaker':'Magnus Stahre','Day':'Thursday','Time':'15:00','Room':'Room 2'},{'Title':'What can Visual Studio do for Mobile Developers?','Speaker':'Sam Basu','Day':'Thursday','Time':'16:00','Room':'Room 2'},{'Title':'Optimizing ReactJS','Speaker':'Scott Kay','Day':'Thursday','Time':'17:00','Room':'Room 2'},{'Title':'OOP, SOLID, and Mixins with TypeScript','Speaker':'Nick Hodges','Day':'Friday','Time':'10:00','Room':'Room 2'},{'Title':'Typescript for the Microsoft Developer','Speaker':'Joseph Guadagno','Day':'Friday','Time':'11:00','Room':'Room 2'},{'Title':'Look into your Application with Azure Application Insights','Speaker':'Joseph Guadagno','Day':'Friday','Time':'13:00','Room':'Room 2'},{'Title':'Feed your inner data scientist: JavaScript tools for data visualization and filtering.','Speaker':'Doug Mair','Day':'Friday','Time':'14:00','Room':'Room 2'},{'Title':'What\'s new in Visual Studio and C# 7','Speaker':'Doug Mair','Day':'Friday','Time':'15:00','Room':'Room 2'},{'Title':'Enable IoT with Edge Computing and Machine Learning','Speaker':'Jared Rhodes','Day':'Friday','Time':'16:00','Room':'Room 2'},{'Title':'Dockerize an ASP.NET Core 2.0 Application','Speaker':'Henry He','Day':'Friday','Time':'17:00','Room':'Room 2'},{'Title':'Programming the Blockchain with .NET Core','Speaker':'Henry He','Day':'Wednesday','Time':'10:00','Room':'Room 3'},{'Title':'There is a Bot for that: Building chat bots from idea to production','Speaker':'Geert van der Cruijsen','Day':'Wednesday','Time':'11:00','Room':'Room 3'},{'Title':'Why loop in JavaScript when you can map, reduce or filter?','Speaker':'Scott McAllister','Day':'Wednesday','Time':'13:00','Room':'Room 3'},{'Title':'Alternative Device Interfaces and Machine Learning','Speaker':'Jared Rhodes','Day':'Wednesday','Time':'14:00','Room':'Room 3'},{'Title':'Angular for Rank Beginners','Speaker':'Nick Hodges','Day':'Wednesday','Time':'15:00','Room':'Room 3'},{'Title':'Cross Platform Mobile Application Development using Xamarin and Azure','Speaker':'Richard Taylor','Day':'Wednesday','Time':'16:00','Room':'Room 3'},{'Title':'Grappling the JavaScript Ecosystem with F# and Fable','Speaker':'Stachu Korick','Day':'Wednesday','Time':'17:00','Room':'Room 3'},{'Title':'Intro to the Microsoft Bot Framework','Speaker':'Matt Burleigh','Day':'Thursday','Time':'10:00','Room':'Room 3'},{'Title':'ASP.NET Core\'s Built-in Dependency Injection Framework','Speaker':'Jonathan \'J.\' Tower','Day':'Thursday','Time':'11:00','Room':'Room 3'},{'Title':'I know ... Kung Fu - Serverless & Schemaless with Azure Functions and CosmosDB','Speaker':'Santosh Hari','Day':'Thursday','Time':'13:00','Room':'Room 3'},{'Title':'Developer Tools in Chrome for Modern Web Development','Speaker':'Alice Brosey','Day':'Thursday','Time':'14:00','Room':'Room 3'},{'Title':'Creating a Release Pipeline with Team Services','Speaker':'Esteban Garcia','Day':'Thursday','Time':'15:00','Room':'Room 3'},{'Title':'The Future of C#','Speaker':'Scott Kay','Day':'Thursday','Time':'16:00','Room':'Room 3'},{'Title':'(Fr)agile: How Agile Falls Apart, and What You Can Do to Hold It All Together','Speaker':'Sean Killeen','Day':'Thursday','Time':'17:00','Room':'Room 3'},{'Title':'React vs Angular vs Vue','Speaker':'Bill Wolff','Day':'Friday','Time':'10:00','Room':'Room 3'},{'Title':'Xamarin.Forms Takes You Places!','Speaker':'Sam Basu','Day':'Friday','Time':'11:00','Room':'Room 3'},{'Title':'Outrun the 🐻Performance Optimizations for Progressive Web Apps','Speaker':'Chris Lorenzo','Day':'Friday','Time':'13:00','Room':'Room 3'}]";
        }

        public List<string> GetSpeakerNames(string speakerInput)
        {
            var allSessions = JsonConvert.DeserializeObject<List<Session>>(SessionService.GetJsonString());

            var query = allSessions.Where(q => q.Speaker.ToLower().Contains(speakerInput)).Select(q =>q.Speaker).Distinct().ToList();

            return query;

        }

        public List<Session> GetSessions(string speakerName)
        {
            var allSessions = JsonConvert.DeserializeObject<List<Session>>(SessionService.GetJsonString());

            var query = allSessions.Where(s => s.Speaker.ToLower().Contains(speakerName.ToLower()));
            var result = query.ToList();
            return result;

        }
        public List<Session> GetSessions(string[] topics, string[] speakerNames, DateTimeSpec[] times)
        {
            var allSessions = JsonConvert.DeserializeObject<List<Session>>(SessionService.GetJsonString());

            var query = allSessions.Select(s => s);

            if (topics?.Any() == true)
            {
                query = query.Where(q => topics.Any(val => q.Title.ToLower().Contains(val.ToLower())));
            }

            if (speakerNames?.Any() == true)
            {
                string speakerNameForSearch;
                if (speakerNames.Count() == 2)
                {
                    speakerNameForSearch = $"{speakerNames[0]} {speakerNames[1]}".ToLower();
                }
                else
                {
                    speakerNameForSearch = $"{speakerNames[0]}".ToLower();
                }
                query = query.Where(q => q.Speaker.ToLower().Contains(speakerNameForSearch));
            }

            if (times?.Any() == true)
            {
                if (times[0].Type == "date")
                {
                    var day = GetDayFromDate(times[0].ToString());
                    query = query.Where(q => q.Day == day);
                }
                if (times[0].Type == "datetime")
                {
                    var day = GetDayFromDateTime(times[0].ToString());
                    query = query.Where(q => q.Day == day);
                    var time = GetTimeFromDateTime(times[0].ToString());
                    query = query.Where(q => q.Time == time);
                }

            }



            var result = query.ToList();
            return result;
        }
        private string GetNextTimeSlot()
        {
            // insert some logic here:
            return "13:00";
        }

        
        private string GetDayFromDate(string day)
        {
            // this stuff is lame. There must be a better way to parse this
            var daypart = day.Substring(29, 1);
                switch (daypart)
            {
                case "1": return "Monday";
                case "2": return "Tuesday";
                case "3": return "Wednesday";
                case "4": return "Thursday";
                case "5": return "Friday";
                case "6": return "Saturday";
                case "7": return "Sunday";
                default: return "Wednesday";
            }
        }
        private string GetDayFromDateTime(string day)
        {
            // this stuff is lame. There must be a better way to parse this
            var daypart = day.Substring(33, 1);
            switch (daypart)
            {
                case "1": return "Monday";
                case "2": return "Tuesday";
                case "3": return "Wednesday";
                case "4": return "Thursday";
                case "5": return "Friday";
                case "6": return "Saturday";
                case "7": return "Sunday";
                default: return "Wednesday";
            }
        }
        private string GetTimeFromDateTime(string day)
        {
            // this stuff is lame. There must be a better way to parse this
            var timepart = day.Substring(35, 5);
            return timepart;
        }

    }
}
