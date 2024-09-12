using ElementaryMathStudyWebsite.Contract.Services.Interface;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Services.Service
{
    public class SubjectService : ISubjectService, IAppSubjectServices
    {
        private readonly DatabaseContext _context;

        public SubjectService(DatabaseContext context)
        {
            _context = context;
        }

        // Helper method for validation
        private void ValidateSubject(SubjectDTO subjectDTO)
        {
            if (string.IsNullOrWhiteSpace(subjectDTO.SubjectName))
            {
                throw new ArgumentException("Subject name is required and cannot be empty.");
            }

            if (subjectDTO.Price == null || subjectDTO.Price <= 0)
            {
                throw new ArgumentException("Price must be greater than zero.");
            }
        }

        // Create a new subject
        public async Task<Subject> CreateSubjectAsync(SubjectDTO subjectDTO)
        {
            // Validate input
            ValidateSubject(subjectDTO);

            var subject = new Subject
            {
                SubjectName = subjectDTO.SubjectName,
                Price = subjectDTO.Price,
                Status = subjectDTO.Status,
            };

            _context.Subject.Add(subject);
            await _context.SaveChangesAsync();

            return subject;
        }

        // Get all subjects, returning as objects
        public async Task<IEnumerable<object>> GetAllSubjectsAsync()
        {
            return await _context.Subject
                .Select(s => new
                {
                    s.SubjectName,
                    s.Price,
                    s.Status
                })
                .ToListAsync();
        }

        // Get a specific subject by ID
        public async Task<Subject> GetSubjectByIDAsync(string id)
        {
            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            return subject;
        }

        // Search subjects by name
        public async Task<IEnumerable<object>> SearchSubjectAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return await GetAllSubjectsAsync();
            }

            var subjects = await _context.Subject
                .Where(s => EF.Functions.Like(s.SubjectName, $"%{searchTerm}%"))
                .Select(s => new
                {
                    s.SubjectName,
                    s.Price,
                    s.Status
                })
                .ToListAsync();

            if (!subjects.Any())
            {
                throw new KeyNotFoundException($"No subjects found with name containing '{searchTerm}'.");
            }

            return subjects;
        }

        // Update subject
        public async Task<Subject> UpdateSubjectAsync(string id, SubjectDTO subjectDTO)
        {
            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            // Validate input
            ValidateSubject(subjectDTO);

            // Update properties
            subject.SubjectName = subjectDTO.SubjectName;
            subject.Price = subjectDTO.Price;
            subject.Status = subjectDTO.Status;

            _context.Subject.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        // Delete subject by ID
        public async Task<Subject> ChangeSubjectStatusAsync(string id)
        {
            var subject = await _context.Subject.FindAsync(id);
            if (subject == null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' not found.");
            }

            subject.Status = !subject.Status;

            _context.Subject.Update(subject);
            await _context.SaveChangesAsync();
            return subject;
        }
    }
}
