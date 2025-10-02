using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context
{
    public class ProjectDbContext : DbContext
    {
        private readonly IMediator? _mediator;
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = default!;
        public DbSet<Agency> Agencies { get; set; } = default!;
        public DbSet<Responder> Responders { get; set; } = default!;
        public DbSet<Incident> Incidents { get; set; } = default!;
        public DbSet<IncidentResponder> IncidentResponders { get; set; } = default!;
        public DbSet<IncidentLiveStream> IncidentLiveStreams { get; set; } = default!;
        public DbSet<IncidentLiveStreamParticipant> IncidentLiveStreamParticipants { get; set; } = default!;
        public DbSet<IncidentMedia> IncidentMedias { get; set; } = default!;
        public DbSet<AgencySupportedIncident> AgencySupportedIncidents { get; set; } = default!;
        public DbSet<AgencySupportedWork> AgencySupportedWorks { get; set; } = default!;
        public DbSet<ResponderSupportedIncident> ResponderSupportedIncidents { get; set; } = default!;
        public DbSet<ResponderSupportedWork> ResponderSupportedWorks { get; set; } = default!;
        public DbSet<IncidentLocationUpdate> IncidentLocationUpdates { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;


        internal async Task DispatchDomainEventsAsync()
        {
            var domainEntities = ChangeTracker
                .Entries<AuditableEntity>()
                .Where(x => x.Entity.DomainEvents.Any())
                .Select(x => x.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.DomainEvents)
                .ToList();

            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectDbContext).Assembly);
        }

    }
}
