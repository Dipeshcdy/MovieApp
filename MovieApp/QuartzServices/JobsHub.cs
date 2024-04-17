using Microsoft.AspNetCore.SignalR;
using MovieApp.Infrastructure.Interface;

namespace MovieApp.QuartzServices
{
    public class JobsHub : Hub
    {
        private readonly IHubContext<JobsHub> _hubContext;
        private readonly IMovieRepository _movieRepository;

        public JobsHub(IHubContext<JobsHub> hubContext, IMovieRepository movieRepository)
        {
            _hubContext = hubContext;
            _movieRepository = movieRepository;
        }

        public Task SendConcurrentJobsMessage(string message)
        {
            return Clients.All.SendAsync("ConcurrentJobs", message);
        }

        public Task SendNonConcurrentJobsMessage(string message)
        {
            return Clients.All.SendAsync("NonConcurrentJobs", message);
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            // Trigger the ConconcurrentJob when a client connects
            await TriggerConconcurrentJob();
        }
        private async Task TriggerConconcurrentJob()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var movies = _movieRepository.GetAll().Where(x => x.ReleaseDate > today).Take(3).ToList();

            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", movies);
/*
            var endMessage = $"Conconcurrent Job triggered by client connection at {DateTime.UtcNow}";
            await _hubContext.Clients.All.SendAsync("ConcurrentJobs", endMessage);*/
        }
    }
}
