using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public interface ITestRepository
{
  Task<Test> AddTest(Test test);
  Task<Test> GetTest(int id);
  Task<Test> UpdateTest(Test test);
  Task<bool> DeleteTest(int id);
}