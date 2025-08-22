private async Task<(int? userId, int? anonymousId)> GetUserIdAndAnonymousIdAsync()
{
    var userIdClaim = User.FindFirstValue(AuthOptions.UserIdClaimType);
    int? userId = null;
    int? anonymousId = null;

    if (userIdClaim != null && int.TryParse(userIdClaim, out int parsedUserId))
    {
        userId = parsedUserId;
        return (userId, anonymousId);
    }

    // Анонимный пользователь
    var sessionIdHeader = Request.Headers["X-Session-Id"].ToString();

    if (!string.IsNullOrEmpty(sessionIdHeader) && Guid.TryParse(sessionIdHeader, out Guid parsedSessionId))
    {
        var anonymousUser = await _context.Anonymous
            .FirstOrDefaultAsync(a => a.SessionId == parsedSessionId);

        if (anonymousUser != null)
        {
            anonymousId = anonymousUser.Id;
            return (userId, anonymousId);
        }
    }

    // Если SessionId не найден или невалиден — создаём нового анонимного пользователя
    var newSessionId = Guid.NewGuid();
    var newAnonymousUser = new Anonymous
    {
        SessionId = newSessionId,
    };

    _context.Anonymous.Add(newAnonymousUser);
    await _context.SaveChangesAsync(); 

    // Добавляем новый SessionId в заголовок ответа, чтобы клиент мог его запомнить
    Response.Headers["X-Session-Id"] = newSessionId.ToString();
    anonymousId = newAnonymousUser.Id;

    return (userId, anonymousId);
}

 [HttpGet("access/{accessLinkToken}/check-submission")]
 public async Task<IActionResult> CheckSubmission(Guid accessLinkToken)
 {
     var questionnaire = await _context.Questionnaires
         .FirstOrDefaultAsync(q => q.AccessLinkToken == accessLinkToken);

     if (questionnaire == null) return NotFound();

     var (userId, anonymousId) = await GetUserIdAndAnonymousIdAsync();

     var hasSubmitted = await _context.Answers.AnyAsync(a =>
         a.Question.QuestionnaireId == questionnaire.Id &&
         ((userId.HasValue && a.UserId == userId) ||
          (anonymousId.HasValue && a.AnonymousId == anonymousId)));

     return Ok(new { hasSubmitted });
 }

[HttpPost("access/{accessLinkToken}/questions/{questionId}/answer")]
public async Task<IActionResult> SubmitAnswer(Guid accessLinkToken, int questionId, [FromBody] AnswerRequest request)
{
    // Проверка на null
    if (request == null)
    {
        return BadRequest("Тело запроса не может быть пустым.");
    }

    // Находим анкету по ID
    var questionnaire = await _context.Questionnaires
        .FirstOrDefaultAsync(q => q.AccessLinkToken == accessLinkToken);

    if (questionnaire == null)
    {
        return NotFound("Анкета не найдена.");
    }

    // Находим вопрос по ID из пути
    var question = await _context.Questions
        .Include(q => q.Options) // Подключаем варианты ответов
        .FirstOrDefaultAsync(q => q.Id == questionId && q.QuestionnaireId == questionnaire.Id);

    if (question == null)
    {
        return NotFound("Вопрос не найден в указанной анкете.");
    }

    // Проверяем права доступа
    var (userId, anonymousId) = await GetUserIdAndAnonymousIdAsync();

    var hasExistingAnswers = await _context.Answers
        .AnyAsync(a => a.Question.QuestionnaireId == questionnaire.Id &&
        ((userId.HasValue && a.UserId == userId.Value) ||
        (anonymousId.HasValue && a.AnonymousId == anonymousId.Value)));

    if(hasExistingAnswers)
    {
        return BadRequest("Вы уже прошли данную анкету");
    }

    // Обработка ответа в зависимости от типа вопроса
    switch (question.QuestionTypeId)
    {
        case 1: // Текстовый вопрос
            if (string.IsNullOrEmpty(request.AnswerText))
            {
                return BadRequest("Для текстового вопроса требуется поле 'AnswerText'.");
            }

            var textAnswer = new Answer
            {
                Text = request.AnswerText,
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now
            };

            await _context.Answers.AddAsync(textAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Текстовый ответ успешно отправлен.",
                answerId = textAnswer.Id
            });

        case 2: // Выбор одного варианта
            if (!request.AnswerClose.HasValue)
            {
                return BadRequest("Для выбора одного варианта требуется поле 'AnswerClose'.");
            }

            var singleOption = question.Options.FirstOrDefault(o => o.Order == request.AnswerClose.Value);
            if (singleOption == null)
            {
                return BadRequest($"Неверный вариант ответа: {request.AnswerClose.Value}");
            }

            var singleAnswer = new Answer
            {
                Text = null,
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now,
                SelectOption = singleOption.Id
            };

            await _context.Answers.AddAsync(singleAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответ успешно отправлен.",
                answerId = singleAnswer.Id
            });

        case 3: // Выбор нескольких вариантов
            if (request.AnswerMultiple == null || !request.AnswerMultiple.Any())
            {
                return BadRequest("Для выбора нескольких вариантов требуется поле 'AnswerMultiple'.");
            }

            foreach (var order in request.AnswerMultiple)
            {
                var option = question.Options.FirstOrDefault(o => o.Order == order);
                if (option == null)
                {
                    return BadRequest($"Неверный вариант ответа: {order}");
                }

                var multipleAnswer = new Answer
                {
                    Text = null,
                    QuestionId = questionId,
                    UserId = userId,
                    AnonymousId = anonymousId,
                    CreatedAt = DateTime.Now,
                    SelectOption = option.Id
                };

                await _context.Answers.AddAsync(multipleAnswer);
            }
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответы успешно отправлены."
            });

        case 4: // Шкальный вопрос
            if (!request.AnswerScale.HasValue)
            {
                return BadRequest("Для шкального вопроса требуется поле 'AnswerScale'.");
            }

            if (request.AnswerScale < 1 || request.AnswerScale > 10)
            {
                return BadRequest("Значение шкалы должно быть между 1 и 10.");
            }

            var scaleAnswer = new Answer
            {
                Text = request.AnswerScale.ToString(),
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now
            };

            await _context.Answers.AddAsync(scaleAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Шкальный ответ успешно отправлен.",
                answerId = scaleAnswer.Id
            });
        case 5: // Выпадающий список (Dropdown)
                // Проверяем, пришел ли ID опции в поле AnswerClose
            if (!request.AnswerClose.HasValue)
            {
                // Сообщение можно уточнить, что ожидается ID
                return BadRequest("Для выпадающего списка требуется ID выбранного варианта (в поле AnswerClose).");
            }

            // Ищем опцию напрямую по ID, который пришел в AnswerClose
            var dropdownOption = question.Options.FirstOrDefault(o => o.Id == request.AnswerClose.Value); // <<< ИЗМЕНЕНИЕ ЗДЕСЬ: Ищем по o.Id

            if (dropdownOption == null)
            {
                // Если опция с таким ID не найдена
                return BadRequest($"Неверный ID варианта ответа: {request.AnswerClose.Value}");
            }

            // Создаем ответ, сохраняя ID найденной опции
            var dropdownAnswer = new Answer
            {
                Text = null, // Текст не нужен для выбора из списка
                QuestionId = questionId,
                UserId = userId,
                AnonymousId = anonymousId,
                CreatedAt = DateTime.Now,
                SelectOption = dropdownOption.Id // Сохраняем ID выбранной опции
            };

            await _context.Answers.AddAsync(dropdownAnswer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Ответ (выпадающий список) успешно отправлен.",
                answerId = dropdownAnswer.Id
            });
        default:
            return BadRequest("Неизвестный тип вопроса.");
    }
}import { useState, useEffect } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';

// Типы
interface Author {
  firstName: string;
  lastName: string;
}

interface Option {
  id: number;
  optionText: string;
  order: number;
}

export interface Question {
  id: number;
  text: string;
  questionTypeId: number;
  options?: Option[];
  leftScaleValue?: string;
  rightScaleValue?: string;
  divisions?: number;
}

interface QuestionnaireData {
  title: string;
  questions: Question[];
  author: Author;
}

export type AnswerValue = string | number | number[] | null;

export interface ValidationErrors {
  [key: number]: string;
}

interface AnswersLogicState {
  author: Author;
  ansTitle: string;
  questions: Question[];
  answers: { [key: number]: AnswerValue };
  isLoading: boolean;
  apiError: string;
  validationErrors: ValidationErrors;
  isLoginModalOpen: boolean;
  isRegisterModalOpen: boolean;
  popup: PopupState;
}

interface AnswersLogicActions {
  handleSubmit: () => void;
  handleInputChange: (questionId: number, value: AnswerValue) => void;
  handleCheckboxChange: (questionId: number, optionId: number) => void;
  setLoginModalOpen: (open: boolean) => void;
  setRegisterModalOpen: (open: boolean) => void;
  submitAnswers: () => Promise<void>;
  validateAnswers: () => boolean;
  showPopup: (text: string, event: React.MouseEvent | null) => void;

}

export interface PopupState {
  visible: boolean;
  text: string;
  x: number;
  y: number;

}

export const useAnswersLogic = (): [AnswersLogicState, AnswersLogicActions] => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [author, setAuthor] = useState<Author>({ firstName: '', lastName: '' });
  const [ansTitle, setAnsTitle] = useState<string>('');
  const [questions, setQuestions] = useState<Question[]>([]);
  const [answers, setAnswers] = useState<{ [key: number]: AnswerValue }>({});
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [apiError, setApiError] = useState<string>('');
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>({});
  const [isLoginModalOpen, setLoginModalOpen] = useState<boolean>(false);
  const [isRegisterModalOpen, setRegisterModalOpen] = useState<boolean>(false);
  const [popup, setPopup] = useState<PopupState>({ visible: false, text: '', x: 0, y: 0 });

  useEffect(() => {
    const loadQuestionnaire = async () => {
      if (!id) {
        setApiError('Идентификатор анкеты не указан.');
        setIsLoading(false);
        return;
      }

      const token = localStorage.getItem('access_token');

      try {
        const response = await axios.get<QuestionnaireData>(
          `https://localhost:7109/questionnaire/access/${id}`,
          {
            headers: { Authorization: `Bearer ${token}` },
          }
        );

        // Сохраняем SessionId в куки
        const newSessionId = response.headers['x-session-id'];
        if (newSessionId) {
          Cookies.set('anonymousSessionId', newSessionId, { expires: 365, sameSite: 'Lax' });
        }

        const { title, questions: fetchedQuestions, author } = response.data;

        setAuthor(author || { firstName: '', lastName: '' });
        setAnsTitle(title);

        const processedQuestions = fetchedQuestions.map(q => {
          if (q.questionTypeId === 4) {
            const parts = q.text.split('|');
            return {
              ...q,
              text: parts[0] || q.text,
              leftScaleValue: parts[1] || 'Низкое',
              rightScaleValue: parts[2] || 'Высокое',
              divisions: parseInt(parts[3], 10) || 5,
            };
          }
          return q;
        });

        setQuestions(processedQuestions);

        const initialAnswers: { [key: number]: AnswerValue } = {};
        processedQuestions.forEach(q => {
          if (q.questionTypeId === 3) initialAnswers[q.id] = [];
          else if (q.questionTypeId === 4) initialAnswers[q.id] = Math.ceil((q.divisions || 5) / 2);
          else initialAnswers[q.id] = '';
        });
        setAnswers(initialAnswers);

        setIsLoading(false);
      } catch (err: any) {
        if (err.response?.status === 403) {
          navigate('/AlreadySubmitted');
        } else {
          setApiError(err.response?.data || 'Ошибка загрузки анкеты.');
        }
        setIsLoading(false);
      }
    };

    loadQuestionnaire();
  }, [id, navigate]);

  const validateAnswers = (): boolean => {
    const errors: ValidationErrors = {};
    let firstErrorId: number | null = null;

    for (const question of questions) {
      const answer = answers[question.id];
      let isEmpty = false;
      let errorMsg = 'Пожалуйста, ответьте на этот вопрос';
      const questionType = parseInt(question.questionTypeId.toString(), 10);

      switch (questionType) {
        case 1:
          isEmpty = !answer || !String(answer).trim();
          errorMsg = 'Пожалуйста, заполните это поле';
          break;
        case 2:
          isEmpty = answer === '' || answer === null || answer === undefined;
          errorMsg = 'Пожалуйста, выберите один вариант';
          break;
        case 3:
          isEmpty = !Array.isArray(answer) || answer.length === 0;
          errorMsg = 'Пожалуйста, выберите хотя бы один вариант';
          break;
        case 4:
          isEmpty = isNaN(parseInt(answer as string, 10));
          errorMsg = 'Пожалуйста, выберите значение на шкале';
          break;
        case 5:
          isEmpty = answer === '' || answer === null || answer === undefined;
          errorMsg = 'Пожалуйста, выберите один вариант из списка';
          break;
        default:
          break;
      }

      if (isEmpty) {
        errors[question.id] = errorMsg;
        if (firstErrorId === null) firstErrorId = question.id;
      }
    }

    setValidationErrors(errors);

    if (firstErrorId !== null) {
      const element = document.getElementById(`question-${firstErrorId}`);
      if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    }

    return Object.keys(errors).length === 0;
  };

  const showPopup = (text: string, event: React.MouseEvent | null): void => {
    const xPos = event ? event.clientX : window.innerWidth / 2;
    const yPos = event ? event.clientY : window.innerHeight / 2;
    setPopup({ visible: true, text, x: xPos, y: yPos });
    setTimeout(() => {
      setPopup((prev) => ({ ...prev, visible: false }));
    }, 1800);
  };

  const submitAnswers = async () => {
    setApiError('');
    setValidationErrors({});

    if (!validateAnswers()) {

      showPopup('Заполните все поля', null);
      return;
    }

    setIsLoading(true);

    try {
      for (const question of questions) {
        const answer = answers[question.id];
        let payload: { [key: string]: any } | null = null;
        const questionType = parseInt(question.questionTypeId.toString(), 10);

        switch (questionType) {
          case 1:
            payload = { AnswerText: String(answer).trim() };
            break;

          case 2:
            const selectedSingle = question.options?.find((opt) => opt.id === parseInt(answer as string, 10));
            if (!selectedSingle) {
              return;
            }
            payload = { AnswerClose: selectedSingle.order };
            break;

          case 3:
            if (!Array.isArray(answer) || answer.length === 0) {
              return;
            }
            const orders = answer
              .map((id) => question.options?.find((opt) => opt.id === id)?.order)
              .filter((order): order is number => order !== undefined);
            if (orders.length !== answer.length) {
              return;
            }
            payload = { AnswerMultiple: orders };
            break;

          case 4:
            const scaleValue = parseInt(answer as string, 10);
            if (isNaN(scaleValue) || scaleValue < 1 || scaleValue > (question.divisions || 5)) {
              return;
            }
            payload = { AnswerScale: scaleValue };
            break;

          case 5:
            const dropdownValue = parseInt(answer as string, 10);
            if (isNaN(dropdownValue) || answer === '') {
              return;
            }
            payload = { AnswerClose: dropdownValue };
            break;

          default:
            continue;
        }

        if (payload) {
          await axios.post(
            `https://localhost:7109/questionnaire/access/${id}/questions/${question.id}/answer`,
            payload,
            {
              headers: {
                Authorization: `Bearer ${localStorage.getItem('access_token')}`,
                'Content-Type': 'application/json',
                'X-Session-Id': Cookies.get('anonymousSessionId') || '',
              },
            }
          );
        }
      }

      // Успешная отправка
      navigate('/Thanks', { state: { questionnaireId: id } });

    } catch (err: any) {
      console.error('Ошибка при отправке ответов:', err);
      if (err.response?.status === 404) {
        setApiError('Анкета не найдена или закрыта.');
      } else {
        setApiError('Ошибка отправки. Попробуйте позже.');
      }
    } finally {
      setIsLoading(false);
    }
  };


  const handleSubmit = () => {
    submitAnswers();
  };

  const handleInputChange = (questionId: number, value: AnswerValue) => {
    setAnswers((prev) => ({ ...prev, [questionId]: value }));
    if (validationErrors[questionId]) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[questionId];
        return newErrors;
      });
    }
  };

  const handleCheckboxChange = (questionId: number, optionId: number) => {
    setAnswers((prev) => {
      const current = (prev[questionId] as number[]) || [];
      const updated = current.includes(optionId)
        ? current.filter((id) => id !== optionId)
        : [...current, optionId];
      return { ...prev, [questionId]: updated };
    });
    if (validationErrors[questionId]) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev };
        delete newErrors[questionId];
        return newErrors;
      });
    }
  };

  return [
    {
      author,
      ansTitle,
      questions,
      answers,
      isLoading,
      apiError,
      validationErrors,
      isLoginModalOpen,
      isRegisterModalOpen,
      popup,

    },
    {
      handleSubmit,
      submitAnswers,
      validateAnswers,
      handleInputChange,
      handleCheckboxChange,
      setLoginModalOpen,
      setRegisterModalOpen,
      showPopup,

    },
  ];
};
