﻿using EntityFrameworkFirst.Entities;

namespace EntityFrameworkFirst.Repositories
{
    public interface IRepositorySubject
    {
        Task CreateSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task DeleteSubjectAsync(Subject subject);
        Task<Subject> GetSubjectAsync(Guid subjectId);
        Task<IReadOnlyCollection<Subject>> GetAllSubjectsAsync();
    }
}
