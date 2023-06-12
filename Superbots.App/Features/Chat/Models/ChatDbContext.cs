using Microsoft.EntityFrameworkCore;

/*
 * ENTITY FRAMEWORK INFO
 * 
 * Comandi base per inizializzare e aggiornare un Entity Framework database. In questo caso come
 * database engine usiamo SQLite. 
 * 
 * Una volta definiti i modelli con le loro PK e FK usare il comando:
 * 
 * CMD: dotnet ef migrations add InitialMigration -o ./folder
 * 
 * Il nome InitialMigration può essere quello che vogliamo, serve per poter effettuare upgrade o rollback.
 * Questo comando identifica i modelli e le loro relazioni, con il comando successivo andremo a creare
 * le tabelle nel nostro db e le loro constraints
 * 
 * CMD: dotnet ef database update
 */
namespace Superbots.App.Features.Chat.Models
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
    }
}
