using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace educ
{
    /// <summary>
    /// Сервис для работы с административной панелью
    /// Содержит логику загрузки данных и обработки действий администратора
    /// </summary>
    public class AdminService
    {
        private ComplaintService _complaintService = new ComplaintService();
        private AuthorRequestService _authorService = new AuthorRequestService();
        private UnfreezeRequestService _unfreezeService = new UnfreezeRequestService();
        private UserService _userService = new UserService();
        private BookService _bookService = new BookService();
        private ReviewService _reviewService = new ReviewService();

        /// <summary>
        /// Возвращает данные в зависимости от выбранного фильтра
        /// </summary>
        public List<object> GetDataForFilter(string filter)
        {
            switch (filter)
            {
                case "All":
                    return LoadAllRequests();
                case "Complaints":
                    return LoadComplaints();
                case "AuthorRequests":
                    return LoadAuthorRequests();
                case "Unfreeze":
                    return LoadUnfreezeRequests();
                case "Users":
                    return LoadUsers();
                case "FrozenBooks":
                    return LoadFrozenBooks();
                case "FrozenReviews":
                    return LoadFrozenReviews();
                default:
                    return new List<object>();
            }
        }

        /// <summary>
        /// Загружает все типы заявок 
        /// </summary>
        private List<object> LoadAllRequests()
        {
            var complaints = _complaintService.GetPendingComplaints().Select(CreateComplaintItem);
            var authorReqs = _authorService.GetPendingAuthorRequests().Select(CreateAuthorRequestItem);
            var unfreezeReqs = _unfreezeService.GetAllUnfreezeRequests().Where(u => u.Status == 0).Select(CreateUnfreezeRequestItem);

            return complaints.Concat(authorReqs).Concat(unfreezeReqs).ToList();
        }

        /// <summary>
        /// Загружает список жалоб
        /// </summary>
        private List<object> LoadComplaints()
        {
            return _complaintService.GetPendingComplaints().OrderByDescending(c => c.CreatedAt).Select(CreateComplaintItem).ToList<object>();
        }

        /// <summary>
        /// Загружает заявки на получение роли автора
        /// </summary> 
        private List<object> LoadAuthorRequests()
        {
            return _authorService.GetPendingAuthorRequests().OrderByDescending(a => a.CreatedAt).Select(CreateAuthorRequestItem).ToList<object>();
        }

        /// <summary>
        /// Загружает заявки на разморозку
        /// </summary>
        private List<object> LoadUnfreezeRequests()
        {
            return _unfreezeService.GetAllUnfreezeRequests().Where(u => u.Status == 0).OrderByDescending(u => u.CreatedAt).Select(CreateUnfreezeRequestItem).ToList<object>();
        }

        /// <summary>
        /// Загружает список всех пользователей
        /// </summary>
        private List<object> LoadUsers()
        {
            return _userService.GetAllUsers().Where(u => u.Role != 2).OrderBy(u => u.Login).Select(CreateUserItem).ToList<object>();
        }

        /// <summary>
        /// Загружает список замороженных книг
        /// </summary>
        private List<object> LoadFrozenBooks()
        {
            return _bookService.GetAllBooks().Where(b => b.IsFrozen).Select(CreateFrozenBookItem).ToList<object>();
        }

        /// <summary>
        /// Загружает список замороженных отзывов
        /// </summary>
        private List<object> LoadFrozenReviews()
        {
            return _reviewService.GetAllReviews().Where(r => r.IsFrozen).Select(CreateFrozenReviewItem).ToList<object>();
        }

        /// <summary>
        /// объект для отображения жалобы
        /// </summary>
        private object CreateComplaintItem(Complaints c) => new
        {
            Type = GetComplaintType(c),
            Details = GetComplaintTargetFullInfo(c),
            FromUser = $"Жалобу отправил: {GetUserName(c.UserId)}",
            TargetInfo = GetComplaintTargetFullInfo(c),
            Reason = c.Reason,
            Date = c.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Object = c,
            ShowTwoButtons = Visibility.Visible,
            ShowThreeButtons = Visibility.Collapsed,
            ShowOneButton = Visibility.Collapsed
        };

        /// <summary>
        /// объект для отображения заявки на автора
        /// </summary>
        private object CreateAuthorRequestItem(AuthorRequests a) => new
        {
            Type = "Заявка на автора",
            Details = $"Пользователь: {a.Users?.Login}",
            FromUser = $"Имя: {a.Users?.Name}",
            TargetInfo = $"Email: {a.Users?.Email}",
            Reason = a.Reason,
            Date = a.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Object = a,
            ShowTwoButtons = Visibility.Visible,
            ShowThreeButtons = Visibility.Collapsed,
            ShowOneButton = Visibility.Collapsed
        };

        /// <summary>
        /// объект для отображения заявки на разморозку
        /// </summary>
        private object CreateUnfreezeRequestItem(UnfreezeRequests u) => new
        {
            Type = "Заявка на разморозку",
            Details = GetUnfreezeDetails(u),
            FromUser = $"Запрос от: {u.Users?.Login}",
            TargetInfo = GetUnfreezeTarget(u),
            Reason = u.Reason,
            Date = u.CreatedAt.ToString("dd.MM.yyyy HH:mm"),
            Object = u,
            ShowTwoButtons = Visibility.Visible,
            ShowThreeButtons = Visibility.Collapsed,
            ShowOneButton = Visibility.Collapsed
        };

        /// <summary>
        /// объект для отображения пользователя
        /// </summary>
        private object CreateUserItem(Users u) => new
        {
            Type = GetUserRoleShort(u),
            Details = $"Логин: {u.Login} Имя: {u.Name}",
            FromUser = $"Email: {u.Email}",
            StatusText = GetUserStatusDisplay(u),
            Reason = u.IsFrozen ? $"Причина заморозки: {u.FreezeReazon}" : "Пользователь активен",
            Object = u,
            ShowTwoButtons = Visibility.Collapsed,
            ShowThreeButtons = Visibility.Visible,
            ShowOneButton = Visibility.Collapsed
        };

        /// <summary>
        /// объект для отображения замороженной книги
        /// </summary>
        private object CreateFrozenBookItem(Books b) => new
        {
            Type = "Замороженная книга",
            Details = $"Название: {b.Title}",
            FromUser = $"Автор: {b.Users?.Name}",
            Object = b,
            ShowTwoButtons = Visibility.Collapsed,
            ShowThreeButtons = Visibility.Collapsed,
            ShowOneButton = Visibility.Visible
        };

        /// <summary>
        /// объект для отображения замороженного отзыва
        /// </summary>
        private object CreateFrozenReviewItem(Reviews r) => new
        {
            Type = "Замороженный отзыв",
            Details = $"На книгу: {r.Books?.Title}",
            FromUser = $"Автор отзыва: {r.Users?.Name}",
            TargetInfo = $"Оценка: {r.Rating}⭐",
            Reason = $"Причина заморозки: {r.FreezeReazon}",
            Date = $"Дата отзыва: {r.CreatedAt.ToString("dd.MM.yyyy")}",
            Object = r,
            ShowTwoButtons = Visibility.Collapsed,
            ShowThreeButtons = Visibility.Collapsed,
            ShowOneButton = Visibility.Visible
        };

        /// <summary>
        /// Определяем тип жалобы
        /// </summary>
        private string GetComplaintType(Complaints c)
        {
            if (c.ComplaintsBookId != null) return "Жалоба на книгу";
            if (c.ComplaintsReviewId != null) return "Жалоба на отзыв";
            if (c.ComplaintsUserId != null) return "Жалоба на автора";
            return "Жалоба";
        }

        /// <summary>
        /// подробная информацию о цели жалобы
        /// </summary>
        private string GetComplaintTargetFullInfo(Complaints c)
        {
            if (c.ComplaintsBookId != null)
            {
                var book = Core.context.Books.Find(c.ComplaintsBookId);
                return $"Книга: {book.Title}  Автор: {book.Users.Name}";
            }
            if (c.ComplaintsReviewId != null)
            {
                var review = Core.context.Reviews.Find(c.ComplaintsReviewId);
                return $"Отзыв от {review.Users.Name} на книгу: {review.Books.Title}";
            }
            if (c.ComplaintsUserId != null)
            {
                var user = Core.context.Users.Find(c.ComplaintsUserId);
                return $"Автор: {user.Name} ({user.Login})";
            }
            return "Неизвестно";
        }

        /// <summary>
        /// информация о цели заявки на разморозку
        /// </summary>
        private string GetUnfreezeTarget(UnfreezeRequests u)
        {
            if (u.UnfreezeBookId != null)
            {
                var book = Core.context.Books.Find(u.UnfreezeBookId);
                return $"Книга: {book?.Title}";
            }
            if (u.UnfreezeUserId != null)
            {
                var user = Core.context.Users.Find(u.UnfreezeUserId);
                return $"Пользователь: {user?.Login}";
            }
            return "Неизвестно";
        }

        /// <summary>
        /// описание заявки на разморозку
        /// </summary>
        private string GetUnfreezeDetails(UnfreezeRequests u)
        {
            if (u.UnfreezeBookId != null)
            {
                var book = Core.context.Books.Find(u.UnfreezeBookId);
                return $"Хочет разморозить книгу: {book?.Title}";
            }
            if (u.UnfreezeUserId != null)
            {
                var user = Core.context.Users.Find(u.UnfreezeUserId);
                return $"Хочет разморозить пользователя: {user.Login}";
            }
            return "Неизвестно";
        }

        /// <summary>
        /// статус пользователя (активен/заморожен)
        /// </summary>
        private string GetUserStatusDisplay(Users user) => user.IsFrozen ? "Статус: заморожен" : "Статус: активен";

        /// <summary>
        /// роль пользователя
        /// </summary>
        private string GetUserRoleShort(Users user)
        {
            if (user == null) return "Неизвестно";
            switch (user.Role)
            {
                case 2: return "Администратор";
                case 1: return "Автор";
                case 0: return "Читатель";
                default: return "Неизвестно";
            }
        }

        /// <summary>
        /// логин пользователя по ID
        /// </summary>
        private string GetUserName(int? userId)
        {
            var user = Core.context.Users.Find(userId); 
            return user.Login;
        }

        /// <summary>
        /// Извлекает свойство объекта
        /// </summary>
        public object GetObjectFromItem(object item)
        {
            if (item == null) return null;

            var property = item.GetType().GetProperty("Object");

            if (property != null)
            {
                return property.GetValue(item);
            }

            return null;
        }

        /// <summary>
        /// Одобрение заявки/жалобы
        /// </summary>
        public void ApproveItem(object item, Users currentUser)
        {
            if (item == null) return;
            object obj = GetObjectFromItem(item);

            if (obj is Complaints complaint) ProcessComplaint(complaint, true);
            else if (obj is AuthorRequests authorReq) ApproveAuthorRequest(authorReq);
            else if (obj is UnfreezeRequests unfreezeReq) ProcessUnfreezeRequest(unfreezeReq, true);
        }

        /// <summary>
        /// Отклонение заявки/жалобы
        /// </summary>
        public void RejectItem(object item, Users currentUser)
        {
            if (item == null) return;
            object obj = GetObjectFromItem(item);

            if (obj is Complaints complaint)
            {
                _complaintService.UpdateComplaintStatus(complaint.Id, 2, currentUser?.Id ?? 0);
                MessageBox.Show("Жалоба отклонена", "Успешно");
            }
            else if (obj is AuthorRequests authorReq)
            {
                _authorService.UpdateAuthorRequestStatus(authorReq.Id, 2);
                MessageBox.Show("Заявка отклонена", "Успешно");
            }
            else if (obj is UnfreezeRequests unfreezeReq)
            {
                unfreezeReq.Status = 2;
                Core.context.SaveChanges();
                MessageBox.Show("Заявка отклонена", "Успешно");
            }
        }

        /// <summary>
        /// Разморозка книги или отзыва
        /// </summary>
        public void UnfreezeItem(object item)
        {
            if (item == null) return;
            object obj = GetObjectFromItem(item);

            if (obj is Books book)
            {
                book.IsFrozen = false;
                book.FreezeReazon = null;
                MessageBox.Show($"Книга \"{book.Title}\" разморожена", "Успешно");
            }
            else if (obj is Reviews review)
            {
                review.IsFrozen = false;
                review.FreezeReazon = null;
                MessageBox.Show("Отзыв разморожен", "Успешно");
            }
            Core.context.SaveChanges();
        }

        /// <summary>
        /// Заморозка пользователя
        /// </summary>
        public void FreezeUser(Users user)
        {
            string reason = Interaction.InputBox("Введите причину заморозки пользователя:", "Заморозка пользователя", "");
            if (!string.IsNullOrWhiteSpace(reason))
            {
                user.IsFrozen = true;
                user.FreezeReazon = reason;
                Core.context.SaveChanges();
                MessageBox.Show($"Пользователь {user.Login} заморожен", "Успешно");
            }
        }

        /// <summary>
        /// Смена роли пользователя (Автор ↔ Читатель)
        /// </summary>
        public void ChangeUserRole(Users user)
        {
            string oldRole = GetUserRoleShort(user);
            if (user.Role == 1)
            {
                user.Role = 0;  
            }
            else
            {
                user.Role = 1;   
            }
            string newRole = GetUserRoleShort(user);
            Core.context.SaveChanges();
            MessageBox.Show($"Роль пользователя {user.Login} изменена с {oldRole} на {newRole}", "Успешно");
        }

        /// <summary>
        /// Смена пароля пользователя
        /// </summary>
        public void ChangeUserPassword(Users user)
        {
            string newPassword = Interaction.InputBox($"Введите новый пароль для {user.Login}:", "Смена пароля");
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                user.Password = newPassword;
                Core.context.SaveChanges();
                MessageBox.Show($"Пароль для {user.Login} успешно изменён", "Успешно");
            }
        }

        /// <summary>
        /// Обработка одобрения жалобы 
        /// </summary>
        private void ProcessComplaint(Complaints complaint, bool accept)
        {
            if (!accept) return;
            string reason = complaint.Reason ?? "Без причины";

            if (complaint.ComplaintsBookId != null)
            {
                var book = Core.context.Books.Find(complaint.ComplaintsBookId);
                if (book != null)
                {
                    book.IsFrozen = true;
                    book.FreezeReazon = reason;
                    MessageBox.Show($"Книга \"{book.Title}\" заморожена", "Успешно");
                }
            }
            else if (complaint.ComplaintsReviewId != null)
            {
                var review = Core.context.Reviews.Find(complaint.ComplaintsReviewId);
                if (review != null)
                {
                    review.IsFrozen = true;
                    review.FreezeReazon = reason;
                    MessageBox.Show("Отзыв заморожен", "Успешно");
                }
            }
            else if (complaint.ComplaintsUserId != null)
            {
                var user = Core.context.Users.Find(complaint.ComplaintsUserId);
                if (user != null)
                {
                    user.IsFrozen = true;
                    user.FreezeReazon = reason;
                    MessageBox.Show($"Автор {user.Name} заморожен", "Успешно");
                }
            }

            _complaintService.UpdateComplaintStatus(complaint.Id, 1, App.CurrentUser?.Id ?? 0);
            Core.context.SaveChanges();
        }

        /// <summary>
        /// Одобрение заявки на автора
        /// </summary>
        private void ApproveAuthorRequest(AuthorRequests request)
        {
            _authorService.UpdateAuthorRequestStatus(request.Id, 1);
            if (request.Users != null)
                request.Users.Role = 1;
            Core.context.SaveChanges();
            MessageBox.Show($"Пользователь {request.Users?.Login} получил роль автора", "Успешно");
        }

        /// <summary>
        /// Обработка заявки на разморозку
        /// </summary>
        private void ProcessUnfreezeRequest(UnfreezeRequests request, bool accept)
        {
            if (!accept) return;

            if (request.UnfreezeUserId != null)
            {
                var user = Core.context.Users.Find(request.UnfreezeUserId);
                if (user != null)
                {
                    user.IsFrozen = false;
                    user.FreezeReazon = null;
                    MessageBox.Show($"Пользователь {user.Login} разморожен", "Успешно");
                }
            }
            if (request.UnfreezeBookId != null)
            {
                var book = Core.context.Books.Find(request.UnfreezeBookId);
                if (book != null)
                {
                    book.IsFrozen = false;
                    book.FreezeReazon = null;
                    MessageBox.Show($"Книга \"{book.Title}\" разморожена", "Успешно");
                }
            }
            request.Status = 1;
            Core.context.SaveChanges();
        }
    }
}