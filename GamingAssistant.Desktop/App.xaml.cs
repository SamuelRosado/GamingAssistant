using GamingAssistant.Application.Abstractions.Repositories;
using GamingAssistant.Application.Abstractions.Services;
using GamingAssistant.Application.Configuration;
using GamingAssistant.Application.Features.ReplayBuffer.Commands.Handlers;
using GamingAssistant.Application.Services;
using GamingAssistant.Infrastructure.Persistence;
using GamingAssistant.Infrastructure.Repositories;
using GamingAssistant.Infrastructure.Services;
using GamingAssistant.Infrastructure.Streaming.ReplayBuffer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace GamingAssistant.Desktop
{
    public partial class App : System.Windows.Application
    {
        private ServiceProvider? _serviceProvider;
        private readonly SamiraPersonalityLoader _personalityLoader = new();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();

            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"SAMIRA");
            Directory.CreateDirectory(appDataPath);
            string databasePath = Path.Combine(appDataPath,"samira.db");

            var personalitySettings = _personalityLoader.Load();
            services.AddDbContextFactory<SamiraDbContext>(options =>
            {
                options.UseSqlite(
                    $"Data Source={databasePath};Foreign Keys=True");
            });
            services.AddTransient<IConversationRepository, ConversationRepository>();
            services.AddSingleton(personalitySettings);
            services.AddSingleton<IReplayBufferService, ReplayBufferService>();
            services.AddTransient<SaveReplayCommandHandler>();
            services.AddTransient<MainWindow>();
            services.AddSingleton(new SamiraSettings("Samira", "Coronel", "es-419"));
            services.AddSingleton<SamiraIdentityService>();
            services.AddSingleton<SamiraPersonalityService>();
            services.AddHttpClient<IChatCompletionService, OllamaChatCompletionService>();

            _serviceProvider = services.BuildServiceProvider();

            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

        }
    }

}
