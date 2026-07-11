using InsureFlowAPI.Data;
using InsureFlowAPI.Models;
using InsureFlowAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InsureFlowAPI.Repositories
{
    public class ClaimDocumentRepository : IClaimDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        // Constructor for Dependency Injection
        public ClaimDocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves all claim documents.
        public async Task<IEnumerable<ClaimDocument>> GetAllAsync()
        {
            return await _context.ClaimDocuments
                .Include(cd => cd.Claim)
                .ToListAsync();
        }

        // Retrieves all documents for a specific claim.
        public async Task<IEnumerable<ClaimDocument>> GetByClaimIdAsync(int claimId)
        {
            return await _context.ClaimDocuments
                .Where(cd => cd.ClaimId == claimId)
                .Include(cd => cd.Claim)
                .ToListAsync();
        }

        // Retrieves a claim document by its unique identifier.
        public async Task<ClaimDocument?> GetByIdAsync(int id)
        {
            return await _context.ClaimDocuments
                .Include(cd => cd.Claim)
                .FirstOrDefaultAsync(cd => cd.DocumentId == id);
        }

        // Adds a new claim document.
        public async Task AddAsync(ClaimDocument claimDocument)
        {
            await _context.ClaimDocuments.AddAsync(claimDocument);
        }

        // Saves all pending changes to the database.
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}