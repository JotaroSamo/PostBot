
using Microsoft.Extensions.Options;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using PostBot.DAL.MySql;
using PostBot.DAL.MySql.Repository;
using PostBot.DAL.SqlLite.Repository;
using PostBot.Keyboards;
using PostBot.App;


namespace PostBot.Service
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly BotConfiguration _option;
        private readonly IPostRepository _postRepository;
        private readonly IUserMovieBotService _userMovieBotService;
        private const string GetStr = "GetPost_";

        public UpdateHandler(
            ITelegramBotClient botClient,
            ILogger<UpdateHandler> logger, IOptions<BotConfiguration> options, IPostRepository postRepository, IUserMovieBotService userMovieBotService)
        {
            _botClient = botClient;
            _logger = logger;
            _option = options.Value;
            _postRepository = postRepository;
            _userMovieBotService = userMovieBotService;
        }
        // Метод для вставки пользователя и проверки его существования
        public async Task InsertAndCheckUser(long id)
        {
            // Получаем информацию о пользователе по ID
            var user = await _userMovieBotService.GetByIdUser(id);

            // Если пользователь не найден, добавляем его в БД
            if (user is null)
            {
                await _userMovieBotService.InsertAsync(id);
            }
        }

        // Метод для получения постов в формате InlineQueryResult
        private async Task<List<InlineQueryResult>> GetInlinePosts(IEnumerable<ParsedDataPost> post)
        {
            // Сортируем посты по id и преобразуем в список
            var filteredMovies = post.OrderBy(m => m.id).ToList();
            var results = new List<InlineQueryResult>();

            // Для каждого фильма создаем InlineQueryResult
            foreach (var movie in filteredMovies)
            {
                results.Add(NewInlineQuery(movie));
            }

            // Возвращаем результаты
            return await Task.FromResult(results);
        }

        // Метод для создания InlineQueryResultArticle
        private InlineQueryResultArticle NewInlineQuery(ParsedDataPost post)
        {
            var thumbUrl = post.Image; // Получаем URL изображения
                                       // Проверяем, является ли URL корректным
            if (!Uri.IsWellFormedUriString(thumbUrl, UriKind.Absolute))
            {
                thumbUrl = null;  // Устанавливаем значение null, если URL некорректный
            }

            // Создаем новый InlineQueryResultArticle
            var inlineQueryResult = new InlineQueryResultArticle(
                post.id.ToString(),
                title: post.alt_name,
                inputMessageContent: new InputTextMessageContent(GetStr + post.id.ToString()))
            {
                ThumbnailUrl = thumbUrl, // Устанавливаем миниатюру
                Description = post.Desc, // Устанавливаем описание
            };

            return inlineQueryResult; // Возвращаем результат
        }
        private async Task GetPost(ITelegramBotClient botClient, Update update, User userMessage, int getpost)
        {
            string substring = update.Message.Text?.Substring(getpost + GetStr.Length); // Извлекаем номер поста
            if (!int.TryParse(substring, out int result)) // Проверяем на корректность
            {
                Console.WriteLine("Не удалось конвертировать в int, возвращаем 0");
                return; // Выход из метода, если конвертация не удалась
            }
            await botClient.DeleteMessageAsync(userMessage.Id, update.Message.MessageId); // Удаляем сообщение

            // Получаем пост по ID
            var post = await _postRepository.GetPostById(result);
            // Если у поста есть изображение, отправляем его пользователю
            if (post.Image != null)
            {
                string domain = BotStaticUtilities.GetOrCreateDomainAsync();
                string url = $"{domain}{post.id}-{post.alt_name}.html"; // Генерация URL поста
                InputFileUrl inputFileUrl = new InputFileUrl(post.Image);
                InlineKeyboardMarkup keyboard = new(new[]
                {
                            new []
                            {
                                InlineKeyboardButton.WithUrl("Новость", url)
                            },
                        });

                // Отправляем изображение с описанием и кнопкой
                await botClient.SendPhotoAsync(userMessage.Id, inputFileUrl, caption: post.Desc, replyMarkup: keyboard);
            }
        }
        // Основной метод для обработки обновлений от Telegram
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                // Обработка различных типов обновлений
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery when update.CallbackQuery?.Message is not null:
                        var userCallback = update.CallbackQuery.From;
                        // Здесь можно обработать обратные вызовы от кнопок

                        break;

                    case UpdateType.InlineQuery when update.InlineQuery is not null:
                        var userInline = update.InlineQuery.From; // Получаем пользователя из InlineQuery
                        string query = update.InlineQuery.Query.ToLower(); // Получаем текст запроса
                        try
                        {
                            // Пытаемся найти посты по запросу
                            var post = await _postRepository.SearchPostAsync(query);
                            // Получаем результаты постов
                            var results = await GetInlinePosts(post ?? Enumerable.Empty<ParsedDataPost>());

                            _logger.LogInformation("Подключение успешно"); // Логируем успешное подключение
                            if (results != null && results.Any()) // Если есть результаты
                            {
                                // Отправляем результаты в ответ на InlineQuery
                                await botClient.AnswerInlineQueryAsync(update.InlineQuery.Id, results, cacheTime: 5, isPersonal: true);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Ошибка" + ex.ToString()); // Логируем ошибку

                        }

                        break;

                    case UpdateType.Message when update.Message?.From is not null:
                        var userMessage = update.Message.From; // Получаем пользователя из сообщения

                        // Проверяем и добавляем пользователя
                        await InsertAndCheckUser(userMessage.Id);

                        var getpost = update.Message.Text?.IndexOf(GetStr) ?? -1; // Проверяем, содержит ли сообщение GetStr

                        if (getpost != -1) // Если содержит
                        {
                            await GetPost(botClient, update, userMessage, getpost);
                        }
                        else
                        {
                            var text = update.Message.Text.ToLower(); // Приводим текст сообщения к нижнему регистру
                            switch (text)
                            {
                                default:
                                    // Ответ по умолчанию
                                    await botClient.SendTextMessageAsync(userMessage.Id, "Привет", replyMarkup: CallbackBoard.InlineBtnPanel);
                                    break;
                            }
                        }

                        break;

                    case UpdateType.ChannelPost when update.ChannelPost is not null:
                        var channelPost = update.ChannelPost.Chat; // Получаем информацию о посте в канале
                                                                   // Проверяем, является ли пост из нашего канала
                        if (_option.ChannelId == channelPost.Id)
                        {
                            var users = await _userMovieBotService.GetAllAsync(); // Получаем всех пользователей
                            foreach (var user in users)
                            {
                                // Отправляем текст поста каждому пользователю
                                await botClient.SendTextMessageAsync(user.UserMovieId, update.ChannelPost.Text);
                            }
                        }

                        break;

                    default:
                        _logger.LogError("Error occurred while managing the user's message."); // Логируем ошибку по умолчанию
                        break;
                }
            }
            catch (Exception ex)
            {
                // Логируем любые исключения
                _logger.LogError(ex.ToString());
                await Console.Out.WriteLineAsync(ex.ToString());
            }
        }

       

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            _logger.LogError(errorMessage);

            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }

}

