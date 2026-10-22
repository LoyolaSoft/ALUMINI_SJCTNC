using CMS.DAO;
namespace CMS.SQL.AddOnCourse
{
    class AddOnCourseSQL
    {
        public static string GetAddOnCourse(AddOnCourseSQLCommands sEnumCommand)
        {
            string query = "";
            switch (sEnumCommand)
            {
                case AddOnCourseSQLCommands.ListCourses:
                    {
                        query = @"";
                    }
                    break;
            }
            return query;
        }
    }
}
