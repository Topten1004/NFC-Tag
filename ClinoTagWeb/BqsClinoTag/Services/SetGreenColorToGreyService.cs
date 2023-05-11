//                                      Allowed values    Allowed special characters   Comment
//┌───────────── second(optional)       0 - 59              * , - /
//│ ┌───────────── minute               0 - 59              * , - /
//│ │ ┌───────────── hour               0 - 23              * , - /
//│ │ │ ┌───────────── day of month      1-31               * , - / L W ?                
//│ │ │ │ ┌───────────── month           1-12 or JAN-DEC    * , - /                      
//│ │ │ │ │ ┌───────────── day of week   0-6  or SUN-SAT    * , - / # L ?                Both 0 and 7 means SUN
//│ │ │ │ │ │
//* * * * * *

using BqsClinoTag.Models;
using Cronos;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Services
{
    public class SetGreenColorToGreyService : BackgroundService
    {
        private readonly CLINOTAGBQSContext _context;
        private readonly CronExpression _cron;

        private const string schedule = "0 0 * * *"; // every day (any day of the week, any month, any day of the month, at 12:00AM)

        public SetGreenColorToGreyService(CLINOTAGBQSContext context) {
            _context = context;

            _cron = CronExpression.Parse(schedule);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                var utcNow = DateTime.UtcNow;
                var nextUtc = _cron.GetNextOccurrence(utcNow);
                var delaySpan = nextUtc.Value - utcNow;

                await Task.Delay(delaySpan, stoppingToken);

                await ChangeColor();
            }
        }

        private async Task ChangeColor()
        {
            try
            {
                var lieus = await _context.Lieus.ToListAsync();
                
                foreach(var item in lieus)
                {
                    item.Progress = 0;
                }

                await _context.SaveChangesAsync();

            } catch (Exception ex)
            {
            
            }
        }
    }
}
