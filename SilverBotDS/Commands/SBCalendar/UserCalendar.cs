using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SilverBotDS.Commands.SBCalendar
{
    internal class UserCalendar
    {
        private Calendar calendar;
        private GraphServiceClient graphClient;

        public async Task<UserCalendar> GetCalendar(Objects.Config config, System.Func<DeviceCodeResult, Task> deviceCodeReadyCallback)
        {
            var scopes = new[] { "Calendars.ReadWrite"/*, "Calendars.ReadWrite.Shared"*/, "email", "User.Read" };

            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
              .Create(config.MicrosoftGraphClientId)

              .Build();

            graphClient = new(new DeviceCodeProvider(publicClientApplication, scopes, deviceCodeReadyCallback));
            calendar = await graphClient.Me.Calendar
                .Request()
                .GetAsync();
            return this;
        }

        private readonly List<Event[]> Pages = new();
        private uint page;

        public string UserName()
        {
            return calendar.Owner.Name;
        }

        public string Email()
        {
            return calendar.Owner.Address;
        }

        public async Task Add(Event task)
        {
            await graphClient.Me.Calendar.Events.Request().AddAsync(task);
        }

        public async Task<Event[]> GetCurrentPageAsync()
        {
            if (calendar.CalendarView is null)
            {
                var queryOptions = new List<QueryOption>
{
    new QueryOption("startDateTime",( DateTime.UtcNow- TimeSpan.FromDays(365)).ToString("o")),
    new QueryOption("endDateTime", DateTime.UtcNow.ToString("o"))
};

                calendar.CalendarView = await graphClient.Me.Calendar.CalendarView.Request(queryOptions).GetAsync();
            }
            if (Pages.Count - 1 >= page)
            {
                return Pages[(int)page];
            }
            else
            {
                Pages.Add(calendar.CalendarView.CurrentPage.ToArray());
                return await GetCurrentPageAsync();
            }
        }

        public void NextPage()
        {
            calendar.CalendarView.InitializeNextPageRequest(graphClient, calendar.CalendarView.NextPageRequest.RequestUrl);
            page++;
        }

        public void BeforePage()
        {
            page--;
        }
    }
}