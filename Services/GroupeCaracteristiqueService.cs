using Microsoft.EntityFrameworkCore;
using PFE_PROJECT.Data;
using PFE_PROJECT.Models;

namespace PFE_PROJECT.Services
{
    public class GroupeCaracteristiqueService : IGroupeCaracteristiqueService
    {
        private readonly ApplicationDbContext _context;

        public GroupeCaracteristiqueService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GroupeCaracteristiqueDTO>> GetAllAsync()
        {
            return await _context.GroupeCaracteristiques
                .Include(gc => gc.Caracteristique)
                .Select(gc => new GroupeCaracteristiqueDTO
                {
                    idgrpidq = gc.idgrpidq,
                    idcarac = gc.idcarac,
                    libelleCaracteristique = gc.Caracteristique.libelle
                })
                .ToListAsync();
        }

        public async Task<GroupeIdentiqueDTO?> GetByIdAsync(int id)
        {
            var g = await _context.GroupeIdentiques
                .Include(g => g.Marque)
                .Include(g => g.TypeEquip)
                .Include(g => g.GroupeOrganes).ThenInclude(go => go.Organe)
                .Include(g => g.GroupeCaracteristiques).ThenInclude(gc => gc.Caracteristique)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (g == null) return null;
            return new GroupeIdentiqueDTO
            {
                Id = g.Id,
                CodeGrp = g.codegrp,
                MarqueNom = g.Marque.nom_fabriquant,
                TypeEquipNom = g.TypeEquip.designation,
                IdMarque = g.id_marque,
                IdType = g.id_type_equip,

                Organes = g.GroupeOrganes.Select(o => o.Organe.libelle_organe).ToList(),
                OrganesIds = g.GroupeOrganes.Select(o => o.Organe.id_organe).ToList(),
                Caracteristiques = g.GroupeCaracteristiques.Select(c => c.Caracteristique.libelle).ToList(),
                CaracteristiquesIds = g.GroupeCaracteristiques.Select(c => c.Caracteristique.id_caracteristique).ToList()
            };
        }

        public async Task<IEnumerable<GroupeCaracteristiqueDTO>> CreateAsync(int idgrpidq, List<int> idcaracs)
        {
            var groupeCaracteristiques = idcaracs.Select(idcarac => new GroupeCaracteristique
            {
                idgrpidq = idgrpidq,
                idcarac = idcarac
            }).ToList();

            _context.GroupeCaracteristiques.AddRange(groupeCaracteristiques);
            await _context.SaveChangesAsync();

            return await GetByGroupeIdAsync(idgrpidq);
        }

        public async Task<bool> DeleteByGroupeIdAsync(int idgrpidq)
        {
            var groupeCaracteristiques = await _context.GroupeCaracteristiques
                .Where(gc => gc.idgrpidq == idgrpidq)
                .ToListAsync();

            if (!groupeCaracteristiques.Any()) return false;

            _context.GroupeCaracteristiques.RemoveRange(groupeCaracteristiques);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<GroupeCaracteristiqueDTO>> GetByGroupeIdAsync(int idgrpidq)
        {
            return await _context.GroupeCaracteristiques
                .Include(gc => gc.Caracteristique)
                .Where(gc => gc.idgrpidq == idgrpidq)
                .Select(gc => new GroupeCaracteristiqueDTO
                {
                    idgrpidq = gc.idgrpidq,
                    idcarac = gc.idcarac,
                    libelleCaracteristique = gc.Caracteristique.libelle
                })
                .ToListAsync();
        }
    }
} 