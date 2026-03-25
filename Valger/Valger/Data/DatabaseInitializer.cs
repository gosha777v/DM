using Microsoft.EntityFrameworkCore;
using Valger.Data;
using Valger.Models;
using System.Linq;

namespace Valger
{
    public static class DatabaseInitializer
    {
        public static void Initialize(Context context)
        {
            // Создаём базу, если её нет
            context.Database.EnsureCreated();

            // Проверяем, есть ли уже роли
            if (!context.Roles.Any())
            {
                // Создаём стандартные роли
                var roles = new Role[]
                {
                    new Role { RoleName = "Администратор" },
                    new Role { RoleName = "Менеджер" },
                    new Role { RoleName = "Пользователь" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // Проверяем, есть ли уже пользователи
            if (!context.Users.Any())
            {
                // Создаём администратора по умолчанию
                var adminRole = context.Roles.First(r => r.RoleName == "Администратор");
                var adminUser = new User
                {
                    Login = "admin",
                    Password = "admin123",
                    FullName = "Администратор системы",
                    RoleId = adminRole.RoleId
                };
                context.Users.Add(adminUser);

                // Также можно создать тестового менеджера
                var managerRole = context.Roles.First(r => r.RoleName == "Менеджер");
                var managerUser = new User
                {
                    Login = "manager",
                    Password = "manager123",
                    FullName = "Тестовый менеджер",
                    RoleId = managerRole.RoleId
                };
                context.Users.Add(managerUser);

                // Создаём обычного пользователя
                var userRole = context.Roles.First(r => r.RoleName == "Пользователь");
                var regularUser = new User
                {
                    Login = "user",
                    Password = "user123",
                    FullName = "Тестовый пользователь",
                    RoleId = userRole.RoleId
                };
                context.Users.Add(regularUser);

                context.SaveChanges();
            }
        }
    }
}