using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace labo.signalr.api.Hubs
{
    public class UselessTasksHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private static int _nbUser;
        public UselessTasksHub(ApplicationDbContext context)
        {
            _context = context;
        }
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            _nbUser++;
            await Clients.Caller.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }

        public async Task AjouterTache(UselessTask nouvelleTache)
        {
            _context.UselessTasks.Add(nouvelleTache);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }
        public async Task CompleteTache (int tacheId)
        {
            UselessTask? tacheCompleter = await _context.UselessTasks.FindAsync(tacheId);
            tacheCompleter!.Completed = true;
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToListAsync());
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
            // TODO: Ajouter votre logique
            _nbUser--;
            await Clients.All.SendAsync("UserCount", _nbUser);
        }
    }
}
