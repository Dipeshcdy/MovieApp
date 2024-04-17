using Microsoft.AspNetCore.SignalR;
using MovieApp.Infrastructure.Interface;
using Quartz;

namespace MovieApp.QuartzServices
{
    public class ConconcurrentJob : IJob
    {
        private readonly ILogger<ConconcurrentJob> _logger;
        private static int _counter = 0;
        private readonly IHubContext<JobsHub> _hubContext;
        private readonly IMovieRepository _movieRepository;

        public ConconcurrentJob(ILogger<ConconcurrentJob> logger,
            IHubContext<JobsHub> hubContext,IMovieRepository movieRepository)
        {
            _logger = logger;
            _hubContext = hubContext;
            _movieRepository = movieRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var count = _counter++;
            var today = DateOnly.FromDateTime(DateTime.Today);
            var movies = _movieRepository.GetAll().Where(x => x.ReleaseDate > today).Take(3).ToList();
            foreach (var movie in movies)
            {
                _logger.LogInformation($"Movie: {movie.Title}, Release Date: {movie.ReleaseDate}");
            }

            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", movies);

            /*Thread.Sleep(7000);

            var endMessage = $"Conconcurrent Job END {count} {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", endMessage);
            _logger.LogInformation(endMessage);*/
        }
    }
}
