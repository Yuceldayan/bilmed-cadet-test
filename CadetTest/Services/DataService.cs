using CadetTest.Entities;
using CadetTest.Helpers;
using CadetTest.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace CadetTest.Services
{
    public interface IDataService
    {
        string GetRandomString(int stringLength);
        List<Consent> GetRangeById(int id, int adet);
        Task<Consent> GetByIdAsync(int id);
        Task<Consent> CreateAsync(ConsentSaveRequest request);
        Task<Consent> UpdateAsync(int id, ConsentSaveRequest request);
        Task<bool> DeleteAsync(int id);
    }
    public class DataService : IDataService
    {
        private DataContext _context;

        public DataService(DataContext context)
        {
            _context = context;
        }

        #region Public Methods

        public List<Consent> GetRangeById(int id, int adet)
        {
            var cevap = _context.Consents.Where(k => k.Id >= id).OrderBy(k => k.Id).Take(adet).ToList();
            return cevap;
        }

        public async Task<Consent> GetByIdAsync(int id)
        {
            return await _context.Consents.AsNoTracking().FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Consent> CreateAsync(ConsentSaveRequest request)
        {
            var consent = new Consent();
            CopyFields(request, consent);

            _context.Consents.Add(consent);
            await _context.SaveChangesAsync();

            return consent;
        }

        public async Task<Consent> UpdateAsync(int id, ConsentSaveRequest request)
        {
            var consent = await _context.Consents.FirstOrDefaultAsync(k => k.Id == id);
            if (consent == null) return null;

            CopyFields(request, consent);
            await _context.SaveChangesAsync();

            return consent;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var consent = await _context.Consents.FirstOrDefaultAsync(k => k.Id == id);
            if (consent == null) return false;

            _context.Consents.Remove(consent);
            await _context.SaveChangesAsync();

            return true;
        }

        public string GetRandomString(int stringLength)
        {
            var sb = new StringBuilder();
            int numGuidsToConcat = (((stringLength - 1) / 32) + 1);
            for (int i = 1; i <= numGuidsToConcat; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }

            return sb.ToString(0, stringLength);
        }
        #endregion

        #region Private Methods

        private static void CopyFields(ConsentSaveRequest request, Consent consent)
        {
            consent.Type = request.Type;
            consent.Recipient = request.Recipient.Trim();
            consent.Status = request.Status;
            consent.RecipientType = request.RecipientType;
        }
        #endregion
    }
}
